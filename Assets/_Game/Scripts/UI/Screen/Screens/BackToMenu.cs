using Com.IsartDigital.Platformer.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace  Com.IsartDigital.Platformer
{
    public class BackToMenu : MonoBehaviour
    {

        [SerializeField] private Animator _BatTransition;
        private const int TITLECARD_IDX = 1;
        private const string TRANSITION_ANIMATION_NAME = "Bat_Transition";

        ScriptSFX _BatSFX => GetComponent<ScriptSFX>();


        public IEnumerator BackToScreen()
        {
            _BatTransition.Play(TRANSITION_ANIMATION_NAME);
            _BatSFX.PlaySFX();
            yield return new WaitForSecondsRealtime(1f);
            SceneManager.LoadScene(TITLECARD_IDX);
        }
    }
}
