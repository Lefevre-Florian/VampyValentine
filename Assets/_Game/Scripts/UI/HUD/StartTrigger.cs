using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Com.IsartDigital.Platformer.UI.Gameplay
{
    public class StartTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject _ProgressBar = default;
        private const string PLAYER_TAG = "Player";
        [SerializeField] private float _SecondToStartSFX = 0.5f;

        private void Start()
        {
            if (_ProgressBar == null)
            {
                Destroy(this);
            }
            else
                _ProgressBar.SetActive(false);

        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(PLAYER_TAG))
            {
                _ProgressBar.SetActive(true);
                _ProgressBar.GetComponent<LevelEvolution>().ResetProgressBar();
                StartCoroutine(StartSFX());
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        public IEnumerator StartSFX()
        {
            yield return new WaitForSeconds(_SecondToStartSFX);
            _ProgressBar.GetComponent<AfterCinematicHUDElement>().SetTriggerOnCollect();
            StopCoroutine(StartSFX());
        }
    }
}
