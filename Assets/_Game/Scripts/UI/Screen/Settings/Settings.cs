using UnityEngine;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.UI.Settings
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] protected GameObject _Mobile = default;
        [SerializeField] protected GameObject _PC = default;
  
        protected virtual void Start()
        {
            #if UNITY_STANDALONE
            _PC.SetActive(true);
            _Mobile.SetActive(false);
            #elif UNITY_ANDROID || UNITY_IOS
            _PC.SetActive(false);
            _Mobile.SetActive(true);
            #endif
        }

    }
}