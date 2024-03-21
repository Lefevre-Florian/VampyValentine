using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Platformer
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Tuto : DeviceDisplay
    {
        [Header("Mobile")]
        [SerializeField] private Sprite _MobileDisplay = null;

        private SpriteRenderer _Renderer = null;

        protected override void Start()
        {
            _Renderer = GetComponent<SpriteRenderer>();
            base.Start();
            #if UNITY_ANDROID || UNITY_IOS
            _Renderer.sprite = _MobileDisplay;
            #endif
        }

        #if UNITY_STANDALONE
        protected override void OnDeviceUpdated(DeviceController.DeviceSignature pUpdatedSignature) => _Renderer.sprite = m_DeviceLibrary[pUpdatedSignature];

        protected override void OnDeviceFocusUpdated(bool pState) => _Renderer.sprite = m_DeviceLibrary[m_DeviceManager.CurrentSignature];
        #endif

    }
}
