using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Author : 
namespace Com.IsartDigital.Platformer.UI
{
    public class ScreenCallToAction : Screen
    {
        [SerializeField] private GameObject _TargetScreen = null;

        [Header("Input")]
        [SerializeField] private InputAction _Input;

        protected override void Start()
        {
            base.Start();

            _Input.performed += Execute;
        }

        protected override void OnEnable() => _Input.Enable();

        protected override void OnDisable() => _Input.Disable();

        protected virtual void Execute(InputAction.CallbackContext pContext)
        {
            CloseScreen();
            TargetScreen(_TargetScreen);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _Input.performed -= Execute;
            _Input = null;
        }
    }
}
