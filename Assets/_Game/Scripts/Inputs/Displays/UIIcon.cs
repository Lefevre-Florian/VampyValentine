using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Platformer
{
    [RequireComponent(typeof(Image))]
    public class UIIcon : DeviceDisplay
    {
        private Image _Renderer = null;

        protected override void Start()
        {
            _Renderer = GetComponent<Image>();
            base.Start();
            #if UNITY_ANDROID || UNITY_IOS
            _Renderer.sprite = null;
            Destroy(gameObject);
            #endif
        }

        #if UNITY_STANDALONE
        protected override void OnDeviceUpdated(DeviceController.DeviceSignature pUpdatedSignature) => _Renderer.sprite = m_DeviceLibrary[pUpdatedSignature];

        protected override void OnDeviceFocusUpdated(bool pState) => _Renderer.sprite = m_DeviceLibrary[m_DeviceManager.CurrentSignature];
        #endif

    }
}
