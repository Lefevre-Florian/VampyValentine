using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Author : 
namespace Com.IsartDigital.Platformer.UI
{
    public class LoadingScreen : ScreenCallToAction
    {

        [Header("UI")]
        [SerializeField] private GameObject _InputTextField = null;

        private bool _IsInteractable = false;

        private LoadScene _LoadManager = null;

        protected override void Start()
        {
            base.Start();

            _LoadManager = LoadScene.GetInstance();
            _LoadManager.OnLoadPending += SetInteraction;

            _InputTextField.SetActive(false);
        }

        private void SetInteraction()
        {
            _InputTextField.SetActive(true);

            _IsInteractable = true;
        }

        protected override void Execute(InputAction.CallbackContext pContext)
        {
            if (!_IsInteractable)
                return;

            _IsInteractable = false;
            _LoadManager.CompleteLoading();

        }

        protected override void OnDestroy()
        {
            _LoadManager.OnLoadPending -= SetInteraction;
            _LoadManager = null;
            base.OnDestroy();
        }
    }
}
