using UnityEngine;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.UI.Gameplay
{
    public class BtnPause : BtnHUD
    {
        [SerializeField] private Canvas _MenuPause = default;

        protected override void OnBtn()
        {
            base.OnBtn();
            if (_MenuPause != null) _MenuPause.enabled = true;
            Time.timeScale = 0;
            GameManager.currentGameState = GameManager.GameState.Pause;
        }
    }
}