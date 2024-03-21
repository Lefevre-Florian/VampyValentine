using FMODUnity;
using System;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer.SoundManager
{
    public class UpdateMusicParam : MonoBehaviour
    {
        [SerializeField] private ParamRef _Param;
        
        public void UpdateMusic() => SoundManager.GetInstance().UpdateMusic(_Param);

    }
}
