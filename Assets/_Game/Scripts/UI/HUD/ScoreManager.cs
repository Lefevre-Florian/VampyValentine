using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.ScoreManager
{
    public class ScoreManager : MonoBehaviour
    {
        #region Singleton
        private static ScoreManager _Instance = null;

        public static ScoreManager GetInstance()
        {
            if (_Instance == null) _Instance = new ScoreManager();
            return _Instance;
        }

        private ScoreManager() : base() { }
        #endregion

        [Header("Export Variables")]
        public int nbMaxPerchCollectible = 0;
        public int nbMaxNormalCollectible = 0;

        [Tooltip("In Second")] [SerializeField,] private float _TimeToShowCollectibelHUD = 2f;
        
        private int _NbNormalCollectibleCollected = 0;
        private int _NbPerchCollectibleCollected = 0;
        
        [NonSerialized] public int scoreTotal = 0;

        private Coroutine _Timer = null;

        public int NbNormalCollectibleCollected { get { return _NbNormalCollectibleCollected; } }

        public event Action<int, int, GameManager.CollectibleType> OnCollectibleUpdated;

        public event Action<bool, GameManager.CollectibleType> OnCollectibleDisplayStatusUpdated;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }

            _Instance = this;

        }

        public void UpdateCollectibleScore(int pScore, GameManager.CollectibleType pType)
        {
            int lMaxScore = 0;
            int lNBCollected = 0;

            switch (pType)
            {
                case GameManager.CollectibleType.Normal:
                    lNBCollected = ++_NbNormalCollectibleCollected;
                    lMaxScore = nbMaxNormalCollectible;
                    break;
                case GameManager.CollectibleType.Perch:
                    lNBCollected = ++_NbPerchCollectibleCollected;
                    lMaxScore = nbMaxPerchCollectible;
                    break;
                default:
                    break;
            }

            OnCollectibleUpdated?.Invoke(lNBCollected, lMaxScore, pType);
            UpdateScore(pScore, pType);
        }

        private void UpdateScore(int pScore, GameManager.CollectibleType pType)
        {
            scoreTotal += pScore;
            _Timer = StartCoroutine(ShowCollectibleHUD(pType));
        }

        private IEnumerator ShowCollectibleHUD(GameManager.CollectibleType pType)
        {
            OnCollectibleDisplayStatusUpdated?.Invoke(true, pType);

            yield return new WaitForSeconds(_TimeToShowCollectibelHUD);

            if (_Timer != null)
                StopCoroutine(ShowCollectibleHUD(pType));
            _Timer = null;

            OnCollectibleDisplayStatusUpdated?.Invoke(false, pType);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            _Timer = null;

            if (_Instance != null)
                _Instance = null;
        }
    }
}