using Com.IsartDigital.Platformer.InGameElement.Killzone;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Com.IsartDigital.Platformer.SoundManager;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.UI.Gameplay
{
    [RequireComponent(typeof(Slider))]
    public class LevelEvolution : MonoBehaviour
    {
        private Slider _ProgressBar = null;

        [SerializeField] private float _LevelDuration = 180f;
        private TimerHUD _TimerHUD = null;

        private bool _PlayerDead = false;

        [SerializeField] private ScriptSFX _EndTimer;
        [SerializeField] private ScriptSFX _DieMaid;
        [SerializeField] private ScriptSFX _DieTimer;
        private bool _EndSoundIsAlreadyPlayed = false;
        private Animator _Animator => GetComponent<Animator>();
        private float _TimerToStart => _LevelDuration - (_LevelDuration / 4);

        #region Singleton
        private static LevelEvolution _Instance = null;

        public static LevelEvolution GetInstance()
        {
            if (_Instance == null) _Instance = new LevelEvolution();
            return _Instance;
        }

        private LevelEvolution() : base() { }
        #endregion
        private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;
        }
        void Start()
        {
            _ProgressBar = GetComponent<Slider>();
            _TimerHUD = TimerHUD.GetInstance();
            _ProgressBar.maxValue = _LevelDuration;

            Killzone.resetLevel.AddListener(ResetProgressBar);
        }

        void Update()
        {
            if (GameManager.currentGameState == GameManager.GameState.Pause) return;

            UpdateProgressBar();

            if (_ProgressBar.value >= _TimerToStart && !_EndSoundIsAlreadyPlayed)
            {
                _EndSoundIsAlreadyPlayed = true;
                _EndTimer.PlayLongSFX();
                _Animator?.SetTrigger("NearEnd"); 
            }

            if (_ProgressBar.value == _ProgressBar.maxValue && !_PlayerDead && _ProgressBar.value > 2)
            {
                print(_ProgressBar.maxValue);
                print(_ProgressBar.value);
                _DieMaid.PlaySFX();
                _DieTimer.PlaySFX();
                _PlayerDead = true;
                StartCoroutine(OnDeathCoroutine());
            }
        }

        public void UpdateProgressBar(float pSpeed = 1f) => _ProgressBar.value = _TimerHUD.currentTimer * pSpeed;

        public void UpdateProgression(int pDuration)
        {
            _LevelDuration = pDuration;
            ResetProgressBar();
        }

        public void ResetProgressBar()
        {
            _Animator?.SetTrigger("Reset");
            _EndTimer.StopLongSFX();
            _EndSoundIsAlreadyPlayed = false;
            if (_ProgressBar != null)
            {
                _ProgressBar.value = 0;
                _ProgressBar.maxValue = _LevelDuration;
            }
            if (_TimerHUD != null) _TimerHUD.currentTimer = 0;
        }

        protected IEnumerator OnDeathCoroutine()
        {
            yield return new WaitForSeconds(GameManager.GetInstance().secondOnDeath);
            _PlayerDead = false;
            Killzone.resetLevel.Invoke();
            StopCoroutine(OnDeathCoroutine());
        }

        private void OnDestroy()
        {
            Killzone.resetLevel.RemoveListener(ResetProgressBar);
            if (_Instance != null)
                _Instance = null;
        }
    }
}