using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

// Author : Lefevre Florian
namespace Com.IsartDigital.Platformer.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        #region Singleton
        private static LocalizationManager _Instance = null;

        public static LocalizationManager GetInstance()
        {
            if(_Instance == null) _Instance = new LocalizationManager();
            return _Instance;
        }

        private LocalizationManager() : base() { }
        #endregion

        private static int _SessionLanguage = (int)Languages.EN;

        private const char CSV_SEPARATOR = ';';
        private const char LINE_SEPARATOR = '\n';

        [SerializeField] private TextAsset _LocalizationFile = null;

        // Variables
        private Dictionary<string, string[]> _Translations = null;

        public Dictionary<string, string[]> Translations
        {
            get { return _Translations; }
            private set { _Translations = value; }
        }

        public Languages CurrentLanguage { get { return (Languages)_SessionLanguage; } private set {; } }

        // Events
        public event Action OnTranslationChanged;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;

            if (_LocalizationFile != null)
            {
                // Initializing the main translations dictionary
                _Translations = new Dictionary<string, string[]>();
                int lNBLanguages = Enum.GetNames(typeof(Languages)).Length;

                // Filling the dictionary of words by languages (key -> words)
                string[] lContent = _LocalizationFile.text.Split(LINE_SEPARATOR);

                string[] lCSVLine = null;
                string[] lWords = null;

                int lLength = lContent.Length;

                if (lLength <= 1)
                {
                    _Translations = null;
                    return;
                }

                for (int i = 1; i < lLength - 1; i++)
                {
                    lCSVLine = lContent[i].Split(CSV_SEPARATOR);
                    lWords = new string[lNBLanguages];

                    for (int j = 0; j < lNBLanguages; j++)
                        lWords[j] = lCSVLine[j + 1];
                    _Translations.Add(lCSVLine[0], lWords);
                }
            }
        }

        public void UpdateLocalization(int pLanguageID) => UpdateLocalization((Languages)pLanguageID);

        public void UpdateLocalization(Languages pLanguage)
        {
            if (_Translations == null)
                return;

            _SessionLanguage = (int)pLanguage;

            OnTranslationChanged?.Invoke();
        }

        public string GetTranslation(string pKey)
        {
            if (_Translations == null || !_Translations.ContainsKey(pKey))
                return "";
            else
                return _Translations[pKey][_SessionLanguage];
        }

        private void OnDestroy()
        {
            if(_Translations != null)
            {
                _Translations.Clear();
                _Translations = null;
            }

            if (_Instance != null)
                _Instance = null;
        }

    }
}
