using Com.IsartDigital.Platformer.BDD;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

using TMPro;
using UnityEngine;

// Author : Bastien Chevalier
namespace Com.IsartDigital.Platformer.UI
{
    public class WinScreen : Screen
    {
        [Header("WinScreen")]
        [SerializeField] private TextMeshProUGUI _NameText;
        [SerializeField] private TextMeshProUGUI _ScoreText;
        [SerializeField] private TextMeshProUGUI _TimerText;
        [SerializeField] private TextMeshProUGUI _CollectibleText;
        [SerializeField] private PlayfabLeaderboard _Leaderboard;
        [SerializeField] private int _LevelIndex;

        private List<string> _LeaderboardTarget = new List<string> { };

        [SerializeField] private int _LevelSelectorBuildIndex;
        [SerializeField] private int _NextLevelBuildIndex;

        private const string HIGH_SCORE = "HighScore";
        private const string BEST_TIME = "BestTime";
        private const string COLLECTIBLE = "Collectible";
        private const string HIGH_SCORE2 = "HighScore2";
        private const string BEST_TIME2 = "BestTime2";
        private const string COLLECTIBLE2 = "Collectible2";

        protected override void Start()
        {
            base.Start();

            if (!PlayfabLogin.Instance.offline)
            {
                if(_LevelIndex == 1)
                {
                    PlayerPrefs.SetInt(HIGH_SCORE, ScoreManager.ScoreManager.GetInstance().scoreTotal);
                    PlayerPrefs.SetInt(BEST_TIME, GameManager.GetInstance().totalTime);
                    PlayerPrefs.SetInt(COLLECTIBLE, ScoreManager.ScoreManager.GetInstance().NbNormalCollectibleCollected);
                }
                else if (_LevelIndex == 2)
                {
                    PlayerPrefs.SetInt(HIGH_SCORE2, ScoreManager.ScoreManager.GetInstance().scoreTotal);
                    PlayerPrefs.SetInt(BEST_TIME2, GameManager.GetInstance().totalTime);
                    PlayerPrefs.SetInt(COLLECTIBLE2, ScoreManager.ScoreManager.GetInstance().NbNormalCollectibleCollected);
                }
                if (_LevelIndex == 1) _LeaderboardTarget = _Leaderboard.ListLevelOneLeaderboard;
                else if(_LevelIndex == 2) _LeaderboardTarget = _Leaderboard.ListLevelTwoLeaderboard;

                if(_LeaderboardTarget != null) SendLeaderboard(new List<int> { ScoreManager.ScoreManager.GetInstance().scoreTotal, GameManager.GetInstance().totalTime, ScoreManager.ScoreManager.GetInstance().NbNormalCollectibleCollected }, _LeaderboardTarget);
                _NameText.text += PlayfabLogin.Instance.playerName;
            }
            else
            {
                _NameText.text += "Offline User";
            }

            _ScoreText.text += ScoreManager.ScoreManager.GetInstance().scoreTotal.ToString();
            _TimerText.text += GameManager.GetInstance().totalTime.ToString();
            _CollectibleText.text += ScoreManager.ScoreManager.GetInstance().NbNormalCollectibleCollected.ToString();
        }

        public void SendLeaderboard(List<int> pStatList, List<string> pTargetLeaderboard)
        {
            if (!PlayfabLogin.Instance.offline)
            {
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

        private void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result)
        {
            if (_LevelIndex == 1) _Leaderboard.GetLeaderboardLevelOne();
            else if (_LevelIndex == 2) _Leaderboard.GetLeaderboardLevelTwo();
        }

        private void OnError(PlayFabError error)
        {
            return;
        }

        public void NextLevel()
        {
            //Changer niveau
            LoadScene.GetInstance().StartLoadScene(_NextLevelBuildIndex);
        }

        public void Back()
        {
            //retour level selector
            GameManager.currentGameState = GameManager.GameState.Menu;
            LoadScene.GetInstance().StartLoadScene(_LevelSelectorBuildIndex);
        }
    }
}