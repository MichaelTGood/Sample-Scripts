using AzureModels;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary> A collection of static Azure server calls, made via PlayFab.
/// </summary>
public static class Azure
{
    private static ISerializerPlugin serializer = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

    public static class Create
    {
        /// <summary> Securely creates a group, and joins the player as the leader.
        /// </summary>
        /// <param name="eventId">The code for the player's event</param>
        /// <param name="groupName">The desired name for the group</param>
        /// <param name="result">The response returned by Azure.</param>
        public static void Group(string eventId, string groupName, Action<CreateGroup_Response> result) => 
            Group(new CreateGroup_Request(eventId, groupName), result);

        /// <summary> Securely creates a group, and joins the player as the leader.
        /// </summary>
        /// <param name="groupRequest">A pre-compiled request, containing the eventId and desired name for the group.</param>
        /// <param name="result">The response returned by Azure.</param>
        public static void Group(CreateGroup_Request groupRequest, Action<CreateGroup_Response> result)
        {
            var request = new ExecuteFunctionRequest{
                FunctionName = "CreateGroup",
                FunctionParameter = groupRequest
            };

            PlayFabCloudScriptAPI.ExecuteFunction(request, 
                (response)=> { result(serializer.DeserializeObject<CreateGroup_Response>(response.FunctionResult.ToString())); },
                onPlayFabError);
        }

    }

    public static class Fetch
    {

        /// <summary> Calls to Azure Functions (via PlayFab) and retrieves data (Suspects, Weapons, Puzzles)
        /// </summary>
        public static void EventData(DataRequest dataRequest, string saveDir, EventDataModel dataManager, 
                                            PuzzleManager puzzleManager, Action<FetchDataResult> fetchResult)
        {
            //-- Build Request
            var request = new ExecuteFunctionRequest{
                FunctionName = "PullData",
                FunctionParameter = dataRequest
            };

            //-- Call to Azure for All Data
            PlayFabCloudScriptAPI.ExecuteFunction(request,
            (result)=> 
            {
                var response = serializer.DeserializeObject<EventDataFile>(result.FunctionResult.ToString());
                
                CCC.FILE_MANAGEMENT.LOAD_DATA_FILE(response, dataManager, puzzleManager);


                //-- Check that the pulled data is not empty
                if (dataManager.Suspects.Length > 0 && !String.IsNullOrEmpty(dataManager.Suspects[dataManager.Suspects.Length - 1].LName))
                    { 
                        fetchResult(new FetchDataResult(true, "FetchEventData() successfully collected and set Event Data.")); 

                        //-- Save the full data file locally.
                        CCC.FILE_MANAGEMENT.SAVE(saveDir, CCC.FILE_MANAGEMENT.SAVE_TYPE.DataFile, null, response);
                    }
                else 
                    { fetchResult(new FetchDataResult(false, "FetchEventData() found no data online or errored.")); }

                
            },  onPlayFabError);

        }

        public static void EventsList(GameManager gameManager, Action<FetchDataResult> fetchResult, out List<GameCustomData> assembledGamesList)
        {

            List<GameCustomData> gamesList = new List<GameCustomData>();

            PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest{FunctionName = "PullGamesList"}, 
                (result) => 
                {

                    List<GameCustomData> response = serializer.DeserializeObject<List<GameCustomData>>(result.FunctionResult.ToString());

                    foreach(var game in response)
                    {
                        //-- Add object to the list.
                        // gamesList.Add(serializer.DeserializeObject<Game>(game.ToString()));
                        gamesList.Add(game);

                    };


                    if(gamesList.Count > 0 && !string.IsNullOrEmpty(gamesList[gamesList.Count - 1].EventId))
                        { fetchResult(new FetchDataResult(true, "Game List should be assembled and populated.")); }
                    else
                        { fetchResult(new FetchDataResult(false, "Fetched GamesList was empty.")); }
                }, onPlayFabError
            );

            //-- Return all the games collected back
            assembledGamesList = gamesList;
        }

        /// <summary> Returns the player's inventory items as GameCustomData, so it can be displayed and handled locally.
        /// </summary>
        public static void GameCustomData(Action<Dictionary<string, GameCustomData>> gamesInfo)
        {
            var request = new ExecuteFunctionRequest{
                FunctionName = "FetchGameCustomData"
            };

            PlayFabCloudScriptAPI.ExecuteFunction(request,
            (result)=> { gamesInfo(serializer.DeserializeObject<Dictionary<string, GameCustomData>>(result.FunctionResult.ToString())); },
            onPlayFabError);
        }

        /// <summary> Fetches all data collected from Player and stores it locally. Will also call for GameCustomData / owned Game Events.
        /// </summary>
        public static void PlayerProfileFull(GetPlayerCombinedInfoRequestParams playerInfoRequest, 
                                                        Action<Dictionary<string, GameCustomData>> gamesInfo)
        {
                var request = new GetPlayerCombinedInfoRequest{
                    InfoRequestParameters = playerInfoRequest
                };

                PlayFabClientAPI.GetPlayerCombinedInfo(request,
                (response)=>
                {
                    Auth.PlayerInfo.UpdateInfo(response.InfoResultPayload);
                    Azure.Fetch.GameCustomData(gamesInfo);

                }, onPlayFabError);


                

        }

    }

    public static class Redeem
    {
        /// <summary> Redeems a code (PlayFab Coupon) for an event, via Azure.
        /// </summary>
        /// <param name="eventCode">The 10 character code, aka PlayFab Coupon.</param>
        /// <param name="response">Response object from Azure.</param>
        public static void EventCode(string eventCode, Action<CouponResponse> response)
        {
            var redeemCouponRequest = new ExecuteFunctionRequest{
                FunctionName = "RedeemCoupon",
                FunctionParameter = new CouponRedemption{
                    PlayFabId = Auth.PlayerInfo.MasterPlayerAccount,
                    Coupon = eventCode,
                    CatalogVersion = CCC.Catalogs.EVENTS
                },
                GeneratePlayStreamEvent = true
            };

            PlayFabCloudScriptAPI.ExecuteFunction(
                redeemCouponRequest,
                (redeemCouponResponse)=> { response(serializer.DeserializeObject<CouponResponse>(redeemCouponResponse.FunctionResult.ToString())); }, 
                onPlayFabError
            );
            
        }

        /// <summary> Redeems a member code, so the player can join a team. Will build the request for Azure based on local player's profile.
        /// </summary>
        /// <param name="memberCode">The 12 character member code, provided by the leader of the team.</param>
        /// <param name="response">Response object from Azure.</param>
        public static void MemberCode(string memberCode, Action<RedeemMemberResponse> response)
        {
            Redeem.MemberCode(new RedeemMemberRequest{
                MemberCode = memberCode, 
                PlayFabId = Auth.PlayerInfo.MasterPlayerAccount,
                DisplayName = Auth.PlayerInfo.DisplayName 
                }, response);
        }

        /// <summary> Redeems a member code, so the player can join a team. Will build the request for Azure based on local player's profile.
        /// </summary>
        /// <param name="request">The compiled request, with member code and PlayFab ID</param>
        /// <param name="response">Response object from Azure.</param>
        public static void MemberCode(RedeemMemberRequest request, Action<RedeemMemberResponse> response)
        {
            var redeemMemberCodeRequest = new ExecuteFunctionRequest{
                FunctionName = "RedeemMember",
                FunctionParameter = request,
                GeneratePlayStreamEvent = true
            };

            PlayFabCloudScriptAPI.ExecuteFunction(
                redeemMemberCodeRequest,
                (memberCodeResponse) =>
                {
                    response(serializer.DeserializeObject<RedeemMemberResponse>(memberCodeResponse.FunctionResult.ToString()));

                },
                onPlayFabError
            );
        }

    }

    public static class Update
    {
        /// <summary> Adds or Updates Key-Value entries in the player's UserData. 
        /// </summary>
        /// <param name="data">The K/V pairs to add or update.</param>
        /// <param name="permission">private = only user. public = other player's can access it. Default = private</param>
        public static void UserData(Dictionary<string, string> data, UserDataPermission permission = UserDataPermission.Private)
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest{Data = data, Permission = permission}, null, onPlayFabError);
        }
        
    }

    public static class Verify
    {
        /// <summary> Calls to Azure Functions (via PlayFab) and checks the revision number on the data, returns if its up to date, or needs to be redownloaded.
        /// </summary>
        /// <param name="dataRequest">The identifying information for the player's current event data.</param>
        /// <param name="verifyResult">The response from Azure, if the player needs to call for a refreshed event data.</param>
        public static void EventData(DataRequest dataRequest, Action<VerifyDataResponse> verifyResult)
        {
            //-- Build Request
            var request = new ExecuteFunctionRequest{
                FunctionName = "VerifyData",
                FunctionParameter = dataRequest
            };

            //-- Call to Azure for All Data
            PlayFabCloudScriptAPI.ExecuteFunction(request,
            (result)=> 
            {
                verifyResult(serializer.DeserializeObject<VerifyDataResponse>(result.FunctionResult.ToString()));
            }, 
            onPlayFabError);

        }

    }

    /// <summary> Sends PlayFab error to the log
    /// </summary>
    private static void onPlayFabError(PlayFabError error)
    {
        Debug.LogError($"<color=\"orange\">PlayFab <color=\"red\">Error</color></color> : \n" + error.GenerateErrorReport());
        
    }


}
