using Com.IsartDigital.Platformer.InGameElement.Killzone;
using Com.IsartDigital.Platformer.Light;
using Com.IsartDigital.Platformer.PlayerBis;
using Com.IsartDigital.Platformer.SoundManager;
using Com.IsartDigital.Platformer.Utils;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Platformer.InGameElement
{
    public class LightOccluder : GameElement, IResetableObject
    {
        [Header("Physics")]
        [SerializeField] private SpriteLight2D _LightSource = null;

        [Header("Animations")]
        [SerializeField] private string _AnimationTrigger = "";
        [SerializeField] private string _AnimationResetTrigger = "Reset";

        [Header("Rendering")]
        [SerializeField] private Transform[] _Extents = null;

        private bool _IsOpen = true;

        private Animator _Animator = null;
        private BoxCollider2D _Collider2D = null;

        public Vector3 initialPosition => throw new System.NotImplementedException();

        protected override void Start()
        {
            base.Start();

            _Animator = GetComponent<Animator>();
            _Collider2D = GetComponent<BoxCollider2D>();

            Killzone.Killzone.resetLevel.AddListener(ResetObject);
        }

        public override void Effect(PlayerController pPlayer)
        {
            if (pPlayer.isDashing)
            {
                _Animator.ResetTrigger(_AnimationResetTrigger);

                _IsOpen = !_IsOpen;

                if(_LightSource != null)
                    _LightSource.SwitchOffLight();

                if(_Extents != null)
                {
                    int lLength = _Extents.Length;
                    for (int i = 0; i < lLength; i++)
                        _Extents[i].gameObject.SetActive(false);
                }

                // Play Animation
                _Animator.SetTrigger(_AnimationTrigger);
                _Collider2D.enabled = false;
                GetComponent<ScriptSFX>().PlaySFX();
            }
        }

        public void ResetObject()
        {
            _Animator.SetTrigger(_AnimationResetTrigger);
            _Animator.ResetTrigger(_AnimationTrigger);

            _Collider2D.enabled = true;

            _IsOpen = true;
            _LightSource.SwitchOnLight();

            if (_Extents != null)
            {
                int lLength = _Extents.Length;
                for (int i = 0; i < lLength; i++)
                    _Extents[i].gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            Killzone.Killzone.resetLevel.RemoveListener(ResetObject);
        }
    }
}
