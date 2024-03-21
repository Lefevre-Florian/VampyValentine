using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Author : Matteo Renaudin
namespace Com.IsartDigital.Platformer.UI.Settings
{
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _TextValue = default;
        [SerializeField] private Slider _Slider = default;
        [SerializeField] private SoundManager.SoundManager.BusType _BusType = default;

        private void Start()
        {
            UpdateText();
            _Slider.onValueChanged.AddListener(delegate { UpdateText(); });
            if (_BusType == SoundManager.SoundManager.BusType.Master)
            {
                _Slider.onValueChanged.AddListener(delegate { SoundManager.SoundManager.GetInstance().UpdateMasterBus((float)Math.Round(_Slider.value * 2, 2)); });
            }
            else
            {
                _Slider.onValueChanged.AddListener(delegate { SoundManager.SoundManager.GetInstance().UpdateVCA(_BusType, (float)Math.Round(_Slider.value * 2, 2)); });
            }
            
        }
        private void UpdateText() => _TextValue.text = (Math.Round(_Slider.value, 2) * 100).ToString() ;

        private void OnDestroy()
        {
            _Slider.onValueChanged.RemoveListener(delegate { UpdateText(); });

            if (_BusType == SoundManager.SoundManager.BusType.Master)
            {
                _Slider.onValueChanged.RemoveListener(delegate { SoundManager.SoundManager.GetInstance().UpdateMasterBus((float)Math.Round(_Slider.value * 2, 2)); });
            }
            else
            {
                _Slider.onValueChanged.RemoveListener(delegate { SoundManager.SoundManager.GetInstance().UpdateVCA(_BusType, (float)Math.Round(_Slider.value * 2, 2)); });
            }
        }
    }
}
