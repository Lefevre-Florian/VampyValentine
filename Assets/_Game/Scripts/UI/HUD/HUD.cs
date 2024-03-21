using TMPro;
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Com.IsartDigital.Platformer.UI.Gameplay;
using static Com.IsartDigital.Platformer.GameManager;
using Com.IsartDigital.Platformer.SoundManager;
using FMODUnity;

// Author : Matteo Renaudin & Lefevre Florian
namespace Com.IsartDigital.Platformer.UI
{
    public class HUD : MonoBehaviour
    {
        #region Singleton
        private static HUD _Instance = null;

        public static HUD GetInstance()
        {
            if(_Instance == null) _Instance = new HUD();
            return _Instance;
        }

        private HUD() : base() {}
        #endregion
        
        private const string COLLECTIBLE_TEXT = " / ";
        
        [Serializable]
        private struct TupleCollectible
        {
            public GameManager.CollectibleType type;
            public TextMeshProUGUI text;
        }

        [Header("UI - Gameplay")]
        [SerializeField] private GameObject _GameplayContainer = null;

        [Space(2)]
        [SerializeField] private TupleCollectible[] _CollectibleTextTuples = null;

        [Header("UI - Mobile")]
        [SerializeField] private GameObject _MobileContainer = null;

        [Header("UI - Navigation")]
        [SerializeField] private Screen _PauseMenu = null;
        [SerializeField] private Screen _SettingMenu = null;

        private InputManager _InputManager;

        // Collectibles
        private ScoreManager.ScoreManager _ScoreManager = null;
        
        private Dictionary<GameManager.CollectibleType, TextMeshProUGUI> _CollectiblesText = null;

        public delegate void OnPauseDelegate(bool pIsPause);
        public OnPauseDelegate onPause = new OnPauseDelegate((bool pIsPause) => { });

        private ScriptSFX _SFXOnClick => GetComponent<ScriptSFX>();

        [SerializeField] private ParamRef _OnPause = null;
        [SerializeField] private ParamRef _OnResume = null;

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
            #if UNITY_STANDALONE
            _MobileContainer.SetActive(false);
            #endif

            int lLength = _CollectibleTextTuples.Length;
            if(lLength != 0)
            {
                _CollectiblesText = new Dictionary<GameManager.CollectibleType, TextMeshProUGUI>();
                for (int i = 0; i < lLength; i++)
                {
                    _CollectiblesText.Add(_CollectibleTextTuples[i].type,
                                          _CollectibleTextTuples[i].text);
                    _CollectibleTextTuples[i].text.transform.parent.transform.gameObject.SetActive(false);
                }

                _CollectibleTextTuples = null;

                _ScoreManager = ScoreManager.ScoreManager.GetInstance();
                _ScoreManager.OnCollectibleUpdated += UpdateCollectibleLabel;
                _ScoreManager.OnCollectibleDisplayStatusUpdated += CollectibleDisplayStatus;
            }

            _InputManager = new InputManager();
            _InputManager.Enable();

            _InputManager.Main.Pause.started += OnInputPause;

            _PauseMenu.CloseScreen();
            _SettingMenu.CloseScreen();
        }

        #region Global
        private void OnInputPause(InputAction.CallbackContext pContext)
        {
            _SFXOnClick.PlaySFX();
            if (GameManager.currentGameState == GameManager.GameState.Pause)
            {
                _PauseMenu.CloseScreen();
                _SettingMenu.CloseScreen();
                Resume();
                SoundManager.SoundManager.GetInstance().UpdateMusic(_OnResume);
            }
            else if (GameManager.currentGameState == GameManager.GameState.InGameLvl2
                    || GameManager.currentGameState == GameManager.GameState.InGameLvl1 || GameManager.currentGameState == GameState.Cinematic)
            {
                _PauseMenu.OpenScreen();
                SoundManager.SoundManager.GetInstance().UpdateMusic(_OnPause);
                Pause();
            }
        }

        public void Resume()
        {
            SoundManager.SoundManager.GetInstance().UpdateMusic(_OnResume);
            _GameplayContainer.SetActive(true);
            
            if (GameManager.currentLevel == GameManager.GameState.InGameLvl1)
                GameManager.currentGameState = GameManager.GameState.InGameLvl1;
            else if (GameManager.currentLevel == GameManager.GameState.InGameLvl2)
                GameManager.currentGameState = GameManager.GameState.InGameLvl2;

            onPause.Invoke(false);
            Time.timeScale = 1f;
            TimerHUD.GetInstance().coroutine = StartCoroutine(TimerHUD.GetInstance().Timer());
        }

        public void Restart()
        {
            SoundManager.SoundManager.GetInstance().UpdateMusic(_OnResume);
            if (GameManager.currentLevel == GameManager.GameState.InGameLvl1)
                GameManager.currentGameState = GameManager.GameState.InGameLvl1;
            else if (GameManager.currentLevel == GameManager.GameState.InGameLvl2)
                GameManager.currentGameState = GameManager.GameState.InGameLvl2;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Pause()
        {
            SoundManager.SoundManager.GetInstance().UpdateMusic(_OnPause);
            GameManager.currentGameState = GameManager.GameState.Pause;
            Time.timeScale = 0;
            StopCoroutine(TimerHUD.GetInstance().coroutine);

            onPause.Invoke(true);

            _GameplayContainer.SetActive(false);
        }
        #endregion

        #region Collectible

        private void UpdateCollectibleLabel(int pScore, int pMaxScore, GameManager.CollectibleType pCollectibleType)
        {
            if (_CollectiblesText[pCollectibleType] != null)
                _CollectiblesText[pCollectibleType].text = pScore.ToString() + COLLECTIBLE_TEXT + pMaxScore.ToString();
        }

        private void CollectibleDisplayStatus(bool pStatus, GameManager.CollectibleType pCollectibleType)
        {
            if (_CollectiblesText[pCollectibleType] != null)
            {
                _CollectiblesText[pCollectibleType].transform.parent.transform.gameObject.SetActive(pStatus);
                _CollectiblesText[pCollectibleType].transform.parent.transform.gameObject.GetComponent<CollectibleHUD>().SetTriggerOnCollect();
            }
        } 

        #endregion

        private void OnDestroy()
        {
            if (_ScoreManager != null)
            {
                _ScoreManager.OnCollectibleUpdated += UpdateCollectibleLabel;
                _ScoreManager.OnCollectibleDisplayStatusUpdated += CollectibleDisplayStatus;
                _ScoreManager = null;
            }

            _InputManager.Disable();
            _InputManager.Main.Pause.started -= OnInputPause;
            _InputManager = null;

            if (_Instance != null)
                _Instance = null;
        }

    }
}
