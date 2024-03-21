using PlayFab;
using TMPro;
using UnityEngine;
using PlayFab.ClientModels;
using Com.IsartDigital.Platformer.Localization;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

//Author : Bastien Chevallier
namespace Com.IsartDigital.Platformer.BDD
{
    public class PlayfabLogin : MonoBehaviour
    {
        public static PlayfabLogin Instance;

        [SerializeField] private GameObject startPanel, HUD, _LeaderboardButton;
        [SerializeField] private ChangeInput signUpTab, loginTab;
        [SerializeField] private TextMeshProUGUI username, userPassword, usernameLogin, userPasswordLogin, errorSignUp, errorLogin;
        private string encryptedPassword;
        public bool offline = false;
        public string playerID;
        public string playerName;
        private const string LEVEL_PASSED = "LevelPassed";

        private void Awake()
        {
            Instance = this;
        }

        public void SignUpTab()
        {
            signUpTab.gameObject.SetActive(true);
            errorLogin.text = "";
            loginTab.gameObject.SetActive(false);
            signUpTab.SelectFirst();
        }

        public void LoginTab()
        {
            errorSignUp.text = "";
            signUpTab.gameObject.SetActive(false);
            loginTab.gameObject.SetActive(true);
            loginTab.SelectFirst();
        }

        string Encrypt(string pPassword)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(pPassword);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public void OfflineMode()
        {
            offline = true;
            StartGame();
        }

        public void SignUp()
        {
            Translation lTranslation = errorSignUp.GetComponent<Translation>();
            if (username.text.Length < 4)
            {
                errorSignUp.text = LocalizationUtils.error_user_short;
                lTranslation.UpdateLoca();
                return;
            }
            if(userPassword.text.Length < 4)
            {
                errorSignUp.text = LocalizationUtils.error_pass_short;
                lTranslation.UpdateLoca();
                return;
            }


            RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest { Password = Encrypt(userPassword.text.Remove(userPassword.text.Length - 1)), Username = username.text.Remove(username.text.Length - 1), RequireBothUsernameAndEmail = false};
            errorSignUp.text = LocalizationUtils.loading;
            lTranslation.UpdateLoca();
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, RegisterSuccess, RegisterError);
        }

        public void RegisterSuccess(RegisterPlayFabUserResult result)
        {
            playerName = username.text;
            playerID = result.PlayFabId;
            UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = username.text
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, RegisterError);
        }
        
        public void RegisterError(PlayFabError error)
        {
            errorSignUp.text = ErrorHandler(error);
            Translation lTransla = errorSignUp.GetComponent<Translation>();
            lTransla.UpdateLoca();
        }


        public void LogIn()
        {
            LoginWithPlayFabRequest request = new LoginWithPlayFabRequest { Password = Encrypt(userPasswordLogin.text.Remove(userPasswordLogin.text.Length -1)), Username = usernameLogin.text.Remove(usernameLogin.text.Length -1)};
            errorLogin.text = LocalizationUtils.loading;
            Translation lTransla = errorLogin.GetComponent<Translation>();
            lTransla.UpdateLoca();
            PlayFabClientAPI.LoginWithPlayFab(request, LogInSuccess, LogInError);
        }

        public void LogInError(PlayFabError error)
        {
            errorLogin.text = ErrorHandler(error);
            print(ErrorHandler(error));
            Translation lTransla = errorLogin.GetComponent<Translation>();
            lTransla.UpdateLoca();
        }

        public void LogInSuccess(LoginResult result)
        {
            playerName = usernameLogin.text;
            playerID = result.PlayFabId;
            errorLogin.text = "";
            StartGame();
        }

        private void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
        {
            UpdatePlayerStatisticsRequest statRequest = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                        {
                            new StatisticUpdate
                            {
                                StatisticName = LEVEL_PASSED,
                                Value = 0
                            },
                        }
            };
            PlayFabClientAPI.UpdatePlayerStatistics(statRequest, OnStatInit, RegisterError);
        }

        private void OnStatInit(UpdatePlayerStatisticsResult result)
        {
            errorSignUp.text = "";
            StartGame();
        }

        private void StartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        private string ErrorHandler(PlayFabError error)
        {
            switch (error.Error)
            {
                case PlayFabErrorCode.InvalidTitleId:
                    return LocalizationUtils.error_invalid_id;
                case PlayFabErrorCode.AccountNotFound:
                    return LocalizationUtils.error_account_not_found;
                case PlayFabErrorCode.InvalidPassword:
                    return LocalizationUtils.error_invalid_password;
                case PlayFabErrorCode.InvalidUsername:
                    return LocalizationUtils.error_invalid_username;
                case PlayFabErrorCode.UsernameNotAvailable:
                    return LocalizationUtils.error_username_not_available;
                case PlayFabErrorCode.DuplicateUsername:
                    return LocalizationUtils.error_username_not_available;
                case PlayFabErrorCode.AutomationInvalidInput:
                    return LocalizationUtils.error_invalid_input;
                case PlayFabErrorCode.InvalidEmailOrPassword:
                    return LocalizationUtils.error_invalid_password;
                case PlayFabErrorCode.ServiceUnavailable:
                    return LocalizationUtils.error_connexion;
                case PlayFabErrorCode.InvalidParams:
                    return LocalizationUtils.error_invalid_parameters;
                case PlayFabErrorCode.InvalidUsernameOrPassword:
                    return LocalizationUtils.error_invalid_user_pass;
                case PlayFabErrorCode.APIClientRequestRateLimitExceeded:
                    return LocalizationUtils.error_too_many_request;
                default:
                    print(error.Error);
                    Debug.Log(error.GenerateErrorReport());
                    return LocalizationUtils.error_not_anticipated;
            }
        }
    }
}