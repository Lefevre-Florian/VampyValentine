using System;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Platformer.Player
{
    [RequireComponent(typeof(PlayerController), typeof(SpriteRenderer))]
    public class PlayerDebug : MonoBehaviour
    {
        private const int NB_POINT = 100;

        [SerializeField] private Color _CurveColor = Color.red;

        [SerializeField] private bool _ShowJumpCurve = true;
        [SerializeField] private bool _ShowGlideCurve = true;

        // Variables 
        private float _Gravity = 0f;
        private float _JumpForce = 0f;

        private float _ApogeeTime = 0f;

        private float _SpriteHeight = 0f;

        private Vector2 _GroundedPosition = Vector2.zero;

        // References
        private PlayerController _PlayerController = null;
        private PlayerPhysic _PlayerPhysic = null;
        private SpriteRenderer _Renderer = null;

        private void Start()
        {
#if !UNITY_EDITOR
            Destroy(gameObject);
#endif
            _PlayerController = GetComponent<PlayerController>();
            _PlayerPhysic = GetComponent<PlayerPhysic>();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                if (_PlayerController == null) _PlayerController = GetComponent<PlayerController>();
                if (_PlayerPhysic == null) _PlayerPhysic = GetComponent<PlayerPhysic>();
            }
                

            if (_Renderer == null)
            {
                _Renderer = GetComponent<SpriteRenderer>();
                _SpriteHeight = _Renderer.sprite.bounds.size.y;
            }
                
            if (_ShowJumpCurve)
            {
                Gizmos.color = _CurveColor;
                
                if (Application.isPlaying && _PlayerPhysic.IsGrounded
                    || _GroundedPosition == Vector2.zero)
                    _GroundedPosition = transform.position;

                // Computation & Rendering
                float lJumpDuration = _PlayerController.JumpDuration * _SpriteHeight;

                _ApogeeTime = lJumpDuration / 2f;
                _Gravity = -2 * (_PlayerController.JumpHeight * _SpriteHeight) / Mathf.Pow(_ApogeeTime, 2f);
                _JumpForce = 2 * (_PlayerController.JumpHeight * _SpriteHeight) / _ApogeeTime;

                Vector2 lCurrent = Vector2.zero;
                Vector2 lLast = GetPosition(0f);

                for (int i = 1; i < NB_POINT; i++)
                {
                    lCurrent = GetPosition((float)i / (NB_POINT -1) * lJumpDuration);
                    Gizmos.DrawLine(lLast, lCurrent);
                    lLast = lCurrent;
                }
            }

            if (_ShowGlideCurve)
            {
                Gizmos.color = Color.blue;

                if (Application.isPlaying && _PlayerPhysic.IsGrounded
                    || _GroundedPosition == Vector2.zero)
                    _GroundedPosition = transform.position;

                float lApex = _PlayerController.JumpDuration * _SpriteHeight / 2f;
                float lGlideForce = -2 * _PlayerController.JumpHeight / Mathf.Pow(_PlayerController.GlideDuration, 2f);

                Vector2 lStart = GetPosition(lApex);
                Gizmos.DrawSphere(lStart, .1f);

                float lAngle = Mathf.Atan((-lGlideForce * Mathf.Pow(_PlayerController.GlideDuration, 2f)) / _PlayerController.JumpHeight + (_SpriteHeight / 2f)) * Mathf.Rad2Deg;

                Gizmos.color = Color.blue;
                Vector2 lRotatedDirection = Quaternion.AngleAxis(lAngle, Vector3.forward) * -transform.up;
                Debug.DrawLine(lStart + lRotatedDirection, lStart + lRotatedDirection * 80f);
            }
        }

        private Vector2 GetPosition(float pTime)
        {
            Vector2 lLocalPosition = _GroundedPosition - Vector2.up * (_SpriteHeight / 2f);
            
            return new Vector2(lLocalPosition.x + transform.right.x * pTime,
                               lLocalPosition.y + (transform.up.y * _JumpForce * pTime) + (_Gravity * Mathf.Pow(pTime, 2f) / 2f));
        }

        private void OnDestroy()
        {
            _PlayerController = null;
            _PlayerPhysic = null;
        }

    }
}
