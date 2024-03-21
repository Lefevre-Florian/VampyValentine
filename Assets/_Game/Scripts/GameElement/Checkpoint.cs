using Com.IsartDigital.Platformer.SoundManager;
using Com.IsartDigital.Platformer.UI.Gameplay;
using UnityEngine;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.InGameElement.Checkpoint
{
    public class Checkpoint : GameElement
    {
        public GameObject _CameraOnCheckpoint = null;

        private BoxCollider2D _Collider = null;

        [Header("Animation")]
        [SerializeField] private GameObject _FireAnim;

        [Header("Game design")]
        [SerializeField] private int _LevelDuration = default;
        private Animator _Animator => _FireAnim.GetComponent<Animator>();
        private GameManager _GameManager => GameManager.GetInstance();

        protected override void Start()
        {
            base.Start();
            _Collider = GetComponent<BoxCollider2D>();
        }

        public override void Effect(PlayerBis.PlayerController pPlayer)
        {
            pPlayer.OnCheckpoint(this);
            _GameManager.lastCheckpoint = this;
            _Animator.SetTrigger("TriggerStart");
            VariablesToSave.lastCheckpointBeforeLeave = this;

            if (_Collider != null) _Collider.enabled = false;
            GetComponent<ScriptSFX>().PlaySFX();

            if (TimerHUD.GetInstance().coroutine != null) 
                StopCoroutine(TimerHUD.GetInstance().coroutine);
            GameManager.GetInstance().totalTime += TimerHUD.GetInstance().currentTimer;

            TimerHUD.GetInstance().coroutine = StartCoroutine(TimerHUD.GetInstance().Timer());
            LevelEvolution.GetInstance().UpdateProgression(_LevelDuration);

        }

    }
}
