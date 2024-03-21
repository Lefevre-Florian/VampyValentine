using Com.IsartDigital.Platformer.SoundManager;
using UnityEngine;

namespace Com.IsartDigital.Platformer.Game
{
    public class CameraDetector : MonoBehaviour
    {
        private CameraSwitcher _Switcher => GetComponentInParent<CameraSwitcher>();

        private BoxCollider2D _BoxCollider;

        [SerializeField] bool isFromTrigger;
        private UpdateMusicParam _UpdateMusic => GetComponent<UpdateMusicParam>();

        private CameraSwitcher _CamSwitch => transform.parent.GetComponent<CameraSwitcher>();

        private void Start()
        {
            _BoxCollider = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (isFromTrigger)
            {
                _UpdateMusic.UpdateMusic();
                //_CamSwitch.maidSFX.PlayLongSFX();
#if UNITY_STANDALONE
                MouseDetection.GetInstance().currentLevel = _Switcher.currentLevelPlatform.gameObject;
                #endif
                if (_Switcher.isBlocking)
                {
                    _BoxCollider.isTrigger = false;
                }
                else
                {
                    _Switcher.detectorTo.gameObject.SetActive(true);
                }

                Transform lContainer = _Switcher.AutomaticPlatformsContainer;
                if (lContainer != null)
                {
                    int lLength = lContainer.childCount;
                    for (int i = 0; i < lLength; i++)
                        lContainer.GetChild(i)
                                  .GetChild(0)
                                  .GetComponent<AutomaticPlatform>().enabled = true;
                }

                _Switcher.cameraTo?.SetActive(true);
                _Switcher.cameraFrom?.SetActive(false);
                CameraSwitcher.cameraEvent.Invoke(_Switcher.cameraTo);

                if (_Switcher.isCameraToForced)
                {
                    _Switcher.animPlayer.enabled = true;
                    _Switcher.animPlayer.Play(_Switcher.anim.name, 0, 0f);
                }

            }

            else
            {
                _CamSwitch.maidSFX?.StopLongSFX();
                _Switcher.cameraTo?.SetActive(false);
                _Switcher.cameraFrom?.SetActive(true);
                _Switcher.detectorFrom.gameObject.SetActive(true);
                CameraSwitcher.cameraEvent.Invoke(_Switcher.cameraFrom);
            }

            if (_Switcher.previousAnimator != null)
            {
                _Switcher.previousAnimator.Play("void", 0,0f);
                
            }

            if (!_Switcher.isBlocking) 
                gameObject.SetActive(false);
        }
    }
}
