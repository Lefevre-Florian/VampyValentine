using Com.IsartDigital.Platformer.SoundManager;
using Com.IsartDigital.Platformer.Utils;

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Author : 
namespace Com.IsartDigital.Platformer.UI.Gameplay
{
    public class BtnSkip : BtnHUD, IDesactivateHUD, IPointerDownHandler, IPointerUpHandler
    {
        private const float DEFAULT_SKIP_DURATION = 0.6f;

        private const string TRANSITION_ANIMATION_NAME = "Bat_Transition";

        [SerializeField] private Animator _CinematicAnimator = default;
        [SerializeField] private string _CinematicAnimationName = default;
        [SerializeField] private Image _Fill = default;
        [SerializeField] private Animator _SkipAnim;
        
        [SerializeField] private float _TimeToPress = 2;
        [SerializeField] private float _SkipDuration = DEFAULT_SKIP_DURATION;

        private float _CounterPress = 0;
        private Coroutine _Coroutine = null;
        private InputManager _InputManager = null;
        private ScriptSFX _BatSFX => GetComponent<ScriptSFX>();

        public event Action OnCinematicSkipped;

        protected override void Start()
        {
            #if UNITY_STANDALONE
            _InputManager = new InputManager();
            _InputManager.UI.Skip.performed += OnInputSkipProgress;
            _InputManager.UI.Skip.canceled += OnInputSkipCancelled;

            _InputManager.Enable();
            #endif
            base.Start();
            HUD.GetInstance().onPause += SetCinematicState;

            _Cinematic.onStartCinematic += OnStartCinematic;
            _Cinematic.onEndCinematic += OnEndCinematic;
        }


        public void OnStartCinematic() => gameObject.SetActive(true);
        public void OnEndCinematic() => gameObject.SetActive(false);


        private void OnInputSkipProgress(InputAction.CallbackContext pContext) => _Coroutine = StartCoroutine(PressCoroutine());
        private void OnInputSkipCancelled(InputAction.CallbackContext pContext) => StopCoroutine();
        public void OnPointerUp(PointerEventData eventData) => StopCoroutine();

        public void OnPointerDown(PointerEventData eventData) => _Coroutine = StartCoroutine(PressCoroutine());


        private void SetCinematicState(bool pIsPause)
        {
            if (pIsPause) _CinematicAnimator.speed = 0;

            else _CinematicAnimator.speed = 1;
        }


        private IEnumerator PressCoroutine()
        {
            while (true)
            {
                _CounterPress += Time.deltaTime / (_TimeToPress / 1.5f);
                _Fill.fillAmount = _CounterPress;

                if (_Fill.fillAmount >= 1)
                {
                    if (_CinematicAnimator != null)
                    {
                        AnimatorStateInfo lStateInfo = _CinematicAnimator.GetCurrentAnimatorStateInfo(0);

                        if (lStateInfo.IsName(_CinematicAnimationName))

                        {
                            _SkipAnim.Play(TRANSITION_ANIMATION_NAME);
                            _BatSFX.PlaySFX();
                            StartCoroutine(SkipCorroutine());
                        }

                    }
                    _InputManager?.Disable();
                    StopCoroutine();
                }
                yield return null;
            }
        }


        
        IEnumerator SkipCorroutine()
        {
            yield return new WaitForSeconds(_SkipDuration);
            _CinematicAnimator.Play(_CinematicAnimationName, 0, 1f);

            OnCinematicSkipped?.Invoke();
        }


        private void StopCoroutine()
        {
            if (_Coroutine != null)
            {
                _Fill.fillAmount = 0f;
                _CounterPress = 0f;
                StopCoroutine(_Coroutine);
                _Coroutine = null;
            }
        }
        protected override void OnDestroy()
        {
            HUD.GetInstance().onPause -= SetCinematicState;
#if UNITY_STANDALONE
            _InputManager.UI.Skip.performed -= OnInputSkipProgress;
            _InputManager.UI.Skip.canceled -= OnInputSkipCancelled;
#endif

            if (_Cinematic != null)
            {
                _Cinematic.onStartCinematic -= OnStartCinematic;
                _Cinematic.onEndCinematic -= OnEndCinematic;
            }
            base.OnDestroy();
        }
    }
}