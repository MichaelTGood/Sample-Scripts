using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PlayFab;
using PlayFab.Plugins.CloudScript;
using PlayFab.ServerModels;
using System.Collections.Generic;

namespace Kkachi
{
    public static class FetchGameCustomData
    {
        [FunctionName("FetchGameCustomData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("FetchGameCustomData has been requested to Execute");

            /*
            Foreach ItemInstance ("ii") from the user's inventory ("inventory"), search through
            the primary catalog ("catalog") for a matching catalogItem ("ci"), 
            and add them both (ii & ci) to the list ("catalogItemsToRead"). Then fetch and add relevent
            information on player's group members, before returning it as GameCustomData ("gamesInfo").

            catalog - !ownedEvents => catalogItemsToRead + groupMembers info => gamesInfo
            */

            ////////////////////////////////////////////////////////////////
            
            var context = await FunctionContext<dynamic>.Create(req);
            var playfabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            var serializer = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
            var serverapi = new PlayFabServerInstanceAPI(context.ApiSettings, context.AuthenticationContext);
            var groupAPI = new PlayFabGroupsInstanceAPI(context.ApiSettings, context.AuthenticationContext);
            var dataAPI = new PlayFabDataInstanceAPI(context.ApiSettings, context.AuthenticationContext);

            ////////////////////////////////////////////////////////////////

            /// <summary> The full, primary catalog, "Events".
            /// </summary>
            var catalog = (await serverapi.GetCatalogItemsAsync(new GetCatalogItemsRequest{CatalogVersion=Constants.Catalogs.EVENTS})).Result.Catalog;

            /// <summary> A list of ItemInstances from the user's inventory.
            /// </summary>
            var inventory = (await serverapi.GetUserInventoryAsync(new GetUserInventoryRequest{ PlayFabId = playfabId })).Result.Inventory;

            /// <summary> A collection of catalogItems and itemInstances consisting of only the ones that match the user's inventory.
            /// </summary>
            var catalogItemsToRead = new Dictionary<CatalogItem, ItemInstance>();
            
            /// <summary> This will be the final List that is returned.
            /// </summary>
            var gamesInfo = new Dictionary<string, GameCustomData>();

            ////////////////////////////////////////////////////////////////

            //-- Find matching pair for Inventory InstanceItem in Events Catalog
            foreach(var ii in inventory)
            {
                //-- Foreach replacing catalog.Find(), so it doesn't error out of everything if it doesn't find a match (though, it always should).
                foreach(var ci in catalog)
                {
                    if(ci.ItemId == ii.ItemId)
                        { catalogItemsToRead.Add(ci, ii); }
                }
            }

            //-- Prepares all the data for each event, specific for the User.
            foreach(var kvp in catalogItemsToRead)
            {
                var game = serializer.DeserializeObject<GameCustomData>(kvp.Key.CustomData);
                game.EventId = kvp.Key.ItemId;
                game.EventTitle = kvp.Key.ItemClass;

                //-- If InventoryItem has GroupId, then Fetch Group
                if(kvp.Value.CustomData != null && kvp.Value.CustomData.ContainsKey(Constants.Group.GROUP_ENTITY_KEY))
                { 
                    #region Get Group Info
                        
                        var groupInfo = await groupAPI.GetGroupAsync(new PlayFab.GroupsModels.GetGroupRequest{
                            Group = new PlayFab.GroupsModels.EntityKey{
                                Id = kvp.Value.CustomData[Constants.Group.GROUP_ENTITY_KEY],
                                Type = Constants.Entities.GROUP
                        }});

                        //-- Check for Error (will cause the whole event not to returned to player)
                        if(groupInfo.Error != null)
                        {
                            log.LogError("--- InventoryItem had CustomData for Group, but GROUP NOT FOUND.");
                            continue;
                        }

                        //-- Build the Group
                        game.Group = new Group(
                            game.EventId,
                            groupInfo.Result.GroupName.Remove(0, groupInfo.Result.GroupName.IndexOf('_') + 1),
                            groupInfo.Result.Group,
                            groupInfo.Result.Group.Id.Remove(5)
                        );

                    #endregion

                    #region Get list of members, call for their Profiles, build List<GroupMembers>

                        var memberList = await groupAPI.ListGroupMembersAsync(new PlayFab.GroupsModels.ListGroupMembersRequest{Group=groupInfo.Result.Group});

                        //-- Check for Error (will cause the whole event not to returned to player)
                        if(memberList.Error != null)
                        {
                            log.LogError("--- UNABLE TO FETCH GROUP'S MEMBER LIST");
                            continue;
                        }
                        
                        //-- Foreach "Role" (ie Admin, Members)
                        foreach(var role in memberList.Result.Members)
                        {
                            if(role.Members.Count != 0)
                            {
                                foreach(var member in role.Members)
                                {
                                    var memberToAdd = new GroupMember();
                                    //-- Add PlayFab Master Id
                                    memberToAdd.PlayfabId = member.Lineage[Constants.Entities.MASTER].Id;
                                    memberToAdd.TitlePlayerId = member.Key.Id;
                                    memberToAdd.Type = GroupMember.CodeType.Player;

                                    //-- Set the Role in Team based on "Role" in Group
                                    if(role.RoleId == Constants.Group.Roles.Admin)
                                        { memberToAdd.RoleId = GroupMember.MemberRoles.Leader; }
                                    else
                                        { memberToAdd.RoleId = GroupMember.MemberRoles.Member; }

                                    //-- Fetch and Add DisplayName
                                    var profile = await serverapi.GetPlayerProfileAsync(new GetPlayerProfileRequest{PlayFabId=memberToAdd.PlayfabId});

                                    if(profile.Error != null)
                                        { memberToAdd.DisplayName = "FETCH_ERROR"; }
                                    else
                                        { memberToAdd.DisplayName = profile.Result.PlayerProfile.DisplayName; }

                                    //-- Add member to master list
                                    game.Group.Members.Add(memberToAdd);
                                }
                            }
                        }

                    #endregion

                    #region Get MemberCodes

                        if(game.Group.Members.Count < Constants.Group.GROUP_SIZE_LIMIT)
                        {
                            var eventCatalog = await serverapi.GetCatalogItemsAsync(new GetCatalogItemsRequest{CatalogVersion=kvp.Key.ItemId});

                            //-- If NO errors
                            if(eventCatalog.Error == null)
                            {
                                var groupItem = eventCatalog.Result.Catalog.Find(x => x.ItemId == game.Group.CatalogId);
                                var customData = serializer.DeserializeObject<Dictionary<string, object>>(groupItem.CustomData);
                                var memberCodes = serializer.DeserializeObject<List<GroupMember>>(customData[Constants.Group.GROUP_MEMBERS_OBJECT].ToString());
                                game.Group.Members.AddRange(memberCodes);

                            }
                        }

                    #endregion

                }

                //-- Sets the DateTime property, based on other int properties.
                game.SetDateTime();

                //-- Checks that there are not two instances of the same event.
                if(!gamesInfo.ContainsKey(game.EventId))
                    { gamesInfo.Add(game.EventId, game); }
            }


            ////////////////////////////////////////////////////////////////

            log.LogInformation("Finished FetchGameCustomData.");

            return new OkObjectResult(serializer.SerializeObject(gamesInfo));
        }
    }
}
