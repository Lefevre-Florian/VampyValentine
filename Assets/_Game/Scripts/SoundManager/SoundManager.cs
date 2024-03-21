using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using FMOD;
using System.Data.Common;
using Com.IsartDigital.Platformer.Game;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.SoundManager
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Bank's Variables")]
        [SerializeField] private int _Bank = 1;
        [SerializeField] private List<Bank> _BanksList = default;
        [NonSerialized] public Bank currentBank = null;
        [SerializeField] private Bank _MainLevelBank = null;
        [SerializeField] private Bank _UIBank = null;

        private int _LastBankID = 0;

        [Header("Bus Path")]
        [SerializeField] private string _MasterBusPath = "bus:/";
        private Bus _MasterBus;

        [Header("VCA Path")]
        [SerializeField] private string _MusicVCAPath = "vca:/Music";
        [SerializeField] private string _SFXVCAPath = "vca:/SFX";
        [SerializeField] private VCA _MusicVCA, _SFXVCA;

        private EventInstance _AmbiantSound;
        private EventInstance _Music;

        private Cinematic _Cinematic = null;
        public enum BusType
        {
            Master, Music, SFX
        }

        [SerializeField] private ParamRef _MusicParam;

        #region Singleton
        private static SoundManager _Instance = null;

        public static SoundManager GetInstance()
        {
            if(_Instance == null) _Instance = new SoundManager();
            return _Instance;
        }

        private SoundManager() : base() {}
        #endregion

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;
        }

        private void Start()
        {
            SetBankID();
            UnloadAllBanks();

            ChangeBank(_Bank);

            _MasterBus = RuntimeManager.GetBus(_MasterBusPath);

            _MusicVCA = RuntimeManager.GetVCA(_MusicVCAPath);
            _SFXVCA = RuntimeManager.GetVCA(_SFXVCAPath);

            if (Cinematic.GetInstance() != null)
            {
                _Cinematic = Cinematic.GetInstance();
                _Cinematic.onEndCinematic += UpdateMusicAfterCinematic;
            }

        }


        #region Bank's Function

        private void SetBankID()
        {
            foreach (Bank pBank in _BanksList)
            {
                _LastBankID++;
                pBank.bankID = _LastBankID;
            }
        }

        /// <summary>
        /// UnloadAllBanks : Unload all Banks exept Master and Master.strings
        /// </summary>
        private void UnloadAllBanks()
        {
            foreach(Bank pBank in _BanksList) 
                RuntimeManager.UnloadBank(pBank.bankPath);
        }

        /// <summary>
        /// ChangeBank : Change current Bank and load next Bank
        /// </summary>
        /// <param name="pBankID"> Next Bank ID</param>
        private void ChangeBank(int pBankID)
        {
            RuntimeManager.UnloadBank(_MainLevelBank.bankPath);
            RuntimeManager.UnloadBank(_UIBank.bankPath);

            if (currentBank != null) RuntimeManager.UnloadBank(currentBank.bankPath);
            currentBank = _BanksList.Find(pBank => pBank.bankID == pBankID);

            if (currentBank == null) return;

            if (currentBank.isLevelBank)
            {
                RuntimeManager.LoadBank(_MainLevelBank.bankPath);
            }
            RuntimeManager.LoadBank(_UIBank.bankPath);

            RuntimeManager.LoadBank(currentBank.bankPath);
        }
        #endregion

        public void PlaySFX(EventReference pSFX) => RuntimeManager.PlayOneShot(pSFX);
        
        /// <summary>
        /// CreateLoopSFX : Create loop sound
        /// </summary>
        /// <param name="pSFX"></param>
        /// <returns></returns>
        public EventInstance CreateLoop(EventReference pSound) => RuntimeManager.CreateInstance(pSound);
        public void SetAmbiantSound(EventReference pSound)
        {
            _AmbiantSound = CreateLoop(pSound);
            _AmbiantSound.start();
        }
        public void SetMusic(EventReference pMusic)
        {
            _Music = CreateLoop(pMusic);
            _Music.start();
        }

        public void StopMusic()
        {
            _Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _Music = default;
        }

        private void UpdateMusicAfterCinematic() => UpdateMusic(_MusicParam);

        public void UpdateMusic(ParamRef pParameter) => _Music.setParameterByName(pParameter.Name, pParameter.Value);

        public void UpdateMasterBus(float pValue) => _MasterBus.setVolume(pValue);

        public void UpdateVCA(BusType pType, float pValue)
        {
            switch (pType)
            {
                case BusType.Music:
                    _MusicVCA.setVolume(pValue);
                    break;
                case BusType.SFX:
                    _SFXVCA.setVolume(pValue);
                    break;
            }
        }

        private void OnDestroy()
        {
            if (_Cinematic != null)
            {
                _Cinematic.onEndCinematic -= UpdateMusicAfterCinematic;
            }

            if (_Instance != null) _Instance = null;
        }
    }
}