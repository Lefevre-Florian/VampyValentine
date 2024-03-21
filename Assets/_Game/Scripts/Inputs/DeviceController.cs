using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;

// Author : Lefevre Florian
namespace Com.IsartDigital.Platformer
{
    public class DeviceController : MonoBehaviour
    {
        #region Singleton
        private static DeviceController _Instance = null;

        public static DeviceController GetInstance()
        {
            if(_Instance == null) _Instance = new DeviceController();
            return _Instance;
        }

        private DeviceController() : base() {}
        #endregion

        [SerializeField] private InputAction _Motion = null;

        public enum DeviceSignature
        {
            DEFAULT = 0,
            XBOX = 1,
            PLAYSTATION = 2,
            KEYBOARD = 3
        }

        private DeviceSignature _Current = DeviceSignature.DEFAULT;

        private IDisposable _EventListenner = null;

        public DeviceSignature CurrentSignature { get { return _Current; } }

        // Events
        public event Action OnGamepadConnected;
        public event Action OnGamepadDisconnected;

        public event Action<DeviceSignature> OnDeviceSignatureUpdated;

        public event Action<bool> OnFocusChanged;

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
            #if UNITY_ANDROID || UNITY_IOS
            Destroy(gameObject);
            #endif

            Gamepad lGamepad = Gamepad.current;
            if(lGamepad == null)
            {
                _Current = DeviceSignature.KEYBOARD;
            }
            else
            {
                _Current = GetDeviceSignature(lGamepad.device);
                OnGamepadConnected?.Invoke();
            }

            // Signals
            InputSystem.onDeviceChange += GamepadStateAnalysis;
            _Motion.started += HandleInputMotion;

            _EventListenner = InputSystem.onAnyButtonPress.Call(HandleLastInputActionType);
        }

        private void OnEnable() => _Motion.Enable();
        private void OnDisable() => _Motion.Disable();

        private void HandleInputMotion(InputAction.CallbackContext pContext) => HandleLastInputActionType(pContext.control);

        private void HandleLastInputActionType(InputControl pAction)
        {
            DeviceSignature lSignature = GetDeviceSignature(pAction.device);
            if (lSignature != _Current)
            {
                _Current = lSignature;
                OnFocusChanged?.Invoke(!(lSignature == DeviceSignature.KEYBOARD || lSignature == DeviceSignature.DEFAULT));
            }
        }

        private void GamepadStateAnalysis(InputDevice pDevice, InputDeviceChange pState)
        {
            switch (pState)
            {
                case InputDeviceChange.Added:
                    OnGamepadConnected?.Invoke();
                    OnDeviceSignatureUpdated?.Invoke(GetDeviceSignature(pDevice));
                    return;
                case InputDeviceChange.Removed:
                    OnGamepadDisconnected?.Invoke();
                    OnDeviceSignatureUpdated?.Invoke(DeviceSignature.KEYBOARD);
                    return;
                case InputDeviceChange.Disconnected:
                    OnGamepadDisconnected?.Invoke();
                    OnDeviceSignatureUpdated?.Invoke(DeviceSignature.KEYBOARD);
                    return;
                case InputDeviceChange.Reconnected:
                    OnGamepadConnected?.Invoke();
                    OnDeviceSignatureUpdated?.Invoke(GetDeviceSignature(pDevice));
                    return;
                default:
                    break;
            }
        }
        
        private DeviceSignature GetDeviceSignature(InputDevice pDevice)
        {
            DeviceSignature lSignature;
            if (pDevice is Keyboard or Mouse)
                lSignature = DeviceSignature.KEYBOARD;
            else if (pDevice is DualShockGamepad)
                lSignature = DeviceSignature.PLAYSTATION;
            else if (pDevice is XInputController)
                lSignature = DeviceSignature.XBOX;
            else
                lSignature = DeviceSignature.KEYBOARD;

            return lSignature;
        }

        public bool GetGamepadStatus() => (Gamepad.current != null);

        private void OnDestroy()
        {
            // Security
            _EventListenner.Dispose();
            _EventListenner = null;

            _Motion.started -= HandleInputMotion;
            InputSystem.onDeviceChange -= GamepadStateAnalysis;

            if (_Instance != null)
                _Instance = null;
        }

    }
}
