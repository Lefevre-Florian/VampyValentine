using Cinemachine;
using Com.IsartDigital.Platformer.InGameElement.Checkpoint;
using Com.IsartDigital.Platformer.SoundManager;
using Com.IsartDigital.Platformer.InGameElement.Killzone;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Author : Dany & Bastien
namespace Com.IsartDigital.Platformer.Player
{
    [RequireComponent(typeof(PlayerPhysic))]
    public class PlayerController : MonoBehaviour
    {
        #region Singleton
        private static PlayerController _Instance = null;

        public static PlayerController GetInstance()
        {
            if (_Instance == null) _Instance = new PlayerController();
            return _Instance;
        }

        private PlayerController() : base() { }
        #endregion

        [Header("Export Settings")]
        public Checkpoint lastCheckpoint;

        #region Adjustable Variables
        [Header("Variable Settings")]
        [Range(3, 10), SerializeField] private float _DashDisance = 5f;
        [Range(0.2f, 1f), SerializeField] private float _DashDuration = 0.5f;
        [Range(0.2f, 1f), SerializeField] public float dashCooldown = 0.5f;

        [Header("Movement Settings")]
        [Range(1, 10), SerializeField] private float _Acceleration = 5f;
        [Range(1, 10), SerializeField] private float _MaxSpeed = 5f;
        [SerializeField] private float _DashDistance = 3f;
        public float DashDistance { get { return _DashDistance; } private set { } }
        [SerializeField] private float _JumpDuration = 1f;
        public float JumpDuration { get { return _JumpDuration; } private set { } }
        public float jumpHeight = 1f;
        public float JumpHeight { get { return jumpHeight; } private set { } }
        [SerializeField] private float _GlideDuration = 1f;
        public float GlideDuration { get { return _GlideDuration; } private set { } }

        [Range(0.1f, 10f), SerializeField] private float _GlideCloakDuration = 1f;

        public delegate void OnDeathDelegate();
        public OnDeathDelegate onDeathDelegate = new OnDeathDelegate(() => { });

        #endregion

        #region Variables 
        private Vector3 _Movement;
        private float _HorizontalSpeed;
        public float verticalDir;
        public float horizontalDir;
        public float timeTillLastDash = 0f;

        [NonSerialized] public bool isGliding = false;

        [NonSerialized] public bool isWallAhead; //Utile pour le Dash

        [NonSerialized] public bool isDashButtonPressed = false;

        // References 
        private InputManager _InputManager = null;
        private Joystick.Joystick _Joystick = null;

        private Vector3 _Direction;
        private Vector3 _StartPos;

        [NonSerialized] public Vector3 posCorrection;
        [NonSerialized] public bool isDashing;
        [NonSerialized] public bool isDown;
        [NonSerialized] public bool hasJumped;

        [NonSerialized] public bool isTp = false;

        [NonSerialized] public float gravity = -9.8f;
        [NonSerialized] public float glideForce = 0f;

        [NonSerialized] public float elapsedTime;
        private float _Ratio;
        [NonSerialized] public Coroutine timer = null;

        [NonSerialized] public Transform gameContainer = null;
        private PlayerPhysic _PlayerPhysic = null;
        private PlayerAnim _PlayerAnim = null;
        private Action _DoAction = null;

        [NonSerialized] public SpriteRenderer sprite = null;

        public bool isJump = false;

        [NonSerialized] public bool isDashingUp = false; 
        #endregion



        private void Awake()
        {

            if (_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;
            _PlayerPhysic = GetComponent<PlayerPhysic>();
            _PlayerAnim = GetComponent<PlayerAnim>();
            sprite = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
#if UNITY_STANDALONE
            // Initialize input system 
            _InputManager = new InputManager();
            _InputManager.Main.Move.started += OnInputMove;
            _InputManager.Main.Move.canceled += OnInputMove;
            _InputManager.Main.Move.performed += OnInputMove;

            _InputManager.Main.Jump.started += OnInputJump;
            _InputManager.Main.Jump.canceled += OnInputJump;


            _InputManager.Main.Down.started += OnInputDown;
            _InputManager.Main.Down.canceled += OnInputDown;

            _InputManager.Main.Dash.started += OnInputDash;
            _InputManager.Main.DashUp.started += OnInputDashUp;
            _InputManager.Main.Enable();
            #endif

            // Glide force computation 
            glideForce = -2 * jumpHeight / Mathf.Pow(_GlideDuration, 2f);

#if UNITY_ANDROID	
            _Joystick = Joystick.Joystick.GetInstance();
            _Joystick.updateMovement += OnInputMove;
#endif

            gameContainer = transform.parent;

            Killzone.resetLevel.AddListener(ResetPlayerOnDeath);
            SetModeVoid();
        }


        private void FixedUpdate()
        {
            if (GameManager.currentGameState == GameManager.GameState.Pause) return;
            if (_DoAction != null)
            {
                _DoAction();

            }
            Move();
            AnimPlayer();

            if (!isDashing && _PlayerPhysic.isGrounded)
            {
                timeTillLastDash += Time.deltaTime;
            }

            if (_InputManager != null)
            {
                if (_InputManager.Main.DashUp.IsPressed())
                {
                    verticalDir = _InputManager.Main.DashUp.ReadValue<float>();
                }
            }
            
        }



        #region Inputs

        private void OnInputMove(InputAction.CallbackContext pContext)
        {
            _HorizontalSpeed = pContext.ReadValue<float>();
            horizontalDir = _HorizontalSpeed;
            if (_HorizontalSpeed > 0f)
                transform.localScale = new Vector2(1f, 1f);
            else if (_HorizontalSpeed < 0f)
                transform.localScale = new Vector2(-1f, 1f);
        }

        private void OnInputJump(InputAction.CallbackContext pContext)
        {
            verticalDir = pContext.ReadValue<float>();
            if (pContext.started)
            {
                StartCoroutine(GlideCountdown());
            }
        }

        private IEnumerator GlideCountdown()
        {
            if (_PlayerPhysic.isGrounded)
            {
                SetModeJump();
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                SetModeGlide();
                isGliding = true;
            }
        }

        private void OnInputDown(InputAction.CallbackContext pContext)
        {
            isDown = pContext.ReadValueAsButton();
        }

        private void OnInputDash(InputAction.CallbackContext pContext)
        {
            isDashButtonPressed = pContext.ReadValueAsButton();
            SetModeDash();
        }

        private void OnInputDashUp(InputAction.CallbackContext pContext)
        {

        }




#if UNITY_ANDROID
        private void OnInputMove(float pValue)
        {
            if (_Joystick.CheckDeadZone(pValue)) _HorizontalSpeed = pValue;
            else _HorizontalSpeed = 0;

            horizontalDir = _HorizontalSpeed;

            if (_HorizontalSpeed > 0f)
                transform.localScale = new Vector2(1f, 1f);
            else if (_HorizontalSpeed < 0f)
                transform.localScale = new Vector2(-1f, 1f);
        }
#endif
        #endregion

        #region State machine (player logic)
        public void SetModeJump()
        {
            _PlayerAnim.PlayPulseAnim(_PlayerPhysic.surfacePosition);
            StartCoroutine(JumpCoroutine());
            _DoAction = DoActionJump;
        }


        private IEnumerator JumpCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            hasJumped = true;
        }
        public void SetModeVoid()
        {
            _DoAction = DoActionVoid;
        }
        private void DoActionVoid()
        {
        }
        public void SetModeDash()
        {
            if (_PlayerPhysic.isGrounded)
            {
                _PlayerAnim.PlayDustVFX(_PlayerPhysic.surfacePosition);
            }

            _DoAction = DoActionDash;
        }
        private void DoActionJump()
        {

            if (_PlayerPhysic.isGrounded)
            {
                _PlayerAnim.PlayJumpAnim();
                _PlayerPhysic.isGrounded = false;
                isGliding = false;
                isJump = true;
                _PlayerPhysic.verticalSpeed = (jumpHeight * sprite.size.y);

                GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().jump);
                //GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().land);

                if (_PlayerPhysic.isFalling)
                    _PlayerAnim.PlayFallAnim();
            }



        }

        private void SetModeGlide()
        {
            isGliding = true;
            _DoAction = DoActionGlide;
            GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().glideActivate);
            GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().glideLoop);
        }

        private void DoActionGlide()
        {
            if (isGliding && _InputManager.Main.Jump.IsPressed())
            {    
                _PlayerAnim.PlayGlideAnim();
                _HorizontalSpeed = 2.5f * transform.right.x * Mathf.Sign(transform.localScale.x);

            }
            else
            {
                _HorizontalSpeed = 0f;
                isGliding = false;
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().glideActivate);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().glideLoop, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _PlayerAnim.PlayFallAnim();
                _PlayerPhysic.isFalling = true;
                SetModeVoid();
            }
        }

        private void Move()
        {
            if (isWallAhead)
                _HorizontalSpeed = 0f;

            float lAccelerationAmount = (_HorizontalSpeed > 0.01f) ? _Acceleration : (_HorizontalSpeed < -0.01f) ? -_Acceleration : 0f;

            _HorizontalSpeed = Mathf.Clamp(_HorizontalSpeed + lAccelerationAmount * Time.deltaTime, -_MaxSpeed, _MaxSpeed);

            if (_PlayerPhysic.isGrounded)
                _PlayerPhysic.verticalSpeed = 0f;

            else
                _PlayerPhysic.verticalSpeed += gravity * Time.deltaTime;
/*
            if (_HorizontalSpeed != 0 && !isGliding && _PlayerPhysic.verticalSpeed == 0) GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().footstep);
            else if (_HorizontalSpeed == 0 || isGliding  || _PlayerPhysic.verticalSpeed != 0) GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstep);
*/
            _Movement = new Vector2(_HorizontalSpeed, _PlayerPhysic.verticalSpeed);
            transform.position = Vector3.Lerp(transform.position, transform.position + _Movement, _MaxSpeed * Time.deltaTime);




        }




        private void AnimPlayer()
        {


            if (_PlayerPhysic.verticalSpeed < 0.05f)
            {
                _PlayerPhysic.isFalling = true;


                if (_PlayerPhysic.isGrounded == false && (_DoAction != DoActionJump && _DoAction != DoActionGlide))
                {
                    _PlayerAnim.PlayFallAnim();
                }
            }



            if (_PlayerPhysic.verticalSpeed == 0 && _HorizontalSpeed != 0)
            {
                _PlayerAnim.PlayRunAnim();
                //_PlayerAnim.PlayRunDustAnim();
            }


            if (_PlayerPhysic.verticalSpeed == 0 && _HorizontalSpeed == 0 && _PlayerPhysic.isGrounded)
            {

                _PlayerAnim.PlayIdleAnim();
            }
        }

        private void DoActionDash()
        {
            if (isDashButtonPressed && !isDashing && (timeTillLastDash > dashCooldown))
            {
                isDashing = true;
                timeTillLastDash = 0;
                _PlayerPhysic.CheckMidHorizontal();
                isDashButtonPressed = false;
                _Direction = new Vector3(horizontalDir, verticalDir, 0).normalized;
#if UNITY_ANDROID
                if (horizontalDir == 0 && _PlayerPhysic.verticalSpeed <= 0 && verticalDir != 0) 
                {
                    if (verticalDir == 0) _Direction.x = transform.localScale.x;
                }
#endif
#if UNITY_STANDALONE
                if (horizontalDir == 0 && _PlayerPhysic.verticalSpeed <= 0 && verticalDir == 0) _Direction.x = transform.localScale.x;
#endif
                if (verticalDir >= 0 && _PlayerPhysic.verticalSpeed > 0)
                {
                    _Direction.y = 1f;
                    isDashingUp = true;
                }else isDashingUp = false;


                _Direction *= _DashDisance;
                _StartPos = transform.position;
                GetComponent<PlayerSFX>().PlayOnShotSFX(GetComponent<PlayerSFX>().dash);

                _PlayerAnim.PlayArrowVFX(transform.position, _StartPos + _Direction);
            }

            if (isDashing)
            {
                elapsedTime += Time.deltaTime;
                _Ratio = elapsedTime / _DashDuration;
                transform.position = Vector3.Lerp(_StartPos, _StartPos + _Direction, _Ratio) + posCorrection;
                if (_Ratio >= 1)
                {
                    posCorrection = Vector3.zero;
                    isDashing = false;
                    elapsedTime = 0;
                    //GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().dash, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
            }
            else if (isWallAhead && !isDashingUp)
            {
                isDashing = false;
                elapsedTime = 0;
                //GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().dash, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                SetModeVoid();
            }
            else if (!isDashButtonPressed && !isDashing) 
            {
                SetModeVoid();
            }
        }
#endregion

        public void OnKillZone()
        {
            if (!isDashing && lastCheckpoint != null)
            {
                onDeathDelegate.Invoke();
            }
        }
        private void ResetPlayerOnDeath()
        {
            transform.position = lastCheckpoint.transform.position;
            isTp = true;
        }

        public void OnCheckpoint(Checkpoint pCheckpoint)
        {
            if (!isTp) lastCheckpoint = pCheckpoint;
        }

        public void StartTimer() => timer = StartCoroutine(nameof(Timer));

        public void StopTimer()
        {
            if (timer != null)
                StopCoroutine(timer);
        }

        public IEnumerator Timer()
        {
            yield return new WaitForSeconds(_GlideCloakDuration);

            isGliding = false;
            if (timer != null)
                StopCoroutine(timer);
        }
#if UNITY_STANDALONE
        private void OnDisable() => _InputManager.Main.Disable();
#endif

        private void OnDestroy()
        {
#if UNITY_STANDALONE
            // Clean input system connection before destroying the object 
            _InputManager.Main.Move.started -= OnInputMove;
            _InputManager.Main.Move.canceled -= OnInputMove;
            _InputManager.Main.Move.performed -= OnInputMove;

            _InputManager.Main.Jump.started -= OnInputJump;
            _InputManager.Main.Jump.canceled -= OnInputJump;

            _InputManager.Main.Down.started -= OnInputDown;
            _InputManager.Main.Down.canceled -= OnInputDown;

            _InputManager.Main.Dash.started -= OnInputDash;
#endif
#if UNITY_ANDROID
            _Joystick.updateMovement -= OnInputMove;
#endif

            Killzone.resetLevel.RemoveListener(ResetPlayerOnDeath);
            if (_Instance != null)
                _Instance = null;
        }

    }
}