using Com.IsartDigital.Platformer.Game;
using Com.IsartDigital.Platformer.SoundManager;
using System;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.InGameElement.Collectible
{
    public class Collectible : GameElement
    {
        public CollectibleInfo collectibleInfo = default;
        [NonSerialized] private GameManager.CollectibleType _CollectibleType;

        private int _ScoreOnCollect = 10;
        private ScoreManager.ScoreManager _ScoreManager = null;

        private ScriptSFX _ScriptSFX => GetComponent<ScriptSFX>();

        protected override void Start()
        {
            base.Start();
            if (ScoreManager.ScoreManager.GetInstance() != null) 
                _ScoreManager = ScoreManager.ScoreManager.GetInstance();

            if (_CollectibleType == GameManager.CollectibleType.Normal)
                ScoreManager.ScoreManager.GetInstance().nbMaxNormalCollectible++;

            UpdateInfo();
        }

        public override void Effect(PlayerBis.PlayerController pPlayer)
        {
            _ScriptSFX?.PlaySFX();

            _ScoreManager.UpdateCollectibleScore(_ScoreOnCollect, _CollectibleType);

            Destroy(gameObject);
        }

        public void UpdateInfo()
        {
            if (collectibleInfo != null)
            {
                _ScoreOnCollect = collectibleInfo.score;
                _CollectibleType = collectibleInfo.type;
            }
        }
    }
}