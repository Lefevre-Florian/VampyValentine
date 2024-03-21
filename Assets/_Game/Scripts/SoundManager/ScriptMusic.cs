using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.Platformer
{
    public class TestMusic : MonoBehaviour
    {
        [SerializeField] EventReference _MusicPath = default;
        bool _isPlaying = false;
        [SerializeField] bool _IsAmbiant = false;

        private void Update()
        {
            if (!_isPlaying)
            {
                _isPlaying = true;
                if (!_IsAmbiant)
                    SoundManager.SoundManager.GetInstance().SetMusic(_MusicPath);
                else
                    SoundManager.SoundManager.GetInstance().SetAmbiantSound(_MusicPath);
            }
        }
    }
}
