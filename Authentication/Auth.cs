using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Native;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AzureModels;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Auth : MonoBehaviour
{
    #region Variables

        /// <summary> Singleton instance of Authorization Manager
        /// </summary>
        private static Auth instance = null;
        public static Auth AuthManager { get => instance; }
        

        //-- Player's profile information

        /// <summary> The local player's information.
        /// </summary>
        private static PlayerInfo playerInfo = new PlayerInfo();
        public static PlayerInfo PlayerInfo { get => playerInfo; }

        /// <summary> The group for the event that is currently being played.
        /// </summary>
        private static Group group = new Group();
        public static Group Group { get => group; }
        public static Group SetGroup { set => group = value; }

        /// <summary> This is all the data for the games currently in the inventory.
        /// </summary>
        private static Dictionary<string, GameCustomData> gamesInfo = new Dictionary<string, GameCustomData>();
        public static Dictionary<string, GameCustomData> GamesInfo { get => gamesInfo; }


        //-- Managers

        private GameManager gameManager;

        private DataManager dataManager;

        [SerializeField] 
        private PartyManager partyManager;
        public PartyManager PartyManager { get => partyManager; }

        [SerializeField] 
        private MainMenuViewController mmViewController;

        private IAppleAuthManager _appleAuthManager = null;


        //-- Internal Use Privates

        /// <summary> Requests: AccountInfo, UserData, and PlayerProf (incl: display name, email, date created, and last login)
        /// </summary>
        private GetPlayerCombinedInfoRequestParams PlayerInfoRequest = new GetPlayerCombinedInfoRequestParams
                                                    { GetPlayerProfile = true,
                                                        GetUserAccountInfo = true,
                                                        GetUserData = true,
                                                        ProfileConstraints = new PlayerProfileViewConstraints{
                                                            ShowContactEmailAddresses = true,
                                                            ShowCreated = true,
                                                            ShowDisplayName = true,
                                                            ShowLastLogin = true
                                                        } };
        
        /// <summary> Prefix Text for console and on screen display for PlayFab messages.
        /// </summary>
        private const string playfabTitle = "<color=\"orange\">PlayFab:</color>";


        //-- Debug (obviously)

        [SerializeField] private TextMeshProUGUI debug;
        public TextMeshProUGUI DebugText { get => debug; }
        
        

    #endregion


    private void Awake()
    {
        //Singleton Check
        if(instance == null)
            { instance = this; }
        else 
            { Destroy(this.gameObject); }

        if(gameManager == null)
        {
            try
            {
                gameManager = GameObject.FindObjectOfType<GameManager>();
                gameManager.AuthManagerConnect(this);
            }
            catch (Exception e)
            {
                Debug.Log("<color=\"red\"><b>WARNING</b></color>: Could not find <color=\"purple\">GameManager</color> \n" + e);
            }
            
        }

        if(mmViewController == null)
        {
            try
            {
                mmViewController = GameObject.FindObjectOfType<MainMenuViewController>();
            }
            catch (Exception e)
            {
                Debug.Log("<color=\"red\"><b>WARNING</b></color>: Could not find <color=\"purple\">mmViewController</color> \n" + e);
            }
            
        }
        

        //-- Sign in with Apple
        //-- Currently bugged //TODO
        // If the current platform is supported
        // if (AppleAuthManager.IsCurrentPlatformSupported)
        // {
        //     // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
        //     var deserializer = new PayloadDeserializer();
        //     // Creates an Apple Authentication manager with the deserializer
        //     this._appleAuthManager = new AppleAuthManager(deserializer);    
        // }

    }

    private void Start()
    {   
        //-- If KeepMeLoggedIn has been set, will login, and then check for email/apple/google account
        if(CCC.Authentication.KeepMeLoggedIn)
            { SilentLogin(); }
        else    //-- if false or never set, it will not silent login, but ask for L or R.
            { mmViewController.GoToLoginOrRegister(); }

        Debug.Log($"DeviceID: {GetDeviceId()}");
    }

    private void Update()
    {
        //-- Required for AppleAuth API 
        if(_appleAuthManager != null)
            { _appleAuthManager.Update(); }
    }

    #region Logins

        #region SILENT LOGINS

            public void SilentLogin()
            {
                debug.text += $"\nAttemping Silent login";
                mmViewController.TurnOffScreens();
                SilentPlatformLogin(OnLoginSuccess, onPlayFabError);
            }

            private void SilentPlatformLogin(Action<LoginResult> result, Action<PlayFabError> error)
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.IPhonePlayer:
                        iOSLogin(result, error);
                        break;
                    case RuntimePlatform.Android:
                        AndroidLogin(result, error);
                        break;
                    default:
                        DesktopLogin(result, error);
                        break;
                }
            }

            /// <summary> Logins in with DeviceId.
            /// </summary>
            private void iOSLogin(Action<LoginResult> resultDevice, Action<PlayFabError> errorDevice)
            {
                var request = new LoginWithIOSDeviceIDRequest
                {
                    TitleId = PlayFabSettings.TitleId,
                    DeviceModel = SystemInfo.deviceModel,
                    OS = SystemInfo.operatingSystem,
                    DeviceId = GetDeviceId(),
                    CreateAccount = true,
                    InfoRequestParameters = PlayerInfoRequest
                };

                PlayFabClientAPI.LoginWithIOSDeviceID(request, resultDevice, errorDevice);
            }

            /// <summary> Logins in with DeviceId.
            /// </summary>
            private void AndroidLogin(Action<LoginResult> resultDevice, Action<PlayFabError> errorDevice)
            {
                var request = new LoginWithAndroidDeviceIDRequest
                {
                    TitleId = PlayFabSettings.TitleId,
                    AndroidDevice = SystemInfo.deviceModel,
                    OS = SystemInfo.operatingSystem,
                    AndroidDeviceId = GetDeviceId(),
                    CreateAccount = true,
                    InfoRequestParameters = PlayerInfoRequest
                };

                PlayFabClientAPI.LoginWithAndroidDeviceID(request, resultDevice, errorDevice);
            }

            /// <summary> Logins in with CustomId of PC ID.
            /// </summary>
            private void DesktopLogin(Action<LoginResult> resultDevice, Action<PlayFabError> errorDevice)
            {
                var request = new LoginWithCustomIDRequest
                {
                    TitleId = PlayFabSettings.TitleId,
                    CustomId = GetDeviceId(),
                    CreateAccount = true,
                    InfoRequestParameters = PlayerInfoRequest
                };

                PlayFabClientAPI.LoginWithCustomID(request, resultDevice, errorDevice);
                        
            }

        #endregion
   
        /// <summary> Automatically logins into PlayFab with predetermined accounts. Only for development use!
        /// </summary>
        public void AutoLogin()
        {
            string pfCustomId = "";

            #if UNITY_EDITOR
                pfCustomId = "PFPT_UnityEditor";
            #elif UNITY_STANDALONE_WIN
                pfCustomId = "PFPT_PC";
            #elif UNITY_IOS	
                pfCustomId = "PFPT_iPhone";
            #elif UNITY_ANDROID
                pfCustomId = "PFPT_Android";
            #else
                pfCustomId = "PFPT_unknown";
            #endif
            

            // Log into playfab.
            var request = new LoginWithCustomIDRequest { 
                CustomId = pfCustomId, 
                CreateAccount = true, 
                InfoRequestParameters = PlayerInfoRequest
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, onPlayFabError);

        }

        /// <summary> Log in player with their inputed email and password.
        /// </summary>
        public void LoginWithEmail()
        {
            debug.text += $"\nAttempting Email Login...";
            mmViewController.LoginScreen.interactable = false;
            var loginData = mmViewController.Request_LoginScreenData();
            
            LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest{
                TitleId = PlayFabSettings.TitleId,
                Email = loginData.Email,
                Password = loginData.Password,
                InfoRequestParameters = PlayerInfoRequest
            };

            PlayFabClientAPI.LoginWithEmailAddress(request,
                (result)=>
                    {
                        CCC.Authentication.SetKeepMeLoggedIn(loginData.KeepMeLoggedIn);
                        playerInfo.UpdateInfo(result);
                        OnLoginSuccess(result);
                    },
                (error)=>
                    { 
                        onPlayFabError(error); 
                        mmViewController.LoginScreen.interactable = true;
                    }
            );
        }

        /// <summary> Used to log in players with existing accounts with Apple ID registered.
        /// </summary>
        public void QuickLogin_Apple()
        {
            var quickArgs = new AppleAuthQuickLoginArgs();
            _appleAuthManager.QuickLogin(quickArgs, null, onAppleSignInError);
        }
    
    #endregion

    #region Logout

        /// <summary> Removes all local data on player, and returns the app to a state of fresh install.
        /// </summary>
        public void onLogout()
        {
            PlayFabClientAPI.ForgetAllCredentials();
            playerInfo = new PlayerInfo();
            gamesInfo.Clear();
            CCC.Authentication.SetKeepMeLoggedIn(false);
            mmViewController.GoToLoginOrRegister();
        }

    #endregion

    #region Register Account Details

        /// <summary> Adds Email and Password to account.
        /// </summary>
        public  void Register_NewAccount_Email()
        {
            var registerData = mmViewController.Request_RegisterScreenData();

            var request = new AddUsernamePasswordRequest{
                Email = registerData.Email,
                Username = GenerateUsername(registerData.Firstname, registerData.Lastname),
                Password = registerData.Password,
            };

            PlayFabClientAPI.AddUsernamePassword(request,
                (response) => 
                {
                    var name = new Dictionary<string, string>{
                        {CCC.Authentication.Firstname, registerData.Firstname},
                        {CCC.Authentication.Lastname, registerData.Lastname}
                    };
                    Azure.Update.UserData(name);
                    playerInfo.Email = registerData.Email;
                    playerInfo.Username = response.Username;
                    CCC.Authentication.SetKeepMeLoggedIn(registerData.KeepMeLoggedIn);
                    StartCoroutine(onNewAccount(response, registerData.Email)); 
                },
                onPlayFabError);

        }

        /// <summary> Fetches player's data from Apple account, and stores it locally and to PlayFab.
        /// </summary>
        public void Register_NewAccount_Apple()
        {
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
            debug.text += $"\nAttempting Apple Registration";
            
            _appleAuthManager.LoginWithAppleId(
                loginArgs,
                (credential) =>
                {
                    debug.text += $"\nApple Login success!";
                    var data = credential as IAppleIDCredential;
                    if (data != null)
                    {
                        debug.text += $"\nInside Apple data check.";
                        
                        if(!string.IsNullOrEmpty(data.FullName.FamilyName))
                        {
                            debug.text += $"\nFamilyname is not null";
                            debug.text += $"\n{data.FullName.FamilyName}";
                        }
                        else
                        {
                            debug.text += $"\nFamilyName IS NULL";
                        }

                        if(data.FullName != null)
                        {
                            debug.text += $"\nFullname is not null.";
                            debug.text += $"\n{data.FullName.ToLocalizedString()}";
                        }
                        else
                        {
                            debug.text += $"\nFullname IS NULL";
                        }





                        debug.text += $"\nName: {data.FullName.FamilyName}";

                        var name = new Dictionary<string, string>{
                            {CCC.Authentication.Firstname, data.FullName.GivenName},
                            {CCC.Authentication.Lastname, data.FullName.FamilyName}
                        };
                        Azure.Update.UserData(name);
                        playerInfo.UpdateInfo_Apple(data);
                        //TODO Apple user's PF Username?
                        // playerInfo.Username = GenerateUsername(data.FullName.GivenName, data.FullName.FamilyName);

                        var request = new LinkAppleRequest{
                            IdentityToken = Encoding.UTF8.GetString(data.IdentityToken, 0, data.IdentityToken.Length)
                        };

                        debug.text += $"\nAttempting PF LinkApple";

                        PlayFabClientAPI.LinkApple(request,
                            (response) => 
                            {
                                debug.text += $"\nLinkApple Success! \nAttempting Add username";

                                var requestUsername = new AddUsernamePasswordRequest{
                                Email = playerInfo.Email,
                                Username = GenerateUsername(data.FullName.GivenName, data.FullName.FamilyName),
                                Password = GeneratePassword()
                                };

                                PlayFabClientAPI.AddUsernamePassword(requestUsername,
                                    (responseUsername) => 
                                    {
                                        debug.text += $"\nAdd Username/PW success";
                                        StartCoroutine(onNewAccount(responseUsername, playerInfo.Email)); 
                                    },
                                    onPlayFabError);
                            },
                            onPlayFabError);
                    }
                    else
                    {
                        debug.text += $"\nNo apple data returned?";
                    }

                    
                },
                onAppleSignInError);
        }

        /// <summary> Coroutine: Handles post- new account, such as registering Contact Email and default DisplayName.
        /// Also, sends next command to MainMenuViewController.
        /// </summary>
        private IEnumerator onNewAccount(AddUsernamePasswordResult response, string email)
        {
            debug.text += $"\nAttempting onNewAccount";
            yield return Register_ContactEmail(email);

            //-- Waiting required to not overload PlayFab requests
            yield return new WaitForSeconds(0.5f);
            
            yield return Register_DisplayName(response.Username);

            yield return new WaitForSeconds(0.5f);

            yield return Fetch_PlayerDataFull();

            debug.text += $"\nonNewAccount should have succeeded";

            mmViewController.onRegisterBtnReset();
        }

        /// <summary> Coroutine: registers the email as the user's Contact Email in PlayFab.
        /// </summary>
        private IEnumerator Register_ContactEmail(string newEmail)
        {
            
            var request = new AddOrUpdateContactEmailRequest{
                EmailAddress = newEmail
            };

            PlayFabClientAPI.AddOrUpdateContactEmail(request,
            (result)=>
            {
                Debug.Log($"{playfabTitle} Updated Contact Email: {newEmail}");
            }, onPlayFabError);

            yield return null;
            
        }

        /// <summary> Coroutine: registers the Display Name in PlayFab.
        /// </summary>
        private IEnumerator Register_DisplayName(string newDisplayname)
        {
            var request = new UpdateUserTitleDisplayNameRequest{
                DisplayName = newDisplayname
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            (result)=>
            {
                playerInfo.DisplayName = result.DisplayName;
                Debug.Log($"{playfabTitle} Updated DisplayName: {result.DisplayName}");
            }, onPlayFabError);

            yield return null;
        }

        public void Register_DeviceId()
        {
            #if UNITY_IOS
                var request = new LinkIOSDeviceIDRequest{
                    DeviceId = GetDeviceId(),
                    DeviceModel = SystemInfo.deviceModel,
                    OS = SystemInfo.operatingSystem,
                    ForceLink = true
                };
                PlayFabClientAPI.LinkIOSDeviceID(request,
                (result)=>
                {
                    Debug.Log($"DeviceId Apple Has been updated.");
                }, onPlayFabError);

            #endif

            #if UNITY_ANDROID
                var request = new LinkAndroidDeviceIDRequest{
                    AndroidDeviceId = GetDeviceId(),
                    AndroidDevice = SystemInfo.deviceModel,
                    OS = SystemInfo.operatingSystem,
                    ForceLink = true
                };
                PlayFabClientAPI.LinkAndroidDeviceID(request,
                (result)=>
                {
                    Debug.Log($"DeviceId Android Has been updated.");
                }, onPlayFabError);

            #endif

            mmViewController.GoToMainScreen();
        }

        /// <summary> Calls to Azure to verify the event code and add the event to the player's inventory.
        /// </summary>
        public void Register_EventCode()
        {
            mmViewController.EventCodeRegisterModal.interactable = false;

            Azure.Redeem.EventCode(mmViewController.Request_EventCode(),
            (response)=>
            {
                if(response.ResponseState == ResponseStates.Bad)
                {
                    Debug.LogError($"<color=\"red\">PlayFab Coupon Error:</color> \n{response.Message}");
                    gameManager.DebugMessage.DisplayError(response.Message.ToString());
                    mmViewController.EventCodeRegisterModal.interactable = true;

                }
                else 
                {
                    var serializer = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
                    foreach(var item in serializer.DeserializeObject<List<string>>(response.Message.ToString()))
                        { gameManager.DebugMessage.Display(item); }

                    StartCoroutine(Fetch_PlayerDataFull());
                    mmViewController.GoToGamesListModal();
                }

            });
        }

        /// <summary> Calls to Azure to verify the member code, add the event to the player's inventory and joins them to the group.
        /// </summary>
        public void Register_MemberCode()
        {
            mmViewController.MemberCodeRegisterModal.interactable = false;

            Azure.Redeem.MemberCode(mmViewController.Request_MemberCode(),
                (rmr) => 
                {
                    if(!rmr.Success)
                    {
                        if(string.IsNullOrEmpty(rmr.Message))
                            { onPlayFabError(rmr.Error); }
                        else
                            { Debug.LogError($"<color=\"orange\">PlayFab <color=\"red\">Error</color></color> : {rmr.Message}"); }
                        mmViewController.MemberCodeRegisterModal.interactable = true;
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(rmr.Message))
                            { gameManager.DebugMessage.Display(rmr.Message); }
                        StartCoroutine(Fetch_PlayerDataFull());
                        mmViewController.GoToGamesListModal();
                    }
                });
        }

    #endregion

    #region Profile

        /// <summary> A method to call the Coroutine Fetch_PlayerDataFull(). Useful for GUI buttons.
        /// </summary>
        public void onRefreshProfile() => StartCoroutine(Fetch_PlayerDataFull());

        /// <summary> Fetches and Refreshes all player data in PlayerInfo and GamesInfo.
        /// </summary>
        public IEnumerator Fetch_PlayerDataFull()
        {
            debug.text += "\nRefreshing profile...";

            mmViewController.RefreshMainScreen(true, 2);

            //-- Fetch profile from PlayFab
            var request = new GetPlayerCombinedInfoRequest{
                InfoRequestParameters = PlayerInfoRequest
            };

            PlayFabClientAPI.GetPlayerCombinedInfo(request,
            (response)=>
            {
                playerInfo.UpdateInfo(response.InfoResultPayload);
                debug.text += $"\nProfile found."; 
                mmViewController.ProfileDisplay_ProfileScreen.Set();
                mmViewController.ProfileDisplay_MainScreen.Set();
            },
            onPlayFabError);
            
            mmViewController.RefreshMainScreen(true, 2);

            //-- Fetch event inventory from player's account
            Azure.Fetch.GameCustomData(responseGCD => 
                { 
                    gamesInfo = responseGCD; 
                    debug.text += $"\nGameCustomData found."; 
                    mmViewController.ProfileDisplay_ProfileScreen.Set(); 
                    mmViewController.ProfileDisplay_MainScreen.Set();
                } );
            
            yield return new WaitForSeconds(0.5f);
        }

        /// <summary> Checks if this device is the user's registered device.
        /// </summary>
        /// <returns> TRUE = the current device is registered to this account.</returns>
        private bool CheckDeviceId(LoginResult result)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    Debug.Log("Checking Android Device Id");
                    if(result.InfoResultPayload.AccountInfo.AndroidDeviceInfo != null)
                    {
                        if(result.InfoResultPayload.AccountInfo.AndroidDeviceInfo.AndroidDeviceId == GetDeviceId())
                            { return true; }
                        else
                            { return false; }
                    }
                    else
                        { return false; }
                case RuntimePlatform.IPhonePlayer:
                    Debug.Log("Checking IOS Device Id");
                    if(result.InfoResultPayload.AccountInfo.IosDeviceInfo != null)
                    {
                        if(result.InfoResultPayload.AccountInfo.IosDeviceInfo.IosDeviceId == GetDeviceId())
                                { return true; }
                            else
                                { return false; }
                    }
                    else
                        { return false; }
                default:
                    Debug.Log("Checking Non-IOS/Android Device Id");
                    if(result.InfoResultPayload.AccountInfo.CustomIdInfo != null)
                    {
                        if(result.InfoResultPayload.AccountInfo.CustomIdInfo.CustomId == GetDeviceId())
                                { return true; }
                            else
                                { return false; }
                    }
                    else
                        { return false; }
            }
        }

    #endregion

    #region Group

        /// <summary> Calls to Azure to create a group for the event.
        /// </summary>
        /// <param name="eventId">The identifier for the event (ie: Zmeer2103)</param>
        /// <param name="groupName">The player's request for the group name.</param>
        /// <param name="result">The returned result to pass back.</param>
        public void CreateGroup(string eventId, string groupName, Action<bool> result)
        {
            //-- Adjust GUI elements
            mmViewController.RefreshMainScreen(true);

            //-- Call to PF/Azure to create a group
            Azure.Create.Group(eventId, groupName,
            (createGroupResult) =>
            {
                if(createGroupResult.Error == AzureModels.ErrorCode.none)
                {
                    //-- Full success
                    mmViewController.RefreshMainScreen(false);
                    gamesInfo[eventId].Group = createGroupResult.Group;
                    result(true);
                }
                else
                {
                    //-- Parse error message
                    // TODO Generate Error message feedback to player
                    gameManager.DebugMessage.DisplayError(Enum.GetName(typeof(AzureModels.ErrorCode), createGroupResult.Error));
                    Debug.LogError("<color=\"red\">Group Creation Response:</color> " + createGroupResult.Error.ToString() + " " + createGroupResult.Message);
                    
                    mmViewController.RefreshMainScreen(false);
                    result(false);
                }
            });
        }

        /// <summary> Checks if the local player is the leader of the team. Uses Auth.Group, make sure it is set before this method is called.
        /// </summary>
        public bool IsPlayerLeader()
        {
            //-- Verify from Group.Members. Don't store locally, in case it changes in the backend.
            if(group != null && 
                GroupMember.MemberRoles.Leader == group.Members.Find(x => x.PlayfabId == playerInfo.MasterPlayerAccount).RoleId)
            {
                { return true; }
            }

            return false;
        }

        /// <summary> Fetches the display name of the provided player, as stored in the Group data.
        /// </summary>
        public string FetchDisplayName(string titlePlayerId)
        {
            return group.Members.Find(x => x.TitlePlayerId == titlePlayerId).DisplayName;
        }

    #endregion

    #region Event Handlers

        /// <summary> Handles a PlayFab successful login.
        /// </summary>
        private void OnLoginSuccess(LoginResult result)
        {
            debug.text += $"\nLogin Succeed.";

            playerInfo.UpdateInfo(result);
            partyManager.onLogin();

            //-- If there is no login email or Apple or Google account
            if(string.IsNullOrEmpty(result.InfoResultPayload.AccountInfo.PrivateInfo.Email) 
             && result.InfoResultPayload.AccountInfo.AppleAccountInfo == null
             && result.InfoResultPayload.AccountInfo.GoogleInfo == null)
            {
                debug.text += $"\nNo registration found. Going to Register.";
                mmViewController.GoToRegister();
            }
            else if (!CheckDeviceId(result))
            {
                Debug.Log("DeviceId Does NOT match.");
                StartCoroutine(Fetch_PlayerDataFull());
                mmViewController.GoToDeviceIdRegister();
            }
            else //-- Fetch GameCustomData based on User's Inventory
            {
                debug.text += $"\nAccount is registered. Fetching Profile and GameCustomData.";
                Debug.Log($"Account is registered. Fetching Profile and GameCustomData.");
                StartCoroutine(Fetch_PlayerDataFull());
                mmViewController.GoToMainScreen();
            }
            
        }

        /// <summary> Handles a PlayFab error, reporting it to console, and on screen debug display.
        /// </summary>
        private void onPlayFabError(PlayFabError error)
        {
            // pfCallReturned= true;
            if(error.GenerateErrorReport().Contains(CCC.ErrorCodes.EmailAlreadyRegistered))
            {
                gameManager.DebugMessage.Display("This email address is already registered.");
                mmViewController.onRegisterBtnReset();
            }
            else
            {
                debug.text += $"\n<color=\"red\">PlayFab Error: </color>{error.GenerateErrorReport()}";
                Debug.LogError($"<color=\"red\">PlayFab Error: </color>\n {error.GenerateErrorReport()}");
            }
        }

        /// <summary> Handles an Apple Sign in error, reporting it to console, and on screen debug display.
        /// </summary>
        private void onAppleSignInError(IAppleError error)
        {
            var authorizationErrorCode = error.GetAuthorizationErrorCode();
            debug.text += $"\n<color=\"red\">Apple Error: </color>{authorizationErrorCode.ToString() + " " + error.ToString()}";
            Debug.LogError("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
        }


    #endregion

    #region Helper Methods

        /// <summary> Removes the key and value in PlayerPrefs for the Keep Me Logged In option.
        /// </summary>
        public void Clear_KeepMeLoggedIn()
        {
            if(PlayerPrefs.HasKey(CCC.Authentication.KeepMeLoggedInKey))
            {
                PlayerPrefs.DeleteKey(CCC.Authentication.KeepMeLoggedInKey);
                debug.text += "\nDelete KeepMeLoggedIn from PlayerPrefs";
            }
            else
            {
                debug.text += "\nFound no KeepMeLoggedIn in PlayerPrefs";
            }
        }

        /// <summary> Generates a unique username for new players.
        /// </summary>
        private string GenerateUsername(string firstname, string lastname)
        {
            //-- Remove all spaces
            firstname = string.Concat(firstname.Where(c => !char.IsWhiteSpace(c)));
            lastname = string.Concat(lastname.Where(c => !char.IsWhiteSpace(c)));

            //-- Combine first initial and lastname.
            string username = firstname.Remove(1) + lastname;
            username.ToLowerInvariant();

            //-- Limit username to 8 characters (12 total with UID).
            if(lastname.Length > 8)
                { username.Remove(8); }

            //-- Add random UID to username.
            username += UnityEngine.Random.Range(0, 10);
            username += UnityEngine.Random.Range(0, 10);
            username += UnityEngine.Random.Range(0, 10);
            username += UnityEngine.Random.Range(0, 10);

            return username;
        }

        /// <summary> This is used for the Sign in with Apple accounts, as they require a password to update info?
        /// </summary>
        private string GeneratePassword()
        {
            //-- REMOVED
            //-- Generates a (mostly) random string of characters based on internal requirements (ie: length, characters, cases, etc). 
        }

        /// <summary> Returns the current device's UID.
        /// </summary>
        private string GetDeviceId()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaObject currentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
                    AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
                    return secure.CallStatic<string>("getString", currentResolver, "android_id");
                    
                default:
                    return SystemInfo.deviceUniqueIdentifier;
            }
        }

        /// <summary> DEBUG: For testing Apple Sign In
        /// </summary>
        public void onLoadAppleScene()
        {
            SceneManager.LoadSceneAsync("AppleScene", LoadSceneMode.Additive);
        }

        /// <summary> Caches the PartyManager, when it awakes and reaches out to Auth Manager.
        /// </summary>
        public void PartyManagerHandshake(PartyManager pm)
        {
            if(partyManager == null)
                { partyManager = pm; }
        }


    #endregion
}   