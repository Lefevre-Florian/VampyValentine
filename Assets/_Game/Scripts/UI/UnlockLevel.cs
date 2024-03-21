using Com.IsartDigital.Platformer.BDD;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : Bastien Chevallier
namespace Com.IsartDigital.Platformer.Game
{
    public class UnlockLevel : MonoBehaviour
    {
        public int levelNum = 0;

        private Button _CurrentButton;

        private const string LEVEL_PASSED = "LevelPassed";

        private List<string> list = new List<string> { LEVEL_PASSED };
  
        void Start()
        {
            _CurrentButton = GetComponent<Button>();

            if(!PlayfabLogin.Instance.offline)
            {
                GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest()
                {
                    StatisticNames = list,
                };

                PlayFabClientAPI.GetPlayerStatistics(request, OnStatGet, OnError);
            }
        }

        private void OnStatGet(GetPlayerStatisticsResult result)
        {
            if (result.Statistics[0].Value >= levelNum)
            {
                _CurrentButton.interactable = true;
            }else
            {
                _CurrentButton.interactable = false;
            }

            UpdatePlayerStatisticsRequest statRequest = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                        {
                            new StatisticUpdate
                            {
                                StatisticName = LEVEL_PASSED,
                                Value = 1
                            },
                        }
            };
            PlayFabClientAPI.UpdatePlayerStatistics(statRequest, OnStatInit, OnError);
        }

        private void OnStatInit(UpdatePlayerStatisticsResult result)
        {
        }

        private void OnError(PlayFabError error)
        {
        }
    }
}