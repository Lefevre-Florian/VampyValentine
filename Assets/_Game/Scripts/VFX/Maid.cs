using Com.IsartDigital.Platformer.SoundManager;
using FMODUnity;
using System;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer.Game.VFX
{
    public class Maid : MonoBehaviour
    {
        [SerializeField] private ScriptSFX _SoundMaid;
        [SerializeField] private ParamRef _ParamAmbiance;
        private bool _IsPlaying = false;
        private void Update()
        {
            if (!_IsPlaying)
            {
                _IsPlaying = true;
                _SoundMaid.PlayLongSFX(_ParamAmbiance);
            }
        }
        private void OnDisable()
        {
            _IsPlaying = false;
            _SoundMaid.StopLongSFX();
        }

    }
}
