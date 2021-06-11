using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
using PlayFab;
using PlayFab.Plugins.CloudScript;
using PlayFab.ServerModels;
using PlayFab.GroupsModels;
using PlayFab.DataModels;

namespace Kkachi
{
    public static class RedeemMember
    {
        [FunctionName("RedeemMember")]
        public static async Task<dynamic> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("RedeemMember has been requested to Execute");

            ////////////////////////////////////////////////////////////////
            
            var context = await FunctionContext<RedeemMemberRequest>.Create(req);
            RedeemMemberRequest rmr = context.FunctionArgument;
            RedeemMemberResponse rmResponse = new RedeemMemberResponse();

            //-- rmr.MemberCode
            CatalogItem eventItem = new CatalogItem(); //-- EventCodes item, used for getting Event's full code
            string eventName = null; //-- The Event's full code (ie: Zmeer2103)
            PlayFabResult<PlayFab.AdminModels.GetCatalogItemsResult> groupsCI = new PlayFabResult<PlayFab.AdminModels.GetCatalogItemsResult>();
            PlayFab.AdminModels.CatalogItem groupItem = new PlayFab.AdminModels.CatalogItem();
            PlayFab.GroupsModels.EntityKey groupEntityKey = new PlayFab.GroupsModels.EntityKey();

            var serializer = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer); 
            var serverAPI = new PlayFabServerInstanceAPI(context.ApiSettings, context.AuthenticationContext);
            var adminAPI = new PlayFabAdminInstanceAPI(context.ApiSettings, context.AuthenticationContext);
            var groupAPI = new PlayFabGroupsInstanceAPI(context.ApiSettings, context.AuthenticationContext);

            ////////////////////////////////////////////////////////////////

            #region Decypher provided Member Code

                //-- Check if the code is valid, and can be decypher into a Group and EventCode.
                var decypher = await MemberCodes.Decypher(rmr.MemberCode, log);

                //-- Check for Error
                if(!decypher.Success)
                    { return new OkObjectResult(serializer.SerializeObject(new RedeemMemberResponse(decypher.Success, decypher.Message))); }

                //-- Code should be valid.

            #endregion
            
            #region Get the Full Event Id

                //-- Get the EventCodes Catalog
                var eventCodesCatalog = await serverAPI.GetCatalogItemsAsync(new GetCatalogItemsRequest{
                    CatalogVersion = Constants.Catalogs.EVENT_CODES
                });

                //-- Check for errors
                if(eventCodesCatalog.Error != null)
                    { return new OkObjectResult(serializer.SerializeObject(new RedeemMemberResponse(false, eventCodesCatalog.Error))); }

                //-- Sort through and find the specific event's full code.
                eventItem = eventCodesCatalog.Result.Catalog.Find(x => x.ItemId == decypher.EventCode.ToLowerInvariant());
                eventName = eventItem.DisplayName;

            #endregion

            #region Get Group's Catalog Item and Remove MemberCode
                
                //-- Get all the groups for the specific event.
                groupsCI = await adminAPI.GetCatalogItemsAsync(new PlayFab.AdminModels.GetCatalogItemsRequest{
                    CatalogVersion = eventName
                });

                //-- Check for Error
                if(groupsCI.Error != null)
                    { return new OkObjectResult(serializer.SerializeObject(new RedeemMemberResponse(false, groupsCI.Error))); }

                //-- Find the group for the player
                groupItem = groupsCI.Result.Catalog.Find(x => x.ItemId.ToUpperInvariant() == decypher.Group.ToUpperInvariant());
                var groupCustomData = serializer.DeserializeObject<Dictionary<string, object>>(groupItem.CustomData);
                
                //-- Remove MemberCode from List
                var groupList = serializer.DeserializeObject<List<GroupMember>>(groupCustomData[Constants.Group.GROUP_MEMBERS_OBJECT].ToString());
                groupList.Remove(groupList.Find(x => x.PlayfabId == rmr.MemberCode));

                //-- Replace list in CatalogItem
                groupCustomData[Constants.Group.GROUP_MEMBERS_OBJECT] = groupList;
                groupItem.CustomData = serializer.SerializeObject(groupCustomData);

                //-- Return updated Item to Catalog
                var catalogItemList = new List<PlayFab.AdminModels.CatalogItem>() {groupItem};
                var returnedItem = await adminAPI.UpdateCatalogItemsAsync(new PlayFab.AdminModels.UpdateCatalogItemsRequest{
                    CatalogVersion = eventName,
                    Catalog = catalogItemList,
                    SetAsDefaultCatalog = false
                });

                // -- Group Entity Key (for later)
                groupEntityKey = new PlayFab.GroupsModels.EntityKey{
                    Id = groupCustomData[Constants.Group.GROUP_ENTITY_KEY].ToString(),
                    Type = "group"
                };

            #endregion

            #region Add Event to Player's Inventory

                //-- Grant the event item to the player's inventory
                var grantItem = await serverAPI.GrantItemsToUserAsync(new GrantItemsToUserRequest{
                    PlayFabId = rmr.PlayFabId,
                    CatalogVersion = Constants.Catalogs.EVENTS,
                    ItemIds = new List<string>() {eventName}
                });

                //-- Check for Error
                if(grantItem.Error != null)
                    { return new OkObjectResult(serializer.SerializeObject(new RedeemMemberResponse(false, grantItem.Error))); }

            #endregion

            #region Note in Inventory Item Custom Data
            
                //-- Prep Custom Data
                var inventoryCustomData = new Dictionary<string, string>();
                inventoryCustomData.Add(Constants.Group.GROUP_ENTITY_KEY, groupEntityKey.Id);

                //-- Add the Custom Data note to the Inventory Item
                var updateInventoryItemRequest = new PlayFab.ServerModels.UpdateUserInventoryItemDataRequest{
                    ItemInstanceId = grantItem.Result.ItemGrantResults[0].ItemInstanceId,
                    PlayFabId = rmr.PlayFabId,
                    Data = inventoryCustomData 
                    };
                var addGroup = await serverAPI.UpdateUserInventoryItemCustomDataAsync(updateInventoryItemRequest);

                //-- Check for Error
                if(addGroup.Error != null)
                    { return new OkObjectResult(serializer.SerializeObject(new RedeemMemberResponse(false, addGroup.Error))); }


            #endregion

            #region Join Group

                //-- Add player to Group
                var newMember = new List<PlayFab.GroupsModels.EntityKey>() {new PlayFab.GroupsModels.EntityKey{Id=context.CallerEntityProfile.Entity.Id, Type=context.CallerEntityProfile.Entity.Type}};
                var addMember = await groupAPI.AddMembersAsync(new AddMembersRequest{
                    Group = groupEntityKey,
                    Members = newMember,
                    RoleId = Constants.Group.Roles.Member
                });

                //-- Check for Error
                if(addMember.Error != null)
                    { return new OkObjectResult(serializer.SerializeObject(new RedeemMemberResponse(false, addMember.Error))); }


            #endregion

            ////////////////////////////////////////////////////////////////
            
            return new OkObjectResult(serializer.SerializeObject(new RedeemMemberResponse(true, "Successfully joined team.")));

        }
    }
}
