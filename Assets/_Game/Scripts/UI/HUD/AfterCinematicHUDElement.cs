using Com.IsartDigital.Platformer.Game;
using Com.IsartDigital.Platformer.SoundManager;
using System;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer.UI.Gameplay
{
    public class AfterCinematicHUDElement : MonoBehaviour
    {
        private Animator _Animator => GetComponent<Animator>();
        private const string ON_END_CINEMATIC_TRIGGER = "OnEndCinematic";
        [SerializeField] private bool _OnStart = false;
        [SerializeField] private bool _IsHUD = false;
        [SerializeField] private ScriptSFX _SFXOnSpawn = null;

        public void SetTriggerOnCollect()
        {
            _SFXOnSpawn?.PlaySFX();
            _Animator.SetTrigger(ON_END_CINEMATIC_TRIGGER);
        }

        private void Start()
        {
            if (_OnStart && _IsHUD)
            {
                SetTriggerOnCollect();
            }
            else
            {
                Cinematic.GetInstance().onEndCinematic += SetTriggerOnCollect;
            }
        }
        private void OnDestroy()
        {
            if (!_OnStart)
            {
                Cinematic.GetInstance().onEndCinematic -= SetTriggerOnCollect;
            }
        }
    }
}