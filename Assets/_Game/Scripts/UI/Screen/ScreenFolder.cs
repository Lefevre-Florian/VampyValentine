using System;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer.UI
{
    public class ScreenFolder : Screen
    {
        [Header("Sub-menu")]
        [SerializeField] private GameObject[] _SubScreens = new GameObject[0];

        private GameObject _CurrentSubScreen = null;

        protected override void Start()
        {
            base.Start();
            if (_SubScreens.Length > 0)
                SwitchSubMenu(_SubScreens[0]);
        }

        public void SwitchSubMenu(GameObject pSubScreen)
        {
            int lLength = _SubScreens.Length;
            for (int i = 0; i < lLength; i++)
                _SubScreens[i].SetActive(false);

            _CurrentSubScreen = pSubScreen;
            _CurrentSubScreen.SetActive(true);
        }

    }
}
