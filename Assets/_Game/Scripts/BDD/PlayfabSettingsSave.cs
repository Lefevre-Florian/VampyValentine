using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : 
namespace Com.IsartDigital.Platformer.BDD
{
    public class PlayfabSettingsSave : MonoBehaviour
    {
        [SerializeField] private Toggle _LanguageToggle;
        [SerializeField] private Slider _MasterValue;
        [SerializeField] private Slider _MusicValue;
        [SerializeField] private Slider _SFXValue;
        [SerializeField] private Toggle _FullScreenToggle;
        [SerializeField] private Toggle _JoystickToggle;

        private const string LANGUAGE = "Language";
        private const string MASTER_SOUND = "MasterSound";
        private const string MUSIC_SOUND = "MusicSound";
        private const string SFX_SOUND = "SFXSound";
        private const string FULL_SCREEN = "Fullscreen";
        private const string JOYSTICK = "Joystick";

        void Start()
        {
            if(!PlayfabLogin.Instance.offline)
            {
                PlayfabData.GetUserData(
                    OnDataSuccess,
                    OnFailure);
            }
        }

        public void OnPressed()
        {
            if (!PlayfabLogin.Instance.offline)
            {
                PlayfabData.SaveData(new Dictionary<string, string>()
                {
                    {LANGUAGE,Convert.ToInt32(_LanguageToggle.isOn).ToString()},
                    {MASTER_SOUND,_MasterValue.value.ToString()},
                    {MUSIC_SOUND, _MusicValue.value.ToString()},
                    {SFX_SOUND, _SFXValue.value.ToString()},
                    {FULL_SCREEN,Convert.ToInt32(_FullScreenToggle.isOn).ToString()},
                    {JOYSTICK,Convert.ToInt32(_JoystickToggle.isOn).ToString()}
                },
                successResult =>
                {
                },
                OnFailure);
            }
        }

        private void OnDataSuccess(GetUserDataResult pResult)
        {
            foreach (var item in pResult.Data)
            {
                if (item.Key == MASTER_SOUND) PlayfabData.TryGetData<float>(item.Key, OnMasterGet, OnFailure);
                else if (item.Key == MUSIC_SOUND) PlayfabData.TryGetData<float>(item.Key, OnMusicGet, OnFailure);
                else if (item.Key == SFX_SOUND) PlayfabData.TryGetData<float>(item.Key, OnSFXGet, OnFailure);
                else if (item.Key == LANGUAGE) PlayfabData.TryGetData<int>(item.Key, OnLanguageGet, OnFailure);
                else if (item.Key == FULL_SCREEN) PlayfabData.TryGetData<int>(item.Key, OnFullScreenGet, OnFailure);
                else if (item.Key == JOYSTICK) PlayfabData.TryGetData<int>(item.Key, OnJoystickGet, OnFailure);
            }
        }

        private void OnMasterGet(float pMaster) 
        { 
            _MasterValue.value = pMaster;
            SoundManager.SoundManager.GetInstance().UpdateMasterBus((float)Math.Round(pMaster, 2));
        }

        private void OnMusicGet(float pMaster) 
        { 
            _MusicValue.value = pMaster;
            SoundManager.SoundManager.GetInstance().UpdateVCA(SoundManager.SoundManager.BusType.Music, (float)Math.Round(pMaster, 2));
        }
        private void OnSFXGet(float pMaster) 
        { 
            _SFXValue.value = pMaster;
            SoundManager.SoundManager.GetInstance().UpdateVCA(SoundManager.SoundManager.BusType.SFX, (float)Math.Round(pMaster, 2));
        }

        private void OnLanguageGet(int pLanguage) { _LanguageToggle.isOn = (pLanguage == 0 ? false : true); }
        private void OnFullScreenGet(int pFullScreen) { _FullScreenToggle.isOn = (pFullScreen == 0 ? false : true); }
        private void OnJoystickGet(int pJoystick) { _JoystickToggle.isOn = (pJoystick == 0 ? false : true); }

        private void OnFailure(PlayFabError error)
        {
            print(error.GenerateErrorReport());
        }
    }
}
