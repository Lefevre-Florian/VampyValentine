using FMOD.Studio;
using FMODUnity;
using UnityEngine;

// Author : Matteo Renaudin
namespace Com.IsartDigital.Platformer.SoundManager
{
    public class ScriptSFX : MonoBehaviour
    {
        [SerializeField] private EventReference _PathSFX = default;

        public EventInstance longSFX;
        public bool stopLong = false;
        public bool isCurrentlyPlay = false;
        public void PlaySFX() { SoundManager.GetInstance().PlaySFX(_PathSFX); }
        public void PlayLongSFX()
        {
            if (isCurrentlyPlay) return;

            isCurrentlyPlay = true;
            longSFX = RuntimeManager.CreateInstance(_PathSFX);
            longSFX.start();
        }
        public void PlayLongSFX(ParamRef pParameter)
        {
            longSFX = RuntimeManager.CreateInstance(_PathSFX);
            longSFX.setParameterByName(pParameter.Name, pParameter.Value);
            longSFX.start();
        }
        public void StopLongSFX()
        {
            isCurrentlyPlay = false;
            longSFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            longSFX = default;
        }
    }
}