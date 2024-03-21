using System;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer.Game
{
    public class Cinematic : MonoBehaviour
    {
        #region Singleton
        private static Cinematic _Instance = null;

        public static Cinematic GetInstance()
        {
            if(_Instance == null) _Instance = new Cinematic();
            return _Instance;
        }

        private Cinematic() : base() {}
        #endregion


        public delegate void cinematicDelegate();
        public cinematicDelegate onStartCinematic = new cinematicDelegate(() => { });
        public cinematicDelegate onEndCinematic = new cinematicDelegate(() => { });

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }

            _Instance = this;
        }

        public void StartCinematic()
        {
            GameManager.currentGameState = GameManager.GameState.Cinematic;
            onStartCinematic.Invoke();
        }

        public void EndCinematic()
        {
            GameManager.currentGameState = GameManager.GameState.InGameLvl1;
            onEndCinematic.Invoke();
        }

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }

    }
}
