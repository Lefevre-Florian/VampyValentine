using UnityEngine.EventSystems;
using UnityEngine;

using Com.IsartDigital.Platformer.PlayerBis;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.UI.Gameplay
{
    public class BtnJump : BtnHUD, IPointerDownHandler, IPointerUpHandler    
    {
        private PlayerController _Player = null;
        private bool _BtnIsHolding = false;
        private float _TimeCounter = 0f;
        [SerializeField] private float _TimeMax = 0.5f;

        protected override void Start()
        {
            base.Start();
            _Player = PlayerController.GetInstance();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            _BtnIsHolding = true;
            _Player.btnJumpIsHolding = _BtnIsHolding;
            StartCoroutine(_Player.GlideCountdown());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _Player.isGliding = false;
            _BtnIsHolding = false;
            _Player.btnJumpIsHolding = _BtnIsHolding;
        }

        protected override void OnBtn()
        {
            if (!_Player.canJump) return;

            _Player.canJump = false;
            base.OnBtn();
        }
    }
}