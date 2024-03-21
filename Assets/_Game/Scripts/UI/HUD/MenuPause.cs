using Com.IsartDigital.Platformer.SoundManager;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.UI
{
    public class MenuPause : Screen
    {
        [Header("Scene Management")]
        [SerializeField] private int _MenuBuildIndex = 0;

        private HUD _HUD = null;

        protected override void Start()
        {
            _HUD = HUD.GetInstance();
            base.Start();
        }

        public void OnResume()
        {
            CloseScreen();
            _HUD.Resume();
        }

        public void OnPause()
        {
            OpenScreen();

            _HUD.Pause();
        }

        public override void Quit()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_MenuBuildIndex);
        }

        
        public void OnRestart() => _HUD.Restart();
    }
}