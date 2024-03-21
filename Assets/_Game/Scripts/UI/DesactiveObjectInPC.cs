using UnityEngine;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.UI
{
    public class DesactiveObjectInPC : MonoBehaviour
    { 
        private void Start()
        {
            #if UNITY_STANDALONE
            Destroy(gameObject);
            #endif
        }
    }
}