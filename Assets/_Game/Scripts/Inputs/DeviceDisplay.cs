using System;
using System.Collections.Generic;

using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Platformer
{
    public abstract class DeviceDisplay : MonoBehaviour
    {
        [Serializable]
        private struct SerializedInputDevice
        {
            public DeviceController.DeviceSignature deviceSignature;
            public Sprite displaySprite;
        }

        [Header("PC")]
        [SerializeField] private SerializedInputDevice[] _DeviceDisplays = null;

        // Variables
        protected Dictionary<DeviceController.DeviceSignature, Sprite> m_DeviceLibrary = null;
        protected DeviceController m_DeviceManager = null;

        protected virtual void Start()
        {
            #if UNITY_STANDALONE
            int lLength = _DeviceDisplays.Length;
            if (lLength == 0)
                return;

            m_DeviceLibrary = new Dictionary<DeviceController.DeviceSignature, Sprite>();
            for (int i = 0; i < lLength; i++)
                m_DeviceLibrary.Add(_DeviceDisplays[i].deviceSignature, _DeviceDisplays[i].displaySprite);

            _DeviceDisplays = null;
            m_DeviceManager = DeviceController.GetInstance();
            m_DeviceManager.OnDeviceSignatureUpdated += OnDeviceUpdated;
            m_DeviceManager.OnFocusChanged += OnDeviceFocusUpdated;

            OnDeviceUpdated(m_DeviceManager.CurrentSignature);
            #endif
        }

        #if UNITY_STANDALONE
        protected virtual void OnDeviceUpdated(DeviceController.DeviceSignature pUpdatedSignature) { }

        protected virtual void OnDeviceFocusUpdated(bool pState) { }
        #endif

        private void OnDestroy()
        {
            #if UNITY_STANDALONE
            m_DeviceManager.OnDeviceSignatureUpdated -= OnDeviceUpdated;
            m_DeviceManager.OnFocusChanged -= OnDeviceFocusUpdated;
            #endif
            m_DeviceManager = null;

            m_DeviceLibrary?.Clear();
            m_DeviceLibrary = null;
        }

    }
}
