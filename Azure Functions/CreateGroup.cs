using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PlayFab.Plugins.CloudScript;
using PlayFab;
using PlayFab.AdminModels;
using System.Collections.Generic;

namespace Kkachi
{
    public static class CreateGroup
    {
        [FunctionName("CreateGroup")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("CreateGroup has been requested.");

            ////////////////////////////////////////////////////////////////////////
            
            #region Declarations

                var context = await FunctionContext<CreateGroup_Request>.Create(req);
                var groupRequest = context.FunctionArgument;
                var playfabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
                var groupResponse = new CreateGroup_Response();
                
                var serializer = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
                var adminAPI = new PlayFabAdminInstanceAPI(context.ApiSettings, context.AuthenticationContext);
                var groupAPI = new PlayFabGroupsInstanceAPI(context.ApiSettings, context.AuthenticationContext);
                var serverAPI = new PlayFabServerInstanceAPI(context.ApiSettings, context.AuthenticationContext);
                var dataAPI = new PlayFabDataInstanceAPI(context.ApiSettings, context.AuthenticationContext);

                /// <summary> The catalog for the Event the player has requested to make a group for.
                /// </summary>
                var catalog_Event = (await adminAPI.GetCatalogItemsAsync(new PlayFab.AdminModels.GetCatalogItemsRequest{CatalogVersion = groupRequest.EventId})).Result.Catalog;
                
                /// <summary> The catalog for the two character EventCodes.
                /// </summary>
                var catalog_EventCodes = (await serverAPI.GetCatalogItemsAsync(new PlayFab.ServerModels.GetCatalogItemsRequest{CatalogVersion = "EventCodes"})).Result.Catalog;

                /// <summary> A list of ItemInstances from the user's inventory.
                /// </summary>
                var inventory = (await serverAPI.GetUserInventoryAsync(new PlayFab.ServerModels.GetUserInventoryRequest{ PlayFabId = playfabId })).Result.Inventory;

                PlayFab.ServerModels.ItemInstance eventInInventory = new PlayFab.ServerModels.ItemInstance();

                /// <summary> The two character code for the event.
                /// </summary>
                string eventCode = null;

            #endregion

            ////////////////////////////////////////////////////////////////////////

            #region Verify Event Ownership

                //-- Check if the EventId matches an event in the player's inventory.
                bool hasEvent = false;
                foreach(var game in inventory)
                {
                    if(game.ItemId == groupRequest.EventId)
                    { 
                        hasEvent = true; 
                        eventInInventory = game;
                        break; 
                    }
                }

                //-- If the player does not have the event item purchased, return an error.
                if(!hasEvent)
                {
                    groupResponse.Success = false;
                    groupResponse.Error = ErrorCode.NotInInventory;
                    return new OkObjectResult(serializer.SerializeObject(groupResponse));
                }

                //-- If the player already has a group for this event, then return result.
                if(eventInInventory.CustomData != null && eventInInventory.CustomData.TryGetValue(Constants.Group.GROUP_ENTITY_KEY, out string groupEntity))
                {
                    groupResponse.Success = false;
                    groupResponse.Error = ErrorCode.UserAlreadyHasGroup;
                    groupResponse.Message = groupEntity;
                    return new OkObjectResult(serializer.SerializeObject(groupResponse));
                }
                
            #endregion

            #region Verify Group Name

                // //-- Flatten the name, and remove all spaces.
                string groupNameFlattened = groupRequest.GroupName.ToLowerInvariant().Trim();
                groupNameFlattened = string.Concat(groupNameFlattened.Where(c => !char.IsWhiteSpace(c)));

                //-- Cycle through all other names already registered for this event, flatten and remove spaces, then compare.
                bool dupName = false;
                foreach(var groupItem in catalog_Event)
                {
                    string existingName = groupItem.DisplayName.ToLowerInvariant().Trim();
                    existingName = string.Concat(existingName.Where(c => !char.IsWhiteSpace(c)));
                    if(existingName == groupNameFlattened)
                        { dupName = true; break; }
                }

                //-- If another group has the same basic name, return that in response.
                if(dupName)
                {
                    groupResponse.Success = false;
                    groupResponse.Error = ErrorCode.DuplicateName;
                    groupResponse.Group = new Group();
                    groupResponse.Message = "Group name already exists.";
                    return new OkObjectResult(serializer.SerializeObject(groupResponse));
                }

            #endregion

            #region Create Group

                //-- Create the group, and add the user
                var cgRequest = new PlayFab.GroupsModels.CreateGroupRequest{
                    GroupName = groupRequest.EventId + '_' + groupRequest.GroupName,
                    Entity = new PlayFab.GroupsModels.EntityKey{
                        Id = context.CallerEntityProfile.Lineage.TitlePlayerAccountId, 
                        Type = Constants.Entities.TITLE
                        }
                    };
                var createGroup = await groupAPI.CreateGroupAsync(cgRequest);
                
                //-- Verify no errors
                if(createGroup.Error != null)
                {
                    groupResponse.Success = false;
                    groupResponse.Error = ErrorCode.CreateGroupError;
                    groupResponse.Message = createGroup.Error.GenerateErrorReport();
                    return new OkObjectResult(serializer.SerializeObject(groupResponse));
                }
                else    //-- start creating the response for group.
                {
                    //-- Find the two character Event Code, used for creating and decyphering Member Codes
                    foreach(var item in catalog_EventCodes)
                    {
                        if(item.DisplayName == groupRequest.EventId)
                            { eventCode = item.ItemId; break; }
                    }

                    groupResponse.Group.Entity = createGroup.Result.Group;
                    groupResponse.Group.Name = createGroup.Result.GroupName.Remove(0, createGroup.Result.GroupName.IndexOf('_') + 1);
                    groupResponse.Group.Event = groupRequest.EventId;
                    groupResponse.Group.CatalogId = createGroup.Result.Group.Id.Remove(5);
                }

            #endregion
            
            #region Create and Set Member Codes
                
                //-- Generate Member Codes
                groupResponse.Group.Members = await MemberCodes.Create(
                    eventCode,
                    groupResponse.Group.CatalogId,
                    log);

                //-- Check for error in MemberCodes
                if(groupResponse.Group.Members == null)
                {
                    groupResponse.Error = ErrorCode.CreateGroupError;
                    groupResponse.Message = "CreateMemberCodes reported an error.";
                    return new OkObjectResult(serializer.SerializeObject(groupResponse));
                }

            #endregion 
            
            #region Create Event Catalog Item for Group 
            
                //-- Create the Item to represent the group.

                //-- Create KVP with Group Entity Id
                var groupCustomData = new Dictionary<string, object>();
                groupCustomData.Add(Constants.Group.GROUP_ENTITY_KEY, groupResponse.Group.Entity.Id);
                groupCustomData.Add(Constants.Group.GROUP_MEMBERS_OBJECT, groupResponse.Group.Members);

                //-- Assemble the group's catalog item, for the Event's Catalog
                var group = new CatalogItem{
                    ItemId = groupResponse.Group.CatalogId,
                    DisplayName = groupResponse.Group.Name,
                    ItemClass = "Group",
                    CustomData = serializer.SerializeObject(groupCustomData)
                };

                //-- Collection of Items/Groups (required for SetRequest)
                var catalogItemsToAdd = new List<CatalogItem>();
                catalogItemsToAdd.Add(group);

                //-- UpdateRequest will Update OR Create, but not delete! Best option
                var request = new UpdateCatalogItemsRequest{
                    CatalogVersion = groupRequest.EventId,
                    Catalog = catalogItemsToAdd,
                    SetAsDefaultCatalog = false
                };

                var setCatalogItemsResponse = await adminAPI.UpdateCatalogItemsAsync(request);

                //-- If the Update Catalog Items errored
                if(setCatalogItemsResponse.Error != null)
                {
                    groupResponse.Success = false;
                    groupResponse.Error = ErrorCode.UpdateCatalogItemError;
                    groupResponse.Message = setCatalogItemsResponse.Error.ErrorMessage;
                    return new OkObjectResult(serializer.SerializeObject(groupResponse));
                }

            #endregion

            #region Add Leader to MemberList

                //-- Fetch the Leader's Display Name
                var leaderDisplayName = await serverAPI.GetPlayerProfileAsync(
                    new PlayFab.ServerModels.GetPlayerProfileRequest{
                        PlayFabId = playfabId,
                        ProfileConstraints = new PlayFab.ServerModels.PlayerProfileViewConstraints{
                            ShowDisplayName = true
                        }});

                //-- Add Self/Leader
                GroupMember leader = new GroupMember{
                    DisplayName = leaderDisplayName.Result.PlayerProfile.DisplayName,
                    PlayfabId = playfabId,
                    TitlePlayerId = context.CallerEntityProfile.Lineage.TitlePlayerAccountId,
                    RoleId = GroupMember.MemberRoles.Leader,
                    Type = GroupMember.CodeType.Player
                };
                groupResponse.Group.Members.Insert(0, leader);

            #endregion

            #region Note Inventory Item that Group Exists

                var inventoryCustomData = new Dictionary<string, string>();
                inventoryCustomData.Add(Constants.Group.GROUP_ENTITY_KEY, groupResponse.Group.Entity.Id);

                var updateInventoryItemRequest = new PlayFab.ServerModels.UpdateUserInventoryItemDataRequest{
                    ItemInstanceId = eventInInventory.ItemInstanceId,
                    PlayFabId = playfabId,
                    Data = inventoryCustomData 
                    };
                var addGroup = await serverAPI.UpdateUserInventoryItemCustomDataAsync(updateInventoryItemRequest);

                if(addGroup.Error != null)
                {
                    groupResponse.Success = false;
                    groupResponse.Error = ErrorCode.UpdateUserInvetoryError;
                    groupResponse.Message = addGroup.Error.ErrorMessage;
                    return new OkObjectResult(serializer.SerializeObject(groupResponse));
                }

            #endregion
                
            //-- Then it should have worked.
            groupResponse.Success = true;

            ////////////////////////////////////////////////////////////////////////
            log.LogInformation("CreateGroup has Finished.");

            //-- Return Success Response
            return new OkObjectResult(serializer.SerializeObject(groupResponse));
        }
    }


    public class CreateGroup_Request
    {
        public string EventId;
        public string GroupName;

        public CreateGroup_Request() {}

        public CreateGroup_Request(string eventId, string groupName)
        {
            EventId = eventId;
            GroupName = groupName;
        }
    }

    public class CreateGroup_Response
    {
        public bool Success;
        public ErrorCode Error = 0;
        public string Message = null;
        public Kkachi.Group Group = new Kkachi.Group();
    }

    public enum ErrorCode { none, NotInInventory, UserAlreadyHasGroup, DuplicateName, CreateGroupError, UpdateCatalogItemError, UpdateUserInvetoryError };

}
