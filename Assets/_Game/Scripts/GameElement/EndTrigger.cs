using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections.Generic;
using UnityEngine;
using Com.IsartDigital.Platformer.BDD;
using Com.IsartDigital.Platformer.SoundManager;

// Author : 
namespace Com.IsartDigital.Platformer
{
    public class EndTrigger : MonoBehaviour
    {
        [SerializeField] private int _LevelPassed;

        private const string PLAYER_TAG = "Player";

        private const string LEVEL_PASSED = "LevelPassed";
        [SerializeField] private ScriptSFX _OnWinScreenSFX => GetComponent<ScriptSFX>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(PLAYER_TAG)) 
            {
                GameManager.GetInstance().OnLevelEnd();
                collision.gameObject.GetComponent<PlayerBis.PlayerPhysic>()?.StopSFXFootStep();
            }
            _OnWinScreenSFX.PlaySFX();
            if (!PlayfabLogin.Instance.offline)
            {
                UpdatePlayerStatisticsRequest statRequest = new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                            {
                                new StatisticUpdate
                                {
                                    StatisticName = LEVEL_PASSED,
                                    Value = _LevelPassed
                                },
                            }
                };
                PlayFabClientAPI.UpdatePlayerStatistics(statRequest, OnStatInit, OnError);
            }else
            {
                PlayerPrefs.SetInt(LEVEL_PASSED,_LevelPassed);
            }
        }

        private void OnStatInit(UpdatePlayerStatisticsResult result)
        {

        }

        private void OnError(PlayFabError error)
        {

        }
    }
}
