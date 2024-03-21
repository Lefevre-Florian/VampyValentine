using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer.BDD
{
    public static class PlayfabData
    {
        private static Dictionary<string, UserDataRecord> _userData;
        private static bool _IsGettingUserData = false;

        public static void SaveData(Dictionary<string, string> pData,
            Action<UpdateUserDataResult> pOnSuccess,
            Action<PlayFabError> pOnFail) 
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = pData
            },
            successResult =>
            {
                if (_userData != null)
                {
                    foreach (var key in pData.Keys)
                    {
                        UserDataRecord lValue = new() { Value = pData[key] };

                        if (_userData.ContainsKey(key)) _userData[key] = lValue;
                        else _userData.Add(key, lValue);
                    }
                }

                pOnSuccess(successResult);
            },
            pOnFail);
        }

        public static void GetUserData(Action<GetUserDataResult> pOnSuccess,
            Action<PlayFabError> pOnFail)
        {
            while(_IsGettingUserData)
            {
                Task.Delay(100);
            }

            if(_userData != null )
            {
                pOnSuccess(new GetUserDataResult() { Data = _userData });
                return;
            }

            _IsGettingUserData = true;
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
                onSuccessResult =>
                {
                    _userData = onSuccessResult.Data;
                    _IsGettingUserData = false;

                    pOnSuccess(onSuccessResult);
                }, 
                onFailResult =>
                {
                    _IsGettingUserData=false;
                    pOnFail(onFailResult);
                });
        }

        public static void TryGetData<T>(string key, Action<T> pOnSuccess, Action<PlayFabError> pOnFail)
        {
            try
            {
                if(_userData != null)
                {
                    if (_userData.ContainsKey(key)) pOnSuccess((T)Convert.ChangeType(_userData[key].Value, typeof(T)));
                    else pOnFail(new());
                    return;
                }

                PlayFabClientAPI.GetUserData(new(),
                    GetResult =>
                    {
                        _userData = GetResult.Data;

                        if (_userData.ContainsKey(key))
                            pOnSuccess((T)Convert.ChangeType(GetResult.Data[key].Value, typeof(T)));
                        else pOnFail(new());
                    },
                    Error => pOnFail(Error)
                    );
            }
            catch (Exception e)
            {
                pOnFail(new());
            }
        }
    }
}
