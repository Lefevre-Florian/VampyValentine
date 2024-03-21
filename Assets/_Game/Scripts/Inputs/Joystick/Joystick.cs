using Com.IsartDigital.Platformer.Game;
using Com.IsartDigital.Platformer.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.Joystick
{
    public class Joystick : MonoBehaviour, IDesactivateHUD
    {
        [Header("Export Object")]
        public Image background = default;
        public Image handleImage = default;
        [SerializeField] private GameObject _Handle = default;
        [SerializeField] private Button _Button = default;

        [Header("Export Variables")]
        public float deadZone = 0.2f;
        public float deadZoneDown = -0.9f;

        [NonSerialized] public bool isBtnPressed = false;
        private Vector2 _Center;
        private Vector2 _TouchPosition;
        private Vector2 _JoystickSize;
        private Vector2 _HandlePose;

        public static bool isShow = true;
        private Touch _Touch;

        private PlayerBis.PlayerController _Player = null;

        public delegate void UpdatePlayer(float pValue);
        public UpdatePlayer updateMovement = new UpdatePlayer((pValue) => { });

        private Cinematic _Cinematic = null;

        #region Singleton
        private static Joystick _Instance = null;

        public static Joystick GetInstance()
        {
            if (_Instance == null) _Instance = new Joystick();
            return _Instance;
        }

        private Joystick() : base() { }
        #endregion
        private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;
        }
        void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            _Center = background.transform.position;
            ResetHandlePose();
            _JoystickSize = new Vector2(background.sprite.texture.width, background.sprite.texture.height) * transform.localScale;
            UpdateJoysitckColor();
            _Player = PlayerBis.PlayerController.GetInstance();


            if (Cinematic.GetInstance() != null)
            {
                _Cinematic = Cinematic.GetInstance();
                /*_Cinematic.onStartCinematic += OnStartCinematic;
                _Cinematic.onEndCinematic += OnEndCinematic;*/
            }
#endif
        }

        void Update()
        {
            _Center = background.transform.position;
            if (_HandlePose.y != 0)
            {
                if (_HandlePose.y > 0) PlayerBis.PlayerController.GetInstance().verticalDir = _HandlePose.y;
                else PlayerBis.PlayerController.GetInstance().verticalDir = 0f;
            }
            if (isBtnPressed)
            {
                _HandlePose = NormalizedHandlePosition();
                updateMovement.Invoke(_HandlePose.x);

                if (_HandlePose.y <= deadZoneDown) _Player.isDown = true;
                else _Player.isDown = false;
            }
            else updateMovement.Invoke(0f);
        }
        public void ResetHandlePose()
        {
            _Handle.transform.position = _Center;
        }

        /// <summary>
        /// ReturnNormalizedHandlePosition : Return the position of the handle between -1 and 1.
        /// </summary>
        public Vector2 NormalizedHandlePosition()
        {
            _Touch = Input.GetTouch(0);
            _TouchPosition = _Touch.position;
            _Handle.transform.position = new Vector2(Mathf.Clamp(_TouchPosition.x, _Center.x - _JoystickSize.x, _Center.x + _JoystickSize.x),
                                                     Mathf.Clamp(_TouchPosition.y, _Center.y - _JoystickSize.y, _Center.y + _JoystickSize.y));
            
            return ((Vector2)_Handle.transform.position - _Center).normalized; 
        }

        public bool CheckDeadZone(float pAxe) => pAxe > deadZone || pAxe < -deadZone;


        public void OnStartCinematic() => gameObject.SetActive(false);

        public void OnEndCinematic() => gameObject.SetActive(true);

        public void UpdateJoysitckColor()
        {
            if (!isShow)
            {
                background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);
                handleImage.color = new Color(handleImage.color.r, handleImage.color.g, handleImage.color.b, 0f);
            }
        }
        private void OnDestroy()
        {
            if (_Cinematic != null)
            {
                /*_Cinematic.onStartCinematic -= OnStartCinematic;
                _Cinematic.onEndCinematic -= OnEndCinematic;*/
            }

            if (_Instance != null)
                _Instance = null;
        }
    }
}