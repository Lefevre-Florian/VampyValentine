using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Platformer.SoundManager
{
    public class PlayerSFX : MonoBehaviour
    {
        public PlayerEventInstance footstepStone = null;
        public PlayerEventInstance footstepWood = null;
        public PlayerEventInstance footstepLantern = null;
        public PlayerEventInstance footstepLustre = null;
        public PlayerEventInstance footstep = null;
        public PlayerEventInstance dash = null;
        public PlayerEventInstance jump = null;
        /*
        public PlayerEventInstance landLustre = null;
        public PlayerEventInstance landLantern = null;
        public PlayerEventInstance landWood = null;
        public PlayerEventInstance landStone = null;*/
        public PlayerEventInstance land = null;
        public PlayerEventInstance glideActivate = null;
        public PlayerEventInstance glideLoop = null;
        public PlayerEventInstance dieVoid = null;
        public PlayerEventInstance dieGarlic = null;
        public PlayerEventInstance dieMaid = null;
        public PlayerEventInstance dieBurn = null;
        public PlayerEventInstance burnLoop = null;
        
        public void PlayOnShotSFX(PlayerEventInstance pInstance)
        {
            RuntimeManager.PlayOneShot(pInstance.sfxPath);
        }
        public void PlaySXF(PlayerEventInstance pInstance)
        {
            if (!pInstance.isInstanciate)
            {
                pInstance.eventInstance = RuntimeManager.CreateInstance(pInstance.sfxPath);
                pInstance.eventInstance.start();
                pInstance.isInstanciate = true;
            }
        }
        public void PlaySXF(PlayerEventInstance pInstance, ParamRef pParam)
        {
            if (!pInstance.isInstanciate)
            {
                pInstance.eventInstance = RuntimeManager.CreateInstance(pInstance.sfxPath);
                /*pInstance.eventInstance.start();
                pInstance.eventInstance.setParameterByName(pParam.Name, pParam.Value);
                pInstance.isInstanciate = true;*/
                pInstance.isInstanciate = true;
            }
            pInstance.eventInstance.setParameterByName(pParam.Name, pParam.Value);
            pInstance.eventInstance.start();
        }
        public void StopSFX(PlayerEventInstance pInstance, FMOD.Studio.STOP_MODE pStopMode = FMOD.Studio.STOP_MODE.IMMEDIATE)
        {
            pInstance.isInstanciate = false;
            pInstance.eventInstance.stop(pStopMode);
            pInstance = null;
        }
    }
}
