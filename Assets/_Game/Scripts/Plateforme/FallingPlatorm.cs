using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using Com.IsartDigital.Platformer.Utils;
using Com.IsartDigital.Platformer.InGameElement.Killzone;
using Cinemachine;
using DG.Tweening;
using Com.IsartDigital.Platformer.SoundManager;


//Author : Dany

namespace Com.IsartDigital.Platformer
{
    public class FallingPlatorm : MonoBehaviour, IResetableObject
    {
        private const float UPPER_CHECK = 0.5f;
        private const float FALL_SPEED_MIN = 0.01f;

        [RangeAttribute(1, 5), SerializeField] private float _Acceleration = 3f;
        [RangeAttribute(1, 100), SerializeField] private float _InitialFallSpeed = 30f;
        [RangeAttribute(1, 100), SerializeField] private float _MaxSpeed = 40f;

        [RangeAttribute(0.2f, 3), SerializeField] private float _TimeToFall = 1;
        [SerializeField] private LayerMask _GroundLayer;
        [SerializeField] private LayerMask _PlayerLayer;
        [RangeAttribute(0.1f, 2), SerializeField] private float _RaycastDistance = 0.1f; //Adjust depending the sprite

        [SerializeField] private float _TimeToRespawn = 3f;
        
        private bool _IsFalling = false;
        private float currentFallSpeed;
        public Vector3 initialPosition { get; private set; }

        private SpriteRenderer _Sprite = default;
        private BoxCollider2D _BoxCollider = default;

        private Coroutine _Timer = null;
        private Coroutine _RespawnTimer = null;

        private CinemachineImpulseSource _Impulse => GetComponent<CinemachineImpulseSource>();

        [Header("Anim")]
        [SerializeField] private GameObject _SmokeAnim;
        [SerializeField] private float _Strength = 0.1f;

        [SerializeField] private ScriptSFX _FallSFX = null;
        [SerializeField] private ScriptSFX _QuakeSFX = null;


        private void Start()
        {
            _Sprite = GetComponentInChildren<SpriteRenderer>();
            _BoxCollider = GetComponent<BoxCollider2D>();

            initialPosition = transform.position;
            currentFallSpeed = _InitialFallSpeed;

            Killzone.resetLevel.AddListener(ResetObject);
        }

        public void ResetObject()
        {
            if(_Timer != null)
            {
                StopCoroutine(_Timer);
                _Timer = null;
            }

            currentFallSpeed = _InitialFallSpeed;
            transform.position = initialPosition;

            _IsFalling = false;
            _Sprite.enabled = true;
            _BoxCollider.enabled = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Check if the colliding object is a player and if the player is above
            if (((1 << collision.gameObject.layer) & _PlayerLayer ) != 0
                && Physics2D.BoxCast(transform.position, _BoxCollider.size, 0f, Vector2.up, UPPER_CHECK, _PlayerLayer)
                && _Timer == null)
            {
                _Timer = StartCoroutine(Countdown());
            }
        }

        private IEnumerator Countdown()
        {
            _Sprite.transform.DOShakePosition(_TimeToFall, _Strength).SetEase(Ease.OutQuad);
            _QuakeSFX?.PlaySFX();

            yield return new WaitForSeconds(_TimeToFall);

            _FallSFX?.PlaySFX();
            _IsFalling = true;

            if (_Timer != null)
                StopCoroutine(_Timer);

            if (_RespawnTimer != null)
                StopCoroutine(_RespawnTimer);
            
            _Timer = null;
            _RespawnTimer = null;
        }

        private void Update()
        {
            if (_IsFalling)
            {
                float lAccelerationAmount = (currentFallSpeed > FALL_SPEED_MIN) ? _Acceleration : 0f;

                currentFallSpeed = Mathf.Clamp(currentFallSpeed + lAccelerationAmount * Time.deltaTime, 0f, _MaxSpeed);
                transform.Translate(Vector3.down* currentFallSpeed * Time.deltaTime);

                RaycastHit2D lHit = Physics2D.Raycast(transform.position + Vector3.down * (_BoxCollider.size.y / 2f + (currentFallSpeed * 2f) * Time.deltaTime), 
                                                      Vector2.down, 
                                                      _RaycastDistance, 
                                                      _GroundLayer);
                if (lHit.collider != null && lHit.collider.gameObject != gameObject)
                    DestroyObject();
            }
        }

        private void DestroyObject()
        {
            _IsFalling = false;
            _Sprite.enabled = false;
            _BoxCollider.enabled = false;
            
            _RespawnTimer  = StartCoroutine(RespawnCoroutine());
            

            _Impulse.GenerateImpulseWithForce(0.2f);
        }

        

        private IEnumerator RespawnCoroutine()
        {

            yield return new WaitForSeconds(_TimeToRespawn);
            Instantiate(_SmokeAnim, initialPosition, Quaternion.identity);

            yield return new WaitForSeconds(0.4f);
            ResetObject();

        }


        private void OnDestroy()
        {
            Killzone.resetLevel.RemoveListener(ResetObject);

            if (_Timer != null)
                StopCoroutine(_Timer);

            if (_RespawnTimer != null)
                StopCoroutine(_RespawnTimer);

            _Timer = null;
            _RespawnTimer = null;
        }
    }
}