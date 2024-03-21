using UnityEngine;
using UnityEngine.UI;
using Com.IsartDigital.Platformer.Game;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.UI.Gameplay
{
    public class BtnHUD : MonoBehaviour
    {
        protected Button _Button = null;


        protected Cinematic _Cinematic = null;
        protected virtual void Start()
        {
            if (Cinematic.GetInstance() != null)
            {
                _Cinematic = Cinematic.GetInstance();
            }
            _Button = GetComponent<Button>();
            _Button.onClick.AddListener(OnBtn);
        }

        protected virtual void OnBtn() { }
        protected virtual void OnDestroy()
        {
            _Button?.onClick.RemoveAllListeners();
        }
    }
}