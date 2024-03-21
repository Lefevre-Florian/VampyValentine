using System;
using UnityEngine;
using UnityEngine.UI;

// Author : 
namespace Com.IsartDigital.Platformer.UI
{
    public class ScreenFocus : Screen
    {
        [SerializeField] private RectTransform _ButtonContainer = null;

        private Button[] _Buttons = new Button[0];

        protected override void Start()
        {
            int lLength = _ButtonContainer.childCount;
            if (lLength > 0)
            {
                _Buttons = new Button[lLength];
                for (int i = 0; i < lLength; i++)
                    _Buttons[i] = _ButtonContainer.GetChild(i).GetComponent<Button>();
            }
            base.Start();
        }

        protected override void OnEnable() => RecreateNavigation(m_FirstSelectedButton.GetComponent<Selectable>());

        #if UNITY_STANDALONE
        protected override void GamepadExtension() { }
        #endif

        protected override void OnDisable() => RecreateNavigation();

        private void RecreateNavigation(Selectable pRightButton = null)
        {
            
            Navigation lBtnNavigation;
            foreach (Button lButton in _Buttons)
            {
                lBtnNavigation = lButton.navigation;
                lBtnNavigation.selectOnRight = pRightButton;
                lButton.navigation = lBtnNavigation;
            }
        }
    }
}
