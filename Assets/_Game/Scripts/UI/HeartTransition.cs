using Com.IsartDigital.Platformer.InGameElement.Killzone;
using Com.IsartDigital.Platformer.PlayerBis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Platformer
{
    public class HeartTransition : MonoBehaviour
    {


         private Animator _Animator => GetComponent<Animator>();

        [SerializeField] private AnimationClip _HeartInAnim;


        void Start()
        {
            Killzone.resetLevel.AddListener(OnPlayerDeath);
        }

        private void OnPlayerDeath()
        {
            _Animator.Play(_HeartInAnim.name);
        }


        private void ReverseAnim()
        {

        }

        void Update()
        {
        
        }

        private void OnDestroy()
        {
            Killzone.resetLevel.RemoveListener(OnPlayerDeath);
        }
    }
}
