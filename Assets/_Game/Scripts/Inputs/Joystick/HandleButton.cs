using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.IsartDigital.Platformer.Joystick
{
    public class HandleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Joystick _Joystick = null;
        private void Start()
        {
            if (Joystick.GetInstance() != null)
            {
                _Joystick = Joystick.GetInstance();
                _Joystick.ResetHandlePose();
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_Joystick != null)
                _Joystick.isBtnPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_Joystick != null)
            {
                _Joystick.isBtnPressed = false;
                _Joystick.ResetHandlePose();
            }
        }
    }
}