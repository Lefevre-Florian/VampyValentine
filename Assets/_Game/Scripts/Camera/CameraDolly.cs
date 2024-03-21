using Com.IsartDigital.Platformer.InGameElement.Killzone;
using Com.IsartDigital.Platformer.Utils;
using System;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer
{
    public class CameraDolly : MonoBehaviour, IResetableObject
    {
        [SerializeField] private string _StateName = "";

        [SerializeField] private GameObject _AnimCam;
        private Animator _Animator => GetComponent<Animator>();

        public Vector3 initialPosition { get; private set; } = Vector3.zero;

        private void Start()
        {
            initialPosition = transform.position;

            Killzone.resetLevel.AddListener(ResetObject);
        }

        public void ResetObject()
        {
            if (_AnimCam.active)
            {
                
                _Animator.enabled = true;
                _Animator.Play(_StateName, 0, 0f);

            }

            else
            {
                _Animator.Play("void", 0, 0f);
            }

        }

        private void OnDestroy() => Killzone.resetLevel.RemoveListener(ResetObject);
    }
}
