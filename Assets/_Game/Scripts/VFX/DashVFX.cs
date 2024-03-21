using Com.IsartDigital.Platformer.PlayerBis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Platformer.VFX

{
    public class DashVFX : MonoBehaviour
    {
        private Transform _ParentObject => transform.parent;

        private Animator _Animator => GetComponent<Animator>();

        [SerializeField] private AnimationClip _AnimToPlay;

        private PlayerController _PlayerController => PlayerController.GetInstance();


        private void Start()
        {
            _Animator.Play(_AnimToPlay.name);
        }

        public void OnAnimEnd()
        {
            Destroy(_ParentObject.gameObject);

        }


        private void Update()
        {
            if (!_PlayerController.isDashing)
                OnAnimEnd();
        }
    }
}
