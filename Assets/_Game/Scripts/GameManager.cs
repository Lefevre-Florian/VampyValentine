using Com.IsartDigital.Platformer.Game;
using Com.IsartDigital.Platformer.InGameElement.Checkpoint;
using Com.IsartDigital.Platformer.Player;
using Com.IsartDigital.Platformer.SoundManager;

using System;
using Cinemachine;
using UnityEngine;
using FMODUnity;
using Com.IsartDigital.Platformer.UI.Gameplay;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _Instance = null;

        public static GameManager GetInstance()
        {
            if (_Instance == null) _Instance = new GameManager();
            return _Instance;
        }

        private GameManager() : base() { }
        #endregion


        #region InGame Variables

        [SerializeField] private GameObject _WinScreen;

        public GameObject currentCamera = null;
        private PlayerBis.PlayerController _Player => PlayerBis.PlayerController.GetInstance();


        public Checkpoint lastCheckpoint = null;

        private Animator _PossibleAnimator;

        public float secondOnDeath = 10f;

        #endregion

        [NonSerialized] public int totalTime = 0;
        [SerializeField] private ParamRef _OnWinScreen;

        public enum GameState
        {
            Menu, InGameLvl1, InGameLvl2, Pause, Cinematic
        }
        public static GameState currentGameState = GameState.InGameLvl1;
        public static GameState currentLevel = GameState.InGameLvl1;

        public enum CollectibleType
        {
            Normal, Perch
        }

        private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(this);
                return;
            }

            _Instance = this;

            #if UNITY_STANDALONE || UNITY_EDITOR
            Application.targetFrameRate = 60;
            #elif UNITY_ANDROID || UNITY_IOS
            Application.targetFrameRate = 30;
            #endif
        }

        private void Start()
        {
            CameraSwitcher.cameraEvent.AddListener(OnCameraSwitch);
            _Player.onDeathDelegate += OnPlayerDeath;
            if (VariablesToSave.lastCheckpointBeforeLeave != null && currentGameState == currentLevel)
            {
                currentCamera = VariablesToSave.lastCheckpointBeforeLeave._CameraOnCheckpoint;
                _Player.transform.position = VariablesToSave.lastCheckpointBeforeLeave.transform.position;
            }
            currentCamera.SetActive(true);
        }


        private void OnPlayerDeath()
        {
            if (currentCamera.transform.parent.TryGetComponent<Animator>(out Animator pAnimator))
            {
                _PossibleAnimator = pAnimator;
            }

            else
                _PossibleAnimator = null;



            currentCamera.gameObject.SetActive(false);
            
            currentCamera = lastCheckpoint._CameraOnCheckpoint;
            currentCamera.gameObject.SetActive(true);
        }

        private void OnCameraSwitch(GameObject pCurrentCam)
        {
            currentCamera = pCurrentCam;
#if UNITY_STANDALONE
            MouseDetection.GetInstance().currentIndex = 0;
            MouseDetection.GetInstance().SetupPlatform();
#endif
        }
        public void OnLevelEnd()
        {
            _WinScreen.gameObject.SetActive(true);
            SoundManager.SoundManager.GetInstance().UpdateMusic(_OnWinScreen);
            LevelEvolution.GetInstance()?.ResetProgressBar();
            //Mettre en pause
            currentGameState = GameState.Pause;
        }


        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;

            _Player.onDeathDelegate -= OnPlayerDeath;

        }


    }

}
