using Com.IsartDigital.Platformer.Game;
using Com.IsartDigital.Platformer.Utils;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.UI.Gameplay
{
    public class TimerHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _Text = default;
        [SerializeField] private int _SecondToAdd = 1;
        [NonSerialized] public int currentTimer = 0;
        [NonSerialized] public int totalTime = 0;
        private string _TimeText = string.Empty;
        private Cinematic _Cinematic = null;

        public Coroutine coroutine;
        #region Singleton
        private static TimerHUD _Instance = null;

        public static TimerHUD GetInstance()
        {
            if(_Instance == null) _Instance = new TimerHUD();
            return _Instance;
        }

        private TimerHUD() : base() {}
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
            if (Cinematic.GetInstance() != null)
            {
                _Cinematic = Cinematic.GetInstance();
                _Cinematic.onEndCinematic += OnEndCinematic;
            }
        }

        public IEnumerator Timer()
        {
            while (true)
            {
                currentTimer += _SecondToAdd;

                if (_Text != null)
                {
                    _TimeText = currentTimer / 60 + " : " + currentTimer % 60;
                    _Text.text = _TimeText;
                }
                yield return new WaitForSeconds(_SecondToAdd);
            }
        }
        public void OnEndCinematic()
        {
            coroutine = StartCoroutine(Timer());
        }

        private void OnDestroy()
        {
            if (_Cinematic != null)
                _Cinematic.onEndCinematic -= OnEndCinematic;
            StopCoroutine(Timer());
            if (_Instance != null)
                _Instance = null;
        }
    }
}
