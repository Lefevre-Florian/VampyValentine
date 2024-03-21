using Cinemachine;
using Com.IsartDigital.Platformer.InGameElement.Checkpoint;
using Com.IsartDigital.Platformer.SoundManager;
using Com.IsartDigital.Platformer.InGameElement.Killzone;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Com.IsartDigital.Platformer.Game;



// Author : Dany & Bastien
namespace Com.IsartDigital.Platformer.PlayerBis
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


        [Header("Movement Settings")]
        [Range(1, 10), SerializeField] private float _Acceleration = 5f;
        [Range(1, 50), SerializeField] private float _MaxSpeed = 5f;

        [Header("Dash Settings")]

        [Range(3, 10), SerializeField] private float _DashDistance = 5f;
        public float DashDistance { get { return _DashDistance; } private set { } }

        [Range(0.2f, 1f), SerializeField] private float _DashDuration = 0.5f;
        [Range(0.2f, 1f), SerializeField] public float dashCooldown = 0.5f;

        [Header("Jump Settings")]
        public float jumpHeight = 1f;
        public float JumpHeight { get { return jumpHeight; } private set { } }
        [Range(0f, 5f), SerializeField] private float _JumpCooldown = 1f;
        [NonSerialized] public bool canJump = true;
        [NonSerialized] public bool btnJumpIsHolding;



        [Header("Glide Settings")]

        [SerializeField] private float _GlideDuration = 1f;
        public float GlideDuration { get { return _GlideDuration; } private set { } }

        [Range(0.1f, 10f), SerializeField] private float _GlideCloakDuration = 1f;


        [Header("Physics Settings")]
        public float gravityScale = 10;
        public float fallingMultiplicator = 2;

        #region Variables 
        [NonSerialized] public float verticalDir;
        [NonSerialized] public float horizontalDir;
        [NonSerialized] public float timeTillLastDash = 0f;

        [NonSerialized] public bool isGliding = false;
        [NonSerialized] public bool isControlling = false;

        [NonSerialized] public bool isWallAhead;

        [NonSerialized] public bool playerDied = false;

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

        [NonSerialized] public float elapsedTime;
        private float _Ratio;
        [NonSerialized] public Coroutine timer = null;

        [NonSerialized] public Transform gameContainer = null;
        private PlayerPhysic _PlayerPhysic = null;
        private PlayerAnim _PlayerAnim = null;

        private Action _DoAction = null;

        [NonSerialized] public SpriteRenderer sprite = null;


        [NonSerialized] public bool isDashingUp = false;
        #endregion

        public delegate void OnDeathDelegate();
        public OnDeathDelegate onDeathDelegate = new OnDeathDelegate(() => { });
        [NonSerialized] public Rigidbody2D rb;
        private SpriteRenderer _Sprite => GetComponent<SpriteRenderer>();
        //private float jumpForce => Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale));

        [SerializeField] private float _JumpMultiplicator = -20;
        private float jumpForce => Mathf.Sqrt(jumpHeight * _JumpMultiplicator * Physics2D.gravity.y);


        private float _StockHDir = 0f;

        [Header("Music Settings")]
        [SerializeField] private UpdateMusicParam _UpdateMusicParam_OnDeath = null;
        [SerializeField] private UpdateMusicParam _UpdateMusicParam_OnRevive = null;
        [SerializeField] private float _ResetMusicOnReviveCouldwon = 0.5f;

        private Cinematic _Cinematic = null;

        #region INPUTS

        private void OnInputMove(InputAction.CallbackContext pContext)
        {
            _StockHDir = pContext.ReadValue<float>();
            if (_StockHDir > 0f)
                transform.localScale = new Vector2(1f, 1f);
            else if (_StockHDir < 0f)
                transform.localScale = new Vector2(-1f, 1f);


        }

        private void OnInputJump(InputAction.CallbackContext pContext)
        {
            if (!canJump) return;
            canJump = false;
            verticalDir = pContext.ReadValue<float>();
            StartCoroutine(JumpCoolDown());
            if (pContext.started)
            {
                StartCoroutine(GlideCountdown());
            }
        }

        public IEnumerator JumpCoolDown()
        {
            yield return new WaitForSeconds(_JumpCooldown);
            canJump = true;
            StopCoroutine(JumpCoolDown());
        }

        public IEnumerator GlideCountdown()
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
            if (_Joystick.CheckDeadZone(pValue)) _StockHDir = pValue;
            else _StockHDir = 0;


            if (_StockHDir > 0f)
                transform.localScale = new Vector2(1f, 1f);
            else if (_StockHDir < 0f)
                transform.localScale = new Vector2(-1f, 1f);
        }
