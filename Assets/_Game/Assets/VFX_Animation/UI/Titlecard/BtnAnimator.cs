using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Author : 
namespace Com.IsartDigital.Platformer.UI
{
    public class BtnAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler
    {
        [SerializeField] private bool _IsBtnPlay = false;
        private Animator _Animator => GetComponent<Animator>();

        private const string ON_HOVER_PLAY = "OnHoverPlay";
        private const string ON_HOVER = "OnHover";

        private const string ON_UN_HOVER_PLAY = "OnUnHoverStartBtn";
        private const string ON_UN_HOVER = "OnUnHover";
        private void SetTriggerOnHover()
        {
            if (_IsBtnPlay)
                _Animator.SetTrigger(ON_HOVER_PLAY);
            else
               if (GetComponent<Button>().interactable) _Animator.SetTrigger(ON_HOVER);
        }
        private void SetTriggerOnUnHover()
        {
            if (_IsBtnPlay)
                _Animator.SetTrigger(ON_UN_HOVER_PLAY);
            else
                if (GetComponent<Button>().interactable) _Animator.SetTrigger(ON_UN_HOVER);
        }

        public void OnPointerEnter(PointerEventData eventData) => SetTriggerOnHover();

        public void OnPointerExit(PointerEventData eventData) => SetTriggerOnUnHover();

        public void OnSelect(BaseEventData eventData) => SetTriggerOnHover();

        public void OnDeselect(BaseEventData eventData) => SetTriggerOnUnHover();

        public void OnPointerClick(PointerEventData eventData)
        {
            SetTriggerOnUnHover();
        }
    }
}