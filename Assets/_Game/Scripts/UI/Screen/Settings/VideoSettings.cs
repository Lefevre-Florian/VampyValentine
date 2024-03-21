using System;
using UnityEngine;
using UnityEngine.UI;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.UI.Settings
{
    public class VideoSettings : Settings
    {
        [SerializeField] private Toggle _FullScreen = default;
        [SerializeField] private Toggle _ShowJoystick = default;


        private Joystick.Joystick _Joystick = null;

        #region Singleton
        private static VideoSettings _Instance = null;

        public static VideoSettings GetInstance()
        {
            if(_Instance == null) _Instance = new VideoSettings();
            return _Instance;
        }

        private VideoSettings() : base() {}
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

        protected override void Start()
        {
            base.Start();

            #if UNITY_STANDALONE
            _FullScreen.onValueChanged.AddListener(delegate { OnFullScreen(); });

            #elif UNITY_ANDROID || UNITY_IOS
            _ShowJoystick.onValueChanged.AddListener(delegate { OnShowJoystick(); });
            _Joystick = Joystick.Joystick.GetInstance();
            #endif
        }

        private void OnFullScreen() => UnityEngine.Screen.fullScreen = _FullScreen.isOn;

        private void OnShowJoystick()
        {
            Joystick.Joystick.isShow = _ShowJoystick.isOn;
            Joystick.Joystick.GetInstance().UpdateJoysitckColor();
        }

        private void OnDestroy()
        {
            #if UNITY_STANDALONE
            _FullScreen.onValueChanged.RemoveListener(delegate { OnFullScreen(); });

            #elif UNITY_ANDROID || UNITY_IOS
            _ShowJoystick.onValueChanged.RemoveListener(delegate { OnShowJoystick(); });
            _Joystick = null;
            #endif

            if (_Instance != null)
                _Instance = null;
        }

    }
}
