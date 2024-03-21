using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Author : 
namespace Com.IsartDigital.Platformer
{
    public class LevelSelector_Animator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private bool _IsBtnBack = false;
        public void OnDeselect(BaseEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnSelect(BaseEventData eventData)
        {
            throw new NotImplementedException();
        }

        void Start()
        {
            
        }

    }
}
