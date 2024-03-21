using Com.IsartDigital.Platformer.PlayerBis;
using System;
using System.Collections.Generic;
using UnityEngine;

// Author : Bastien Chevallier
namespace Com.IsartDigital.Platformer.Game
{
    public class PlateformeTraversable : MonoBehaviour
    {
        private PlatformEffector2D _PlatformEffector;
        public bool isDown;
        private const string _DownTag = "Down";
        private const string _UpTag = "Up";
        void Start()
        {
            _PlatformEffector = GetComponent<PlatformEffector2D>();
            gameObject.tag = _UpTag;
        }

        private void Update()
        {
            if (isDown && gameObject.tag == _UpTag)
            {
                gameObject.tag = _DownTag;
                _PlatformEffector.rotationalOffset = 180f;
                PlayerController.GetInstance().GetComponent<PlayerPhysic>().isTraversable = true;
            }
            else if (PlayerController.GetInstance().GetComponent<PlayerController>().rb.velocity.y > 0)
            {
                gameObject.tag = _UpTag;
                _PlatformEffector.rotationalOffset = 0f;
            }
            else
            {
                if (!PlayerController.GetInstance().GetComponent<PlayerPhysic>().isTraversable)
                {
                    isDown = false;
                    gameObject.tag = _UpTag;
                }
            }
        }
    }
}