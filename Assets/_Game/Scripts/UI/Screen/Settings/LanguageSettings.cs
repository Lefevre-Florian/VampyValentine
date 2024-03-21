using System;
using UnityEngine;
using UnityEngine.UI;
using Com.IsartDigital.Platformer.Localization;

// Author : 
namespace Com.IsartDigital.Platformer.UI.Settings
{
    public class LanguageSettings : MonoBehaviour
    {

        [SerializeField] private Sprite _EN_Image = default;
        [SerializeField] private Sprite _FR_Image = default;
        [SerializeField] private Toggle _ToggleLanguage = default;

        private LocalizationManager _LocalizationManager = null;
        #region Singleton
        private static LanguageSettings _Instance = null;

        public static LanguageSettings GetInstance()
        {
            if(_Instance == null) _Instance = new LanguageSettings();
            return _Instance;
        }

        private LanguageSettings() : base() {}
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
            _ToggleLanguage?.onValueChanged.AddListener(delegate { OnValueChanged(); });

            if (LocalizationManager.GetInstance() != null)
                _LocalizationManager = LocalizationManager.GetInstance();

            if (_ToggleLanguage.isOn)
                _ToggleLanguage.GetComponent<Image>().sprite = _EN_Image;
            else
                _ToggleLanguage.GetComponent<Image>().sprite = _FR_Image;

        }

        private void OnValueChanged()
        {
            if (_ToggleLanguage == null) return;

            if (_ToggleLanguage.isOn)
            {
                _LocalizationManager.UpdateLocalization(1);
                _ToggleLanguage.GetComponent<Image>().sprite = _EN_Image;
            }
            else
            {
                _LocalizationManager.UpdateLocalization(0);
                _ToggleLanguage.GetComponent<Image>().sprite = _FR_Image;
            }
        }

        private void OnDestroy()
        {
            _ToggleLanguage?.onValueChanged.RemoveListener(delegate { OnValueChanged(); });
            if (_Instance != null)
                _Instance = null;
        }

    }
}
