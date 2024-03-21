using Com.IsartDigital.Platformer.Game;
using Com.IsartDigital.Platformer.Light;
using Com.IsartDigital.Platformer.SoundManager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

// Author : Dany & Renaudin Matteo
namespace Com.IsartDigital.Platformer.Player
{
    public class PlayerPhysic : MonoBehaviour
    {
        [Header("Collision Settings")]
        [SerializeField] private LayerMask _GameElementLayer = default;
        [SerializeField] private LayerMask _KillZoneLayer = default;
        [SerializeField] private LayerMask _GroundLayerMask = default;
        [SerializeField] private LayerMask _LightLayerMask = default;

        [SerializeField] private ContactFilter2D _Filter = default;

        [Header("Export Settings")]
        [SerializeField] private Transform _GroundRayCastPoint;
        [SerializeField] private Transform _FrontRayCastPoint;
        [SerializeField] private Transform _CeilingRayCastPoint;

        [Header("Variable Settings")]
        [Range(10f, 200f)] private float _GlideProtectAngle = 180f;
        [SerializeField] private float _FallForce = 2f;
        public float FallForce { get { return _FallForce; } private set { } }

        private RaycastHit2D _HitTop;
        private RaycastHit2D _HitGround;
        private RaycastHit2D _Hit;
        private Vector2 lCastOrigin;
        private Vector2 lCastOriginGround;

        [NonSerialized] public float verticalSpeed;
        [NonSerialized] public bool isGrounded;
        [NonSerialized] public bool isFalling;

        [SerializeField] private float _RecoilOnCeiling = 0.3f;
        [NonSerialized] public bool isTraversable = false;

        public bool IsGrounded { get { return isGrounded; } }

        private PlayerController _PlayerController = null;
        private PlayerAnim _PlayerAnim = null;

        [NonSerialized] public Vector2 surfacePosition;
        private Collider2D[] _Results = new Collider2D[1];

        private void Awake()
        {
            _PlayerController = GetComponent<PlayerController>();
            _PlayerAnim = GetComponent<PlayerAnim>();
        }

        private void Update()
        {
            CheckCeiling();
            CheckMidHorizontal();
            CheckGround();
        }

        //private void OnTriggerEnter2D(Collider2D pCollision)
        //{
        //    if (((1 << pCollision.gameObject.layer) & _GameElementLayer) != 0 && !_PlayerController.isTp)
        //    {
        //        pCollision.gameObject.GetComponent<InGameElement.GameElement>().Effect(_PlayerController);
        //    }

        //    if (((1 << pCollision.gameObject.layer) & _LightLayerMask) != 0 && !_PlayerController.isTp)
        //    {
        //        SpriteLight2D lLight = pCollision.GetComponent<SpriteLight2D>();
        //        Vector2 lNormal = ((Vector2)transform.position - lLight.EmissionPoint).normalized;
        //        if (Vector3.Angle(transform.up, lNormal) < _GlideProtectAngle / 2f)
        //            _PlayerController.StartTimer();
        //        else
        //        {
        //            //burn
        //            GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().burnDie);
        //            transform.parent = _PlayerController.gameContainer;
        //            lLight.Effect(_PlayerController);
        //        }
                    
        //    }

        //    if (pCollision.GetComponent<AutomaticPlatform>() || pCollision.GetComponent<MobilePlatform>())
        //        transform.parent = pCollision.transform;


        //}
        //private void OnTriggerStay2D(Collider2D pCollision)
        //{
        //    if (((1 << pCollision.gameObject.layer) & _KillZoneLayer) != 0 && !_PlayerController.isTp && !_PlayerController.isDashing)
        //    {
        //        pCollision.gameObject.GetComponent<InGameElement.GameElement>().Effect(_PlayerController);
        //        GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().die);
        //        //die
        //    }

        //    if (((1 << pCollision.gameObject.layer) & _LightLayerMask) != 0
        //       && !_PlayerController.isGliding  && !_PlayerController.isTp)
        //    {
        //        //burn
        //        GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().burnDie);
        //        transform.parent = _PlayerController.gameContainer;
        //        pCollision.GetComponent<SpriteLight2D>().Effect(_PlayerController);
        //    }
        //}

        //private void OnTriggerExit2D(Collider2D pCollision)
        //{
        //    if (_PlayerController.isTp) _PlayerController.isTp = false;

        //    if (((1 << pCollision.gameObject.layer) & _LightLayerMask) != 0
        //        && _PlayerController.timer != null)
        //        _PlayerController.StopTimer();

        //    if (pCollision.GetComponent<AutomaticPlatform>() || pCollision.GetComponent<MobilePlatform>())
        //        transform.parent = (_PlayerController != null) ? _PlayerController.gameContainer : null;
            
        //}

        private void CheckGround()
        {

            Vector2 lSize = new Vector2(transform.localScale.x, transform.localScale.y);
            if (Physics2D.OverlapBox(_GroundRayCastPoint.position, lSize, 0, _Filter, _Results) > 0)
            {
                isTraversable = false;
                isGrounded = true;
                isFalling = false;
                verticalSpeed = 0;
                _PlayerController.isJump = false; 
                surfacePosition = Physics2D.ClosestPoint(_GroundRayCastPoint.position, _Results[0]);
                transform.position = new Vector3(transform.position.x, surfacePosition.y + _PlayerController.sprite.size.y / 2, transform.position.z);
                if (_PlayerController.hasJumped)
                {
                    //GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().land);
                    GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().jump);

                    _PlayerAnim.PlayLandingAnim(surfacePosition);
                    _PlayerController.hasJumped = false;
                    _PlayerController.SetModeVoid();

                    _PlayerController.timeTillLastDash = _PlayerController.dashCooldown;
                }
            }
            else
            {
                if (_PlayerController.isGliding && verticalSpeed < 0f)
                    verticalSpeed = _PlayerController.glideForce;

                isGrounded = false;
            }
        }

        public void CheckMidHorizontal()
        {
            lCastOrigin = _FrontRayCastPoint.position - new Vector3(0, 0.2f, 0);
            lCastOriginGround = _FrontRayCastPoint.position - Vector3.up;

            _HitTop = Physics2D.BoxCast(lCastOrigin, new Vector2(0.05f, 0.95f), 0, new Vector2(0, 0.01f), 0, _GroundLayerMask);
            _HitGround = Physics2D.BoxCast(lCastOriginGround, new Vector2(0.05f, 0.95f), 0, new Vector2(0, 0.01f), 0, _GroundLayerMask);

            if (_HitTop.collider && _HitGround.collider)
            {
                _PlayerController.posCorrection = Vector3.zero;
                _PlayerController.isWallAhead = true;
                if (!_PlayerController.isDashingUp)
                {
                    _PlayerController.elapsedTime = 0;
                    _PlayerController.isDashing = false;
                }
            }
            else if (_HitGround.collider && !_HitTop.collider)
            {
                RaycastHit2D _HitReplacement = Physics2D.Raycast(_FrontRayCastPoint.position, Vector2.down, 0.1f, _GroundLayerMask);
                _PlayerController.posCorrection = new Vector3(0, _HitReplacement.point.y + 0.73f, 0);
                _PlayerController.isWallAhead = true;
            }
            else
            {
                _PlayerController.isWallAhead = false;
            }
        }

        public void CheckCeiling()
        {
            if (Physics2D.Raycast(_CeilingRayCastPoint.position, Vector2.up, 0.1f, _GroundLayerMask))
            {
                verticalSpeed = -_RecoilOnCeiling;
                _PlayerController.isDashing = false;
                _PlayerController.elapsedTime = 0;
                _PlayerController.SetModeVoid();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector2 castOrigin = _FrontRayCastPoint.position - new Vector3(0, 0.75f);
            Vector2 boxSize = new Vector2(0.05f, 1.5f);

            lCastOrigin = _FrontRayCastPoint.position - new Vector3(0, 0.2f, 0);
            lCastOriginGround = _FrontRayCastPoint.position - (Vector3.up * transform.localScale.y) - new Vector3(0, 0.18f, 0);

            _HitTop = Physics2D.Raycast(lCastOrigin, Vector2.right, 0.05f, _GroundLayerMask);
            _HitGround = Physics2D.Raycast(lCastOriginGround, Vector2.right, 0.05f, _GroundLayerMask);

            Gizmos.DrawWireCube(lCastOriginGround, new Vector2(0.05f, 0.95f * transform.localScale.y));

            Gizmos.color = Color.green;

            Gizmos.DrawWireCube(lCastOrigin, new Vector2(0.05f, 0.95f * transform.localScale.y));

            Vector2 lSize = new Vector2(transform.localScale.x, transform.localScale.y);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_GroundRayCastPoint.position, lSize);
        }
    }
}