#endif
        #endregion



        #region State machine (player logic)
        public void SetModeJump()
        {
            if (isDashing) return;

            _PlayerAnim.PlayPulseAnim(_PlayerPhysic.surfacePosition);
            StartCoroutine(JumpCoroutine());
            _DoAction = DoActionJump;
        }


        private IEnumerator JumpCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            hasJumped = true;
        }
        public void SetModeVoid()
        {
            isGliding = false;
            isDashing = false;
            _DoAction = DoActionVoid;
        }
        private void DoActionVoid()
        {
        }
        public void SetModeDash()
        {
            _DoAction = DoActionDash;
        }
        private void DoActionJump()
        {

            if (_PlayerPhysic.isGrounded)
            {

                _PlayerAnim.PlayJumpAnim();
                _PlayerPhysic.isGrounded = false;
                isGliding = false;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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
            if ((isGliding && _InputManager != null && _InputManager.Main.Jump.IsPressed()) || (isGliding && btnJumpIsHolding))
            {
                _PlayerAnim.PlayGlideAnim();

                float glideSpeed = jumpHeight / _GlideDuration;
                rb.velocity = new Vector2(_MaxSpeed / 2 * transform.right.x * Mathf.Sign(transform.localScale.x), -glideSpeed);

            }
            else
            {

                horizontalDir = 0f;
                isGliding = false;
                isDashing = false;
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().glideActivate);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().glideLoop, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _PlayerAnim.PlayFallAnim();
                _PlayerPhysic.isFalling = true;
                SetModeVoid();
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
                _PlayerAnim.PlayDashScreenShake();
                _Direction = new Vector3(horizontalDir, verticalDir, 0).normalized;

                if (_PlayerPhysic.isGrounded)
                    _PlayerAnim.PlayDustVFX(new Vector2(transform.position.x, _PlayerPhysic.collider.bounds.min.y));
                


#if UNITY_ANDROID
                if (horizontalDir == 0 && _PlayerPhysic.verticalSpeed <= 0 && verticalDir != 0) 
                {
                    if (verticalDir == 0) _Direction.x = transform.localScale.x;
                }
#endif
#if UNITY_STANDALONE
                if (horizontalDir == 0 && _PlayerPhysic.verticalSpeed <= 0 && verticalDir == 0) _Direction.x = transform.localScale.x;

                if (verticalDir >= 0 && !_PlayerPhysic.isGrounded && isWallAhead)
                {
                    _Direction.y = 1f;
                    _Direction.x = 0f;
                    isDashingUp = true;
                }
                else isDashingUp = false;
#endif


#if UNITY_ANDROID
                if (verticalDir >= 0 && isWallAhead && _Direction.x == 0)
                {
                    _Direction.y = 1f;
                    isDashingUp = true;
                }
                else isDashingUp = false;
#endif

                _Direction *= _DashDistance;
                _StartPos = transform.position;
                GetComponent<PlayerSFX>().PlayOnShotSFX(GetComponent<PlayerSFX>().dash);

                _PlayerAnim.PlayArrowVFX(transform.position, _StartPos + _Direction);
                _PlayerAnim.PlayDashAnim(_StartPos + _Direction);

            }

            if (isDashing)
            {
                elapsedTime += Time.deltaTime;
                _Ratio = elapsedTime / _DashDuration;
                transform.position = Vector3.Lerp(_StartPos, _StartPos + _Direction, _Ratio) + posCorrection;
                rb.velocity = new Vector2(0, 0);
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

        private void Awake()
        {

            if (_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;


            rb = GetComponent<Rigidbody2D>();
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
            _InputManager.Main.Down.performed += OnInputDown;
            _InputManager.Main.Down.canceled += OnInputDown;

            _InputManager.Main.Dash.started += OnInputDash;
            _InputManager.Main.DashUp.started += OnInputDashUp;

            if (Cinematic.GetInstance() == null) _InputManager.Main.Enable();
            else
            {
                _Cinematic = Cinematic.GetInstance();
                _Cinematic.onEndCinematic += OnEndCinematic;
            }
#endif


#if UNITY_ANDROID	
            _Joystick = Joystick.Joystick.GetInstance();
            _Joystick.updateMovement += OnInputMove;
#endif

            gameContainer = transform.parent;

            Killzone.resetLevel.AddListener(ResetPlayerOnDeath);
            SetModeVoid();
        }

        private void Update()
        {
            if (GameManager.currentGameState == GameManager.GameState.Pause || GameManager.currentGameState == GameManager.GameState.Cinematic) return;


            if (_DoAction != null)
            {
                _DoAction();

            }
            GravityManager();
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
                }else if(!_InputManager.Main.Jump.IsPressed())
                {
                    verticalDir = 0;
                }
            }
        }

        private void Move()
        {
            horizontalDir = _StockHDir;
            if (isWallAhead)
            {
                horizontalDir = 0;
            }

            if (horizontalDir != 0 && !isGliding && rb.velocity.y == 0)
            {
                _PlayerPhysic.PlaySFXFootStep();
            }
            else if (horizontalDir == 0 || isGliding || rb.velocity.y != 0)
            {
                _PlayerPhysic?.StopSFXFootStep();
            }


            float targetSpeed = horizontalDir * _MaxSpeed;

            float speedDif = targetSpeed - rb.velocity.x;

            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _Acceleration : 20;

            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 0.9f) * Mathf.Sign(speedDif);

            rb.AddForce(movement * Vector2.right);
        }
        private void GravityManager()
        {
            //Normal Gravity
            if (rb.velocity.y >= 0 && !isGliding || !isDashing)
            {
                rb.gravityScale = gravityScale;


            }

            //Falling Gravity
            else if (rb.velocity.y < 0 && !isGliding && !isDashing)
            {
                rb.gravityScale = gravityScale * fallingMultiplicator;
                _PlayerPhysic.isFalling = true;
            }


            //Gliding Gravity
            else if (isGliding)
            {
                rb.gravityScale = 0;


            }

            else if (isDashing)
            {
                rb.gravityScale = 0;

            }
        }

        private void OnEndCinematic() => _InputManager.Enable();
        private void AnimPlayer()
        {


            if (_PlayerPhysic.verticalSpeed < 0.05f && !isDashing && !playerDied)
            {
                _PlayerPhysic.isFalling = true;


                if (_PlayerPhysic.isGrounded == false && (_DoAction != DoActionJump && _DoAction != DoActionGlide))
                {
                    _PlayerAnim.PlayFallAnim();
                }
            }



            if (rb.velocity.y == 0 && horizontalDir != 0 && !isDashing && !playerDied)
            {
                _PlayerAnim.PlayRunAnim();
                _PlayerAnim.PlayRunDustAnim();
            }


            if (rb.velocity.y == 0 && horizontalDir == 0 && _PlayerPhysic.isGrounded && !isDashing && !isControlling && !playerDied)
            {

                _PlayerAnim.PlayIdleAnim();
            }
        }


        public void OnKillZone()
        {
            if (!isDashing && lastCheckpoint != null)
            {
                playerDied = true;
                _UpdateMusicParam_OnDeath?.UpdateMusic();
                _PlayerAnim.PlayDeathMaidAnim();
                onDeathDelegate.Invoke();
                _InputManager?.Main.Disable();
                _PlayerAnim.PlayDeathSmokeVFX();

            }
        }
        private void ResetPlayerOnDeath()
        {
            _InputManager?.Main.Enable();
            StartCoroutine(ResetMusicOnReviveCoulDown());
            playerDied = false;
            transform.position = lastCheckpoint.transform.position;
        }

        private IEnumerator ResetMusicOnReviveCoulDown()
        {
            yield return new WaitForSeconds(_ResetMusicOnReviveCouldwon);
            _UpdateMusicParam_OnRevive?.UpdateMusic();
            StopCoroutine(ResetMusicOnReviveCoulDown());
        }

        public void OnCheckpoint(Checkpoint pCheckpoint)
        {
            lastCheckpoint = pCheckpoint;
        }

        public void StartTimer() => timer = StartCoroutine(nameof(Timer));

        public void StopTimer()
        {
            if (timer != null)
                StopCoroutine(timer);
            GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().burnLoop);
        }

        public IEnumerator Timer()
        {
            GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().burnLoop);
            yield return new WaitForSeconds(_GlideCloakDuration);
            GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().burnLoop);

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
            _InputManager.Main.Down.performed -= OnInputDown;
            _InputManager.Main.Down.canceled -= OnInputDown;

            _InputManager.Main.Dash.started -= OnInputDash;

            if (_Cinematic != null)
                _Cinematic.onEndCinematic -= OnEndCinematic;
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



