using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Com.IsartDigital.Platformer.Game.VFX
{
    public class DestroyableVFX : MonoBehaviour
    {
        [SerializeField] private AnimationClip _AnimToPlay;
        private Animator _Animator => GetComponent<Animator>();

        private Transform _ParentObject => transform.parent;

        private void Start()
        {
            _Animator.Play(_AnimToPlay.name);
        }

        public void StopAndHideAnim()
        {
            _Animator.StopPlayback();
            _Animator.gameObject.SetActive(false);
        }

        public void OnAnimEnd()
        {
            Destroy(_ParentObject.gameObject);
            
        }

    }
}
