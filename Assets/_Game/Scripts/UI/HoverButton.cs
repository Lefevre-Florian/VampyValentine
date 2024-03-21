using Com.IsartDigital.Platformer;
using Com.IsartDigital.Platformer.SoundManager;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.IsartDigital.Platformer.UI

{
    public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler
    {
        [SerializeField] private ParticleSystem _ParticleSystem;
        [SerializeField] private ParamRef _ParamMusic;
        [SerializeField] private ScriptSFX _MusicHeart;
        [SerializeField] private ScriptSFX _OnHover;
        [SerializeField] private ScriptSFX _OnClick;

        [SerializeField] private bool _IsTitleCardBtn = true;

        // -------- Mouse --------//
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_IsTitleCardBtn)
            {
                _ParticleSystem?.Play();
                _MusicHeart?.PlayLongSFX();
            }
            _OnHover?.PlaySFX();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_IsTitleCardBtn)
            {
                _ParticleSystem?.Stop();
                _MusicHeart?.StopLongSFX();
            }
        }

        // -------- External controller --------//
        public void OnSelect(BaseEventData pEventData)
        {
            if (_IsTitleCardBtn)
            {
                _ParticleSystem?.Play();
                _MusicHeart?.PlayLongSFX();
            }
            _OnHover?.PlaySFX();
        }

        public void OnDeselect(BaseEventData pEventData)
        {
            if (_IsTitleCardBtn)
            {
                _ParticleSystem?.Stop();
                _MusicHeart?.StopLongSFX();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_IsTitleCardBtn)
            {
                _MusicHeart?.StopLongSFX();
                if (_MusicHeart != null) _MusicHeart.stopLong = true;
            }
            _OnClick?.PlaySFX();
        }
    }
}
