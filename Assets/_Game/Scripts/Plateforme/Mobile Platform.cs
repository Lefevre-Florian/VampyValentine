using System;

using UnityEngine;

using Com.IsartDigital.Platformer.Utils;
using Com.IsartDigital.Platformer.InGameElement.Killzone;
using Com.IsartDigital.Platformer.SoundManager;

// Author : Bastien Chevallier
namespace Com.IsartDigital.Platformer.Game
{
    public class MobilePlatform : MonoBehaviour,IResetableObject
    {
        [SerializeField] private LineCreation _CurrentLine;
        [SerializeField] public Transform[] points;
        [SerializeField] private bool _IsDiag;

        public float movementSpeed = 0.01f;

        [NonSerialized] public Vector3 d;
        [NonSerialized] public float ratio = 0.5f;

        [NonSerialized] public bool isHold = false;
        [NonSerialized] public bool isInversed = false;

        private float _InitialRatio = 0f;

        private MouseDetection _Mouse;
        private Vector3 _StartPos;
        public Vector3 initialPosition { get; private set; }

        private Vector3 _MousePos;
        private Vector3 _PosToRail;
        private Vector3 _DistanceToRail;
        private Vector3 _PosOnRail;

        public ScriptSFX soundOnMove => GetComponent<ScriptSFX>();

        private void Start()
        {
            //Object Reset
            Killzone.resetLevel.AddListener(ResetObject);
            
            d = points[1].position - points[0].position;

            float lAB = (points[1].position - points[0].position).magnitude;
            float lAC = (transform.position - points[0].position).magnitude;

            _InitialRatio = lAC / lAB;

            d = d.normalized;

            ratio = _InitialRatio;

            _CurrentLine.SetUpLine(points);
            _Mouse = MouseDetection.GetInstance();

            initialPosition = transform.position;
            isInversed = points[0].position.y > points[1].position.y;
        }

        private void OnMouseOver()
        {
            if(Input.GetMouseButtonDown(0) && _Mouse._CurrentPlatform == null)
                _Mouse._CurrentPlatform = this;
        }

        public void Replace(Transform pStart, Transform pEnd)
        {
            #if UNITY_ANDROID
            _MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _PosToRail = new Vector2(_MousePos.x - points[0].transform.position.x, _MousePos.y - points[0].transform.position.y);
            _DistanceToRail = (Vector3.Dot(_PosToRail, d)/Vector3.Dot(d,d))*d;
            _PosOnRail = _DistanceToRail + points[0].transform.position;
            transform.position = _PosOnRail;
            #endif
            #if UNITY_STANDALONE
            transform.position = Vector3.Lerp(points[0].position, points[1].position, ratio);
            #endif

            if (isInversed)
            {
                if(!_IsDiag)
                {
                    if (transform.position.x <= pStart.position.x && transform.position.y >= pStart.position.y)
                    {
                        transform.position = pStart.position;
                        ratio = 0;
                    }
                    else if (transform.position.x >= pEnd.position.x && transform.position.y <= pEnd.position.y)
                    {
                        transform.position = pEnd.position;
                        ratio = 1;
                    }
                }
                else
                {
                    if (transform.position.x >= pStart.position.x && transform.position.y >= pStart.position.y)
                    {
                        transform.position = pStart.position;
                        ratio = 0;
                    }
                    else if (transform.position.x <= pEnd.position.x && transform.position.y <= pEnd.position.y)
                    {
                        transform.position = pEnd.position;
                        ratio = 1;
                    }
                }
            }else {
                if(!_IsDiag)
                {
                    if(transform.position.x <= pStart.position.x && transform.position.y <= pStart.position.y)
                    {
                        transform.position = pStart.position;
                        ratio = 0;
                    }
                    else if (transform.position.x >= pEnd.position.x && transform.position.y >= pEnd.position.y)
                    {
                        transform.position = pEnd.position;
                        ratio = 1;
                    }
                }
                else
                {
                    if (transform.position.x >= pStart.position.x && transform.position.y <= pStart.position.y)
                    {
                        transform.position = pStart.position;
                        ratio = 0;
                    }
                    else if (transform.position.x <= pEnd.position.x && transform.position.y >= pEnd.position.y)
                    {
                        transform.position = pEnd.position;
                        ratio = 1;
                    }
                }
            }
        }

        public void ResetObject()
        {
            MouseDetection.GetInstance().currentIndex = 0;
            MouseDetection.GetInstance().SetupPlatform();

            ratio = _InitialRatio;
            transform.position = initialPosition;
        }

        private void OnDestroy() => Killzone.resetLevel.RemoveListener(ResetObject);
    }
}