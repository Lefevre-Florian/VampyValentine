using Com.IsartDigital.Platformer.SoundManager;
using FMODUnity;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer
{
    public class LoadScene : MonoBehaviour
    {
        #region Singleton
        private static LoadScene _Instance = null;

        public static LoadScene GetInstance()
        {
            if (_Instance == null) _Instance = new LoadScene();
            return _Instance;
        }

        private LoadScene() : base() { }
        #endregion

        private const float SCENE_LOADING_PERCENT = 0.9f;

        [Header("Canvas")]
        [SerializeField] private UI.Screen _LoadingScreen = default;

        [Header("Scenes Index")]
        public int lvl1Scene = 2;
        public int lvl2Scene = 3;

        // Variables
        private Coroutine _AsyncTimer = null;
        private AsyncOperation _AsyncLoader = null;

        // Events
        public event Action OnLoadPending;
        [SerializeField] private EventReference _LoadingMusic = default;
        [SerializeField] private ScriptSFX _LevelAmbient = default;
        private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(this);
                return;
            }

            _Instance = this;
        }

        public void StartLoadScene(int pSceneID)
        {
            SoundManager.SoundManager.GetInstance().StopMusic();
            SoundManager.SoundManager.GetInstance().StopMusic();
            //SoundManager.SoundManager.GetInstance().SetMusic(_LoadingMusic);
            //_LevelAmbient.PlayLongSFX();
            _AsyncTimer = StartCoroutine(LoadSceneAsync(pSceneID));
        }

        /// <summary>
        /// Loading screen renderer and logic computation
        /// </summary>
        /// <param name="pSceneID"></param>
        /// <returns></returns>
        private IEnumerator LoadSceneAsync(int pSceneID)
        {
            _LoadingScreen.OpenScreen();
            
            if (pSceneID == lvl1Scene)
            {
                GameManager.currentGameState = GameManager.GameState.InGameLvl1;
                GameManager.currentLevel = GameManager.GameState.InGameLvl1;
            }
            else if (pSceneID == lvl2Scene)
            {
                GameManager.currentGameState = GameManager.GameState.InGameLvl2;
                GameManager.currentLevel = GameManager.GameState.InGameLvl2;
            }

            _AsyncLoader = SceneManager.LoadSceneAsync(pSceneID);
            _AsyncLoader.allowSceneActivation = false;

            _LoadingScreen.gameObject.SetActive(true);
            while (!_AsyncLoader.isDone) 
            {
                if (_AsyncLoader.progress >= SCENE_LOADING_PERCENT)
                {
                    if (_AsyncTimer != null)
                    {
                        OnLoadPending?.Invoke();

                        StopCoroutine(_AsyncTimer);
                        _AsyncTimer = null;
                    }
                }
                yield return null;
            }
        }

        public void CompleteLoading() => _AsyncLoader.allowSceneActivation = true;

        private void OnDestroy()
        {
            // Security
            StopAllCoroutines();

            _AsyncLoader = null;
            _AsyncTimer = null;

            if (_Instance != null)
                _Instance = null;
        }
    }
}