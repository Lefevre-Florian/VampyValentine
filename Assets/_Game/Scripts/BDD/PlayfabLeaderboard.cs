using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Com.IsartDigital.Platformer.InGameElement.Collectible;

//Author : Bastien Chevallier
namespace Com.IsartDigital.Platformer.BDD
{
    public class PlayfabLeaderboard : MonoBehaviour
    {
        [SerializeField] private GameObject _StatContainer;
        [SerializeField] private GameObject _Stats;
        [SerializeField] private int _LeaderboardTarget;
        [SerializeField] private bool _IsTitlecard;

        private const string LEVEL_ONE_SCORE_LEADERBOARD = "LevelOneScore";
        private const string LEVEL_ONE_TIMER_LEADERBOARD = "LevelOneTimer";
        private const string LEVEL_ONE_COLLECTIBLE_LEADERBOARD = "LevelOneCollectible";
        private const string LEVEL_TWO_SCORE_LEADERBOARD = "LevelTwoScore";
        private const string LEVEL_TWO_TIMER_LEADERBOARD = "LevelTwoTimer";
        private const string LEVEL_TWO_COLLECTIBLE_LEADERBOARD = "LevelTwoCollectible";

        private const string HIGH_SCORE = "HighScore";
        private const string BEST_TIME = "BestTime";
        private const string COLLECTIBLE = "Collectible";
        private const string HIGH_SCORE2 = "HighScore2";
        private const string BEST_TIME2 = "BestTime2";
        private const string COLLECTIBLE2 = "Collectible2";
        private const string LEVEL_PASSED = "LevelPassed";

        public List<string> ListLevelOneLeaderboard = new List<string> { LEVEL_ONE_SCORE_LEADERBOARD, LEVEL_ONE_TIMER_LEADERBOARD, LEVEL_ONE_COLLECTIBLE_LEADERBOARD };
        public List<string> ListLevelTwoLeaderboard = new List<string> { LEVEL_TWO_SCORE_LEADERBOARD, LEVEL_TWO_TIMER_LEADERBOARD, LEVEL_TWO_COLLECTIBLE_LEADERBOARD };

        private bool _IsInLeaderboard = false;

        private string _CurrentLeaderboard;
        private List<string> _CurrentStatList = new List<string> { };

        private void Start()
        {
            if ((PlayerPrefs.GetInt(HIGH_SCORE) != 0 || PlayerPrefs.GetInt(HIGH_SCORE2) != 0) && !PlayfabLogin.Instance.offline)
            {
                SendLeaderboard(
                    new List<int>
                    {
                        PlayerPrefs.GetInt(HIGH_SCORE, 0),
                        PlayerPrefs.GetInt(BEST_TIME, 0),
                        PlayerPrefs.GetInt(COLLECTIBLE, 0)
                    }, ListLevelOneLeaderboard);

                SendLeaderboard(
                    new List<int>
                    {
                        PlayerPrefs.GetInt(HIGH_SCORE2, 0),
                        PlayerPrefs.GetInt(BEST_TIME2, 0),
                        PlayerPrefs.GetInt(COLLECTIBLE2, 0)
                    }, ListLevelTwoLeaderboard);

                PlayerPrefs.SetInt(HIGH_SCORE, 0);
                PlayerPrefs.SetInt(BEST_TIME, 0);
                PlayerPrefs.SetInt(COLLECTIBLE, 0);
                PlayerPrefs.SetInt(HIGH_SCORE2, 0);
                PlayerPrefs.SetInt(BEST_TIME2, 0);
                PlayerPrefs.SetInt(COLLECTIBLE2, 0);
                UpdatePlayerStatisticsRequest statRequest = new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                            {
                                new StatisticUpdate
                                {
                                    StatisticName = LEVEL_PASSED,
                                    Value = PlayerPrefs.GetInt(LEVEL_PASSED, 0)
                                },
                            }
                };
                PlayFabClientAPI.UpdatePlayerStatistics(statRequest, OnStatInit, OnError);

                PlayerPrefs.SetInt(LEVEL_PASSED, 0);
            }

            if (!PlayfabLogin.Instance.offline)
            {
                if(_LeaderboardTarget == 1)
                {
                    GetLeaderboardLevelOne();
                }
                else if(_LeaderboardTarget == 2)
                {
                    GetLeaderboardLevelTwo();
                    if (_IsTitlecard) gameObject.SetActive(false);
                }
            }
        }

        private void OnStatInit(UpdatePlayerStatisticsResult result) { }

        //A appeler quand le joueur a fini la partie
        public void SendLeaderboard(List<int> pStatList, List<string> pTargetLeaderboard)
        {
            if(!PlayfabLogin.Instance.offline)
            {
                _CurrentStatList = pTargetLeaderboard;

                UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                        {
                            new StatisticUpdate
                            {
                                StatisticName = pTargetLeaderboard[0],
                                Value = pStatList[0]
                            },
                            new StatisticUpdate
                            {
                                StatisticName = pTargetLeaderboard[1],
                                Value = pStatList[1] * -1
                            },
                            new StatisticUpdate
                            {
                                StatisticName = pTargetLeaderboard[2],
                                Value = pStatList[2]
                            }
                        }
                };
                PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate, OnError);
            }
        }

        private void OnError(PlayFabError error)
        {
            Debug.Log(error.GenerateErrorReport());
        }

        private void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result)
        {
            return;
        }

        public void GetLeaderBoard(string pLeaderboard)
        {
            int lStatCount = _StatContainer.transform.childCount;
            for (int i = 1; i < lStatCount; i++)
            {
                Destroy(_StatContainer.transform.GetChild(i).gameObject);
            }

            _CurrentLeaderboard = pLeaderboard;

            if(!PlayfabLogin.Instance.offline)
            {
                GetLeaderboardRequest request = new GetLeaderboardRequest
                {
                    StatisticName = _CurrentLeaderboard,
                    StartPosition = 0,
                    MaxResultsCount = 5,
                    ProfileConstraints = new PlayerProfileViewConstraints
                    {
                        ShowDisplayName = true,
                        ShowStatistics = true
                    }
                };
                PlayFabClientAPI.GetLeaderboard(request, OnLeaderBoardGet, OnError);
            }
        }

        private void OnLeaderBoardGet(GetLeaderboardResult result)
        {
            foreach (PlayerLeaderboardEntry item in result.Leaderboard)
            {
                GameObject lStats = Instantiate(_Stats, _StatContainer.transform);
                TextMeshProUGUI[] lTexts = lStats.GetComponentsInChildren<TextMeshProUGUI>();

                lTexts[0].text = (item.Position + 1).ToString();
                lTexts[1].text = item.DisplayName;

                foreach (StatisticModel pStat in item.Profile.Statistics)
                {
                    if (pStat.Name == _CurrentStatList[2]) lTexts[4].text = pStat.Value.ToString();
                    else if (pStat.Name == _CurrentStatList[0]) lTexts[2].text = pStat.Value.ToString();
                    else if (pStat.Name == _CurrentStatList[1])
                    {
                        int lSecond = pStat.Value *-1 % 60;
                        string lSec = "";
                        if (lSecond < 10)
                        {
                             lSec = "0" + lSecond;
                        }else
                        {
                            lSec = lSecond.ToString();
                        }
                        string calculateTime = pStat.Value / 60*-1 + ":" + lSec;
                        lTexts[3].text = calculateTime;
                    }
                }
                if (item.PlayFabId == PlayfabLogin.Instance.playerID)
                {
                    foreach (TextMeshProUGUI pText in lTexts)
                    {
                        pText.color = Color.red;
                    }
                    _IsInLeaderboard = true;
                }
            }

            if(!_IsInLeaderboard)
            {
                GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest
                {
                    StatisticName = _CurrentLeaderboard,
                    MaxResultsCount = 1,
                    ProfileConstraints = new PlayerProfileViewConstraints
                    {
                        ShowDisplayName = true,
                        ShowStatistics = true,
                    }
                };
                PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardAroundPlayer, OnError);
            }else
            {
                _IsInLeaderboard = false;
            }
        }

        private void OnLeaderboardAroundPlayer(GetLeaderboardAroundPlayerResult result)
        {
            foreach (PlayerLeaderboardEntry item in result.Leaderboard)
            {
                GameObject lStats = Instantiate(_Stats, _StatContainer.transform);
                TextMeshProUGUI[] lTexts = lStats.GetComponentsInChildren<TextMeshProUGUI>();

                lTexts[0].text = (item.Position + 1).ToString();
                lTexts[1].text = item.DisplayName;

                foreach (StatisticModel pStat in item.Profile.Statistics)
                {
                    if (pStat.Name == _CurrentStatList[2]) lTexts[4].text = pStat.Value.ToString();
                    else if (pStat.Name == _CurrentStatList[0]) lTexts[2].text = pStat.Value.ToString();
                    else if (pStat.Name == _CurrentStatList[1])
                    {
                        int lSecond = pStat.Value * -1 % 60;
                        string lSec = lSecond.ToString();
                        if (lSecond < 10) lSec = "0" + lSecond;
                        string calculateTime = pStat.Value / 60 * -1 + ":" + lSec;
                        lTexts[3].text = calculateTime;
                    }
                }
                if (item.PlayFabId == PlayfabLogin.Instance.playerID)
                {
                    foreach (TextMeshProUGUI pText in lTexts)
                    {
                        pText.color = Color.red;
                    }
                    _IsInLeaderboard = false;
                }
                if (lTexts[2].text == "Score")
                {
                    Destroy(lStats.gameObject);
                }
            }
        }

        public void GetLeaderboardLevelOne(GameObject pScreenToHide = null)
        {
            if (pScreenToHide != null)
            {
                pScreenToHide.SetActive(false);
                gameObject.SetActive(true);
            }
            _CurrentStatList = ListLevelOneLeaderboard;
            GetLeaderBoard(LEVEL_ONE_SCORE_LEADERBOARD);
        }

        public void GetLeaderboardLevelTwo(GameObject pScreenToHide = null)
        {
            if (pScreenToHide != null)
            {
                pScreenToHide.SetActive(false);
                gameObject.SetActive(true);
            }
            _CurrentStatList = ListLevelTwoLeaderboard;
            GetLeaderBoard(LEVEL_TWO_SCORE_LEADERBOARD);
        }
    }
}