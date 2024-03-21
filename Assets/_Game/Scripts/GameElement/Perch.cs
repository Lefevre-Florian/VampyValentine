using Com.IsartDigital.Platformer.ScoreManager;
using Com.IsartDigital.Platformer.SoundManager;
using System;
using UnityEngine;
using static Com.IsartDigital.Platformer.GameManager;

namespace Com.IsartDigital.Platformer.InGameElement.Collectible
{
    public class Perch : GameElement
    {
        [SerializeField] private CollectibleInfo _CollectibleInfo = null;

        private PlayerBis.PlayerController _PlayerController = null;

        private ScoreManager.ScoreManager _ScoreManager = null;

        private ScriptSFX _ScriptSFX = null;

        private BoxCollider2D _BoxCollider => GetComponent<BoxCollider2D>();

        private PerchCage _Cage => GetComponentInChildren<PerchCage>();


        private void Awake()
        {
            _ScriptSFX = GetComponent<ScriptSFX>();
        }
        protected override void Start()
        {
            base.Start();
            
            _ScoreManager = ScoreManager.ScoreManager.GetInstance();
            _ScoreManager.nbMaxPerchCollectible++;
            
        }

        public override void Effect(PlayerBis.PlayerController pPlayer)
        {
            _PlayerController = pPlayer;

            if (_PlayerController.isDashing)
            {
                _BoxCollider.enabled = false;
                _ScriptSFX?.PlaySFX();
                _ScoreManager.UpdateCollectibleScore(_CollectibleInfo.score, _CollectibleInfo.type);
                _Cage.PlayerEntered();
            }
        }

        
    }
}
