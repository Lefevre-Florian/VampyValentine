using FMOD.Studio;
using FMODUnity;
using System;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.SoundManager
{
    [Serializable]
    public class Bank
    {
        [NonSerialized] public int bankID = 0;
        [BankRef] public string bankPath = default;
        public bool isLevelBank = false;
    }
    [Serializable]
    public class PlayerEventInstance
    {
        public EventInstance eventInstance;
        public EventReference sfxPath = default;
        public bool isInstanciate = false;
    }
}