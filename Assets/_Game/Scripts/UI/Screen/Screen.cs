using Com.IsartDigital.Platformer.SoundManager;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

// Author : Lefevre Florian
namespace Com.IsartDigital.Platformer.UI
{
    public class Screen : MonoBehaviour
    {
        [Header("Support")]
        [SerializeField] private GameObject _MobileLayout = null;
        [SerializeField] private GameObject _PCLayout = null;

        [Header("Export Settings")]
        [SerializeField] protected GameObject m_FirstSelectedButton = null;
        [SerializeField] protected Animator transitionAnimator = null;
        protected ScriptSFX _BatSFX => GetComponent<ScriptSFX>();


        [SerializeField] private bool useTransition = false;
        
        private GameObject _CurrentSelectedButton = null;
        
        private EventSystem _EventSystem = null;

        #if UNITY_STANDALONE
        private DeviceController _DeviceManager = null;
        #endif

        protected virtual void Start()
        {
            if (_MobileLayout != null && _PCLayout != null)
            {
                #if UNITY_STANDALONE
                _MobileLayout.SetActive(false);
                _PCLayout.SetActive(true);
#elif UNITY_ANDROID || UNITY_IOS

                if (_MobileLayout != null)
                {
                    _MobileLayout.SetActive(true);
                    _PCLayout.SetActive(false);
                }
                else
                {
                    _MobileLayout.SetActive(false);
                    _PCLayout.SetActive(true);
                }

#endif
            }

            _EventSystem = EventSystem.current;
            _CurrentSelectedButton = m_FirstSelectedButton;

            #if UNITY_STANDALONE
            GamepadExtension();
            #endif

            OnEnable();
        }
        
        #if UNITY_STANDALONE
        protected virtual void GamepadExtension()
        {
            _DeviceManager = DeviceController.GetInstance();
            _DeviceManager.OnGamepadConnected += ManageGamepad;
            _DeviceManager.OnFocusChanged += ManageFocus;
        }
        #endif

        protected virtual void OnEnable()
        {
            #if UNITY_STANDALONE
            _CurrentSelectedButton = m_FirstSelectedButton;

            if (_DeviceManager == null)
                return;

            if(_DeviceManager.GetGamepadStatus())
                ManageGamepad();
            #endif
        }

        protected virtual void OnDisable()
        {
            #if UNITY_STANDALONE
            _CurrentSelectedButton = m_FirstSelectedButton;
            #endif
        }

        public virtual void OpenScreen() => gameObject.SetActive(true);

        public virtual void CloseScreen() => gameObject.SetActive(false);

        public virtual void Quit()
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        IEnumerator TransitionCoroutine(Screen pScreen)
        {
            yield return new WaitForSecondsRealtime(1);
            CloseScreen();
            pScreen.OpenScreen();
        }

        public virtual void TargetScreen(Screen pScreen)
        {
            if (useTransition)
            {
                _BatSFX.PlaySFX();
                transitionAnimator.Play("Bat_Transition");
            }

            StartCoroutine(TransitionCoroutine(pScreen));
        }

        public virtual void TargetScreen(GameObject pScreen)
        {
            CloseScreen();
            pScreen.SetActive(true);
        }

        #if UNITY_STANDALONE
        /// <summary>
        /// If focus state is True priority to controller either way priority to the keyboard
        /// </summary>
        /// <param name="pFocusState"></param>
        private void ManageFocus(bool pFocusState)
        {
            if (!pFocusState)
                _CurrentSelectedButton = EventSystem.current.currentSelectedGameObject;
            else
                _EventSystem.SetSelectedGameObject(_CurrentSelectedButton);
        }

        private void ManageGamepad()
        {
            if (_EventSystem != null)
                _EventSystem.SetSelectedGameObject(_CurrentSelectedButton);
        }
        #endif

        protected virtual void OnDestroy()
        {
            #if UNITY_STANDALONE
            if(_DeviceManager != null)
            {
                _DeviceManager.OnGamepadConnected -= ManageGamepad;
                _DeviceManager.OnFocusChanged -= ManageFocus;
                _DeviceManager = null;
            }
            #endif
        }
    }
}
