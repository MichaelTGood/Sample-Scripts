using PlayFab;

/// <summary> A collection of useful classes when communicating with Azure server.
/// </summary>
namespace AzureModels 
{

    public class PlayFabIdRequest
    {
        public string PlayFabId {get; set;}
    }

    #region Data Request | Response

        public class DataRequest : PlayFabIdRequest
        {
            public string EventCode { get ; set; }
            public int Revision { get; set; }
        }

        public class VerifyDataResponse
        {

            public bool DataRefreshRequired {get; set;}

            /// <summary> The Shared Access Signature for the EventDataFile.
            /// </summary>
            public string EDF_SAS {get; set;}

            /// <summary> The Shared Access Signature for the Images repository.
            /// </summary>
            public string Images_SAS {get; set;}

            public VerifyDataResponse() {}

            public VerifyDataResponse(VerifyDataResponse vdr)
            {
                DataRefreshRequired = vdr.DataRefreshRequired;
                EDF_SAS = vdr.EDF_SAS;
                Images_SAS = vdr.Images_SAS;
            }

            public VerifyDataResponse(bool dataRefreshRequired, string eDF_SAS, string images_SAS)
            {
                DataRefreshRequired = dataRefreshRequired;
                EDF_SAS = eDF_SAS;
                Images_SAS = images_SAS;
            }
        }

    #endregion

    #region Event Coupon Redemption

        public class CouponRedemption : PlayFabIdRequest
        {

            public string Coupon {get; set;}

            public string CatalogVersion {get; set;}

        }

        public class CouponResponse
        {
            public ResponseStates ResponseState;
            public object Message;
            
            public CouponResponse() {}
            public CouponResponse(ResponseStates responseState, object message)
            {
                ResponseState = responseState;
                Message = message;
            }
        }

        public enum ResponseStates
        {
            Good,
            Bad,
            Neutral
        }

    #endregion

    #region Create Group

        /// <summary> A basic request class to send to Azure.
        /// </summary>
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

        /// <summary> A response class to parse data from Azure.
        /// </summary>
        public class CreateGroup_Response
        {
            public bool Success;
            public ErrorCode Error = 0;
            public string Message = null;
            public Group Group = new Group();

            public CreateGroup_Response() {}

            public CreateGroup_Response(bool success, ErrorCode error, string message, Group group)
            {
                Success = success;
                Error = error;
                Message = message;
                Group = group;
            }
    }

        /// <summary> Error Codes for responses from Azure.
        /// </summary>
        public enum ErrorCode { none, NotInInventory, UserAlreadyHasGroup, DuplicateName, UpdateCatalogItemError, UpdateUserInvetoryError };

    #endregion

    #region Member Code Redemption
        
        public class RedeemMemberRequest : PlayFabIdRequest
        {
            public string MemberCode;
            public string DisplayName;

            public RedeemMemberRequest(string playFabId, string memberCode, string displayName)
            {
                PlayFabId = playFabId;
                MemberCode = memberCode;
                DisplayName = displayName;
            }

            public RedeemMemberRequest() {}
        }

        public class RedeemMemberResponse
        {
        public bool Success;
        public string Message;
        public PlayFabError Error = null;

        public RedeemMemberResponse() {}
        public RedeemMemberResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public RedeemMemberResponse(bool success, PlayFabError error)
        {
            Success = success;
            Error = error;
        }
        }

    #endregion

    /// <summary> A simple class to pass back result information.
    /// </summary>
    public class FetchDataResult
    {
        public bool Success;
        public string Message = null;
        public string Url = null;

        public FetchDataResult(bool success, string message = null, string url = null)
        {
            Success = success;
            Message = message;
            Url = url;
        }
    }

}
