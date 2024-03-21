using Com.IsartDigital.Platformer.UI.Gameplay;

using System;

using UnityEngine;
using UnityEngine.SceneManagement;

// Author : 
namespace Com.IsartDigital.Platformer.UI
{
    public class CreditScreen : MonoBehaviour
    {
        private const int TITLECARD_IDX = 1;

        [SerializeField] private BtnSkip _SkipBtn = null;
        [SerializeField] private int _TitlecardIdx = TITLECARD_IDX;

        private void Start() => _SkipBtn.OnCinematicSkipped += SkipCredit;

        private void SkipCredit() => SceneManager.LoadScene(_TitlecardIdx);

        private void OnDestroy() => _SkipBtn.OnCinematicSkipped -= SkipCredit;

    }
}
