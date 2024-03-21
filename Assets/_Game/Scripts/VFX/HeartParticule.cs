using Com.IsartDigital.Platformer.SoundManager;
using FMODUnity;
using System;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer.Game.VFX
{
    public class HeartParticule : MonoBehaviour
    {
        [SerializeField] private ScriptSFX _SoundHeart;
        [SerializeField] private ParamRef _ParamMusic;

        private bool _IsPlaying = false;
        void Start()
        {
        }
        private void Update()
        {
            if (!_IsPlaying)
            {
                _IsPlaying = true;
                _SoundHeart.PlayLongSFX(_ParamMusic);
            }
        }
        private void OnDisable()
        {
            _IsPlaying = false;
            _SoundHeart.StopLongSFX();
        }
    }
}
