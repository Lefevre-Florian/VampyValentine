using System;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer.UI.Gameplay
{
    public class CollectibleHUD : MonoBehaviour
    {
        private Animator _Animator => GetComponent<Animator>();
        private const string ON_COLLECT_TRIGGER = "OnCollect";

        public void SetTriggerOnCollect() => _Animator.SetTrigger(ON_COLLECT_TRIGGER);
    }
}
