using Com.IsartDigital.Platformer.PlayerBis;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Author : Bastien Chevallier
namespace Com.IsartDigital.Platformer.Game
{
    public class MouseDetection : MonoBehaviour
    {
        #region Singleton
        private static MouseDetection _Instance = null;

        public static MouseDetection GetInstance()
        {
            if(_Instance == null) _Instance = new MouseDetection();
            return _Instance;
        }

        private MouseDetection() : base() {}
        #endregion

        public GameObject currentLevel = null;
        public MobilePlatform _CurrentPlatform;

        public int currentIndex = 0;
        private float _PlatformSpeed;
        private InputManager _InputManager;

        private Action _DoAction = null;


        [SerializeField] private Sprite _NormalPlatform;
        [SerializeField] private Sprite _SelectedPlatform;
        [SerializeField] private Sprite _NextPlatform;



        private PlayerAnim _PlayerAnim =>PlayerAnim.GetInstance();
        private PlayerBis.PlayerController _PlayerController => PlayerController.GetInstance();

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }

            _Instance = this;
        }

        #if UNITY_STANDALONE
        private void Start()
        {
            SetupPlatform();
            _InputManager = new InputManager();
            _InputManager.Main.Switch.started += SwitchPlatform;

            _InputManager.Main.PlatformLR.started += SetMovePlatform;
            _InputManager.Main.PlatformLR.performed += SetMovePlatform;
            _InputManager.Main.PlatformLR.canceled += SetMovePlatform;

            _InputManager.Main.PlatformUD.started += SetMovePlatform;
            _InputManager.Main.PlatformUD.performed += SetMovePlatform;
            _InputManager.Main.PlatformUD.canceled += SetMovePlatform;

            _InputManager.Main.Enable();
        }
#endif

        private void Update()
        {
#if UNITY_STANDALONE
            if (_DoAction != null)
            {
                _DoAction();
            }

            if (_CurrentPlatform != null)
            {
                _CurrentPlatform.Replace(_CurrentPlatform.points[0], _CurrentPlatform.points[1]);
            }
#endif

#if UNITY_ANDROID
            if (_CurrentPlatform != null && Input.GetMouseButton(0))
            {
                _CurrentPlatform.Replace(_CurrentPlatform.points[0], _CurrentPlatform.points[1]);
            }
            else if (Input.GetMouseButtonUp(0)) 
            {
                _CurrentPlatform = null;
            }
#endif
        }

        private void SwitchPlatform(InputAction.CallbackContext pContext)
        {
            currentIndex++;
            if(currentIndex >= currentLevel.transform.childCount) currentIndex = 0;
            SetupPlatform();
        }

        private void SetMovePlatform(InputAction.CallbackContext pContext)
        {
            if(_CurrentPlatform != null)
            {
                _PlatformSpeed = pContext.ReadValue<float>();
                if (_PlatformSpeed != 0)
                {
                    _CurrentPlatform.soundOnMove.PlayLongSFX();
                    _CurrentPlatform.ratio += 1f * Mathf.Sign(_PlatformSpeed * Time.deltaTime) * (_CurrentPlatform.movementSpeed * Time.deltaTime);
                    _DoAction = MovePlatformAction;
                }else
                {
                    _DoAction = DoActionVoid;
                    _CurrentPlatform.soundOnMove.StopLongSFX();
                }
            }
        }

        private void DoActionVoid()
        {
            _PlayerController.isControlling = false;

        }

        private void MovePlatformAction()
        {
            if(_CurrentPlatform != null)
            {
                _CurrentPlatform.ratio += _PlatformSpeed * _CurrentPlatform.movementSpeed;

                if (_PlayerController.rb.velocity == Vector2.zero)
                {
                    _PlayerController.isControlling = true;
                    _PlayerAnim.PlayControlAnim();

                }
            }
        }

        public void SetupPlatform()
        {
            if(currentLevel != null)
            {
                int lPlatformChildCount = currentLevel.transform.childCount;
                int lNextPlatIndex = currentIndex + 1;
                if(lNextPlatIndex == lPlatformChildCount) lNextPlatIndex = 0;

                for (int i = 0; i < lPlatformChildCount; i++)
                {
                    if (i == currentIndex)
                    {
                        currentLevel.transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().sprite = _SelectedPlatform;
                        _CurrentPlatform = currentLevel.transform.GetChild(i).GetChild(0).GetComponent<MobilePlatform>();
                    }
                    else if (i == lNextPlatIndex) currentLevel.transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().sprite = _NextPlatform;
                    else currentLevel.transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().sprite = _NormalPlatform;
                }
            }
        }

        private void OnDestroy()
        {
#if UNITY_STANDALONE
            _InputManager.Main.Switch.started -= SwitchPlatform;

            _InputManager.Main.PlatformLR.started -= SetMovePlatform;
            _InputManager.Main.PlatformLR.performed -= SetMovePlatform;
            _InputManager.Main.PlatformLR.canceled -= SetMovePlatform;

            _InputManager.Main.PlatformUD.started -= SetMovePlatform;
            _InputManager.Main.PlatformUD.performed -= SetMovePlatform;
            _InputManager.Main.PlatformUD.canceled -= SetMovePlatform;
#endif
            if (_Instance != null)
                _Instance = null;
        }
    }
}