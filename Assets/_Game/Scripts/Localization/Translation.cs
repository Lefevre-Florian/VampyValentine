using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// Author : Lefevre Florian & Bastien Chevallier
namespace Com.IsartDigital.Platformer.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Translation : MonoBehaviour
    {
        // Variables
        private TextMeshProUGUI _TextField = null;

        private string _TranslationKey = "";

        // References
        private LocalizationManager _LocalizationManager = null;

        private void Start()
        {
            UpdateLoca();
        }

        public void UpdateLoca()
        {
            _LocalizationManager = LocalizationManager.GetInstance();
            if (_LocalizationManager == null)
                return;

            _TextField = GetComponent<TextMeshProUGUI>();
            _TranslationKey = _TextField.text.ToUpper().Trim();

            UpdateTextField();
            _LocalizationManager.OnTranslationChanged += UpdateTextField;
        }

        private void UpdateTextField() => _TextField.text = _LocalizationManager.GetTranslation(_TranslationKey);

        private void OnEnable()
        {
            if(_LocalizationManager != null)
                UpdateTextField();
        }

        private void OnDestroy()
        {
            if(_LocalizationManager != null)
            {
                _LocalizationManager.OnTranslationChanged -= UpdateTextField;
                _LocalizationManager = null;
            }
        }

    }
}
