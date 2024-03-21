using System;
using UnityEngine;
using Com.IsartDigital.Platformer.Utils;
using Com.IsartDigital.Platformer.InGameElement.Killzone;
using Com.IsartDigital.Platformer.SoundManager;

// Author : Bastien Chevallier
namespace Com.IsartDigital.Platformer
{
    public class AutomaticPlatform : MonoBehaviour,IResetableObject
    {
        [SerializeField] private Transform[] _Points;
        [SerializeField] private float _PlatformMovementDuration = 1f;

        private float _ElapsedTime;
        private float _Ratio;
        
        [SerializeField] private bool _IsLeft = false;

        private bool _InitialLeft;

        public Vector3 initialPosition { get; private set; }


        private void Start()
        {
            Vector3 lRailDirection = (_Points[0].position - _Points[_Points.Length - 1].position).normalized;

            Vector2 lRailPosition = new Vector2(transform.position.x - _Points[0].transform.position.x, transform.position.y - _Points[0].transform.position.y);
            Vector3 lDistanceToRail = (Vector3.Dot(lRailPosition, lRailDirection) / Vector3.Dot(lRailDirection, lRailDirection)) * lRailDirection;
            lRailPosition = lDistanceToRail + _Points[0].transform.position;

            initialPosition = lRailPosition;
            Killzone.resetLevel.AddListener(ResetObject);

            enabled = false;
            _InitialLeft = _IsLeft;
        }

        private void Update()
        {
            if (GameManager.currentGameState == GameManager.GameState.Pause) return;

            _ElapsedTime += Time.deltaTime;
            _Ratio = _ElapsedTime / _PlatformMovementDuration;
            if (_IsLeft)
            {
                transform.position = Vector3.Lerp(_Points[0].transform.position, _Points[1].transform.position, _Ratio);
            }
            else 
            {
                transform.position = Vector3.Lerp(_Points[1].transform.position, _Points[0].transform.position, _Ratio);
            }

            if(_Ratio >= 1f)
            {
                _IsLeft = !_IsLeft;
                _ElapsedTime = 0f;
            }
        }



        public void ResetObject()
        {
            _IsLeft = _InitialLeft;
            
            _Ratio = 0f;
            _ElapsedTime = 0f;

            transform.position = initialPosition;
        }


        private void OnDestroy()
        {
            Killzone.resetLevel.RemoveListener(ResetObject);

        }
    }
}
