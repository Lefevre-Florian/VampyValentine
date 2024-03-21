using Com.IsartDigital.Platformer.Game;
using Com.IsartDigital.Platformer.InGameElement;
using Com.IsartDigital.Platformer.InGameElement.Killzone;
using Com.IsartDigital.Platformer.Light;
using Com.IsartDigital.Platformer.SoundManager;

using FMODUnity;
using System;

using UnityEngine;

// Author : Dany & Renaudin Matteo
namespace Com.IsartDigital.Platformer.PlayerBis
{
    public class PlayerPhysic : MonoBehaviour
    {
        private string CURTAIN_TAG = "Curtain";

        [Header("Collision Settings")]
        [SerializeField] private LayerMask _GameElementLayer = default;
        [SerializeField] private LayerMask _KillZoneLayer = default;
        [SerializeField] private LayerMask _GroundLayerMask = default;
        [SerializeField] private LayerMask _RealGroundLayerMask = default;
        [SerializeField] private LayerMask _LightLayerMask = default;
        [SerializeField] private LayerMask _PlatformTraversableLayerMask = default;

        [SerializeField] private string _VoidTag = "Void";
        [SerializeField] private string _MaidTag = "Maid";
        [SerializeField] private string _GarlicTag = "Garlic";

        [SerializeField] private string _WoodTag = "Wood";
        [SerializeField] private string _StoneTag = "Stone";
        [SerializeField] private string _LustreTag = "Lustre";
        [SerializeField] private string _LanternTag = "Lantern";

        [Header("Export Settings")]
        [SerializeField] private Transform _GroundRayCastPoint;
        [SerializeField] private Transform _FrontRayCastPoint;
        [SerializeField] private Transform _CeilingRayCastPoint;

        [Header("Variable Settings")]
        [SerializeField][Range(10f, 200f)] private float _GlideProtectAngle = 180f;
        [SerializeField] private float _FallForce = 2f;
        public float FallForce { get { return _FallForce; } private set { } }

        private RaycastHit2D _HitTop;
        private RaycastHit2D _HitGround;
        private RaycastHit2D _HitKill;
        private RaycastHit2D _Hit;
        private Vector2 lCastOrigin;
        private Vector2 lCastOriginGround;

        [NonSerialized] public float verticalSpeed;
        [NonSerialized] public bool isGrounded;
        [NonSerialized] public bool isFalling;

        [NonSerialized] public CapsuleCollider2D collider;

        [SerializeField] private float _RecoilOnCeiling = 0.3f;
        [NonSerialized] public bool isTraversable = false;

        public bool IsGrounded { get { return isGrounded; } }

        private PlayerController _PlayerController = null;
        private PlayerAnim _PlayerAnim = null;

        [NonSerialized] public Vector2 surfacePosition;
        private Collider2D[] _Results = new Collider2D[1];

        [SerializeField] private ParamRef _ResetMusicParam;

        [SerializeField] private ParamRef _SurfaceWood;
        [SerializeField] private ParamRef _SurfaceStone;
        [SerializeField] private ParamRef _SurfaceLantern;
        [SerializeField] private ParamRef _SurfaceLustre;

        private GameObject _Surface = null;
        void Start()
        {
            _PlayerController = GetComponent<PlayerController>();
            _PlayerAnim = GetComponent<PlayerAnim>();   
            collider = GetComponent<CapsuleCollider2D>();

        }

        // Update is called once per frame
        void Update()
        {
            CheckGround();
            CheckMidHorizontal();
            CheckPlatform();
            CheckCeiling();
        }

        private void CheckGround()
        {
            Vector2 lSize = new Vector2(1, 0.2f);
            
            RaycastHit2D hit = Physics2D.BoxCast(new Vector2(collider.bounds.center.x, collider.bounds.min.y), lSize, 0f, Vector2.zero, 0f, _GroundLayerMask);

            if (hit.collider != null)
            {
                _Surface = hit.collider.gameObject;
                isTraversable = false;
                isGrounded = true;
                isFalling = false;
                _PlayerController.isGliding = false;
                surfacePosition = hit.collider.ClosestPoint(hit.point);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().glideActivate);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().glideLoop, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);


                if (_PlayerController.hasJumped)
                {

                    if (hit.collider.gameObject.CompareTag(_StoneTag))
                    {
                        GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().land, _SurfaceStone);
                    }
                    else if (hit.collider.gameObject.CompareTag(_LustreTag))
                    {
                        GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().land, _SurfaceLustre);
                    }
                    else if (hit.collider.gameObject.CompareTag(_LanternTag))
                    {
                        GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().land, _SurfaceLantern);
                    }
                    else
                        GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().land, _SurfaceWood);

                    //PlaySFXLand();

                    GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().jump);


                    _PlayerController.rb.velocity = new Vector2(_PlayerController.rb.velocity.x, 0);

                    _PlayerAnim.PlayLandingAnim(surfacePosition);
                    _PlayerController.hasJumped = false;
                    _PlayerController.SetModeVoid();
                    _PlayerController.timeTillLastDash = _PlayerController.dashCooldown;
                }
            }
            else
            {
                isGrounded = false;
            }
        }

        private void CheckPlatform()
        {
            Vector2 lSize = new Vector2(collider.bounds.size.x, 0.2f);

            RaycastHit2D hit = Physics2D.BoxCast(new Vector2(collider.bounds.center.x, collider.bounds.min.y), lSize, 0f, Vector2.zero, 0f, _PlatformTraversableLayerMask);

            if (hit.collider != null)
            {
                hit.collider.gameObject.GetComponent<PlateformeTraversable>().isDown = PlayerController.GetInstance().isDown;
                surfacePosition = hit.collider.ClosestPoint(hit.point);
            }
        }

        private void CheckCeiling()
        {
            RaycastHit2D lHit = Physics2D.Raycast(_CeilingRayCastPoint.transform.position, Vector2.up,0.1f,_RealGroundLayerMask);
            if(lHit.collider != null)
            {
                ResetDash();
            }
        }

        public void CheckMidHorizontal()
        {
            lCastOrigin = _FrontRayCastPoint.position - new Vector3(0, 0.2f, 0);
            lCastOriginGround = _FrontRayCastPoint.position - Vector3.up;

            _HitTop = Physics2D.BoxCast(lCastOrigin, new Vector2(0.05f, 0.95f), 0, new Vector2(0, 0.01f), 0, _GroundLayerMask);
            _HitGround = Physics2D.BoxCast(lCastOriginGround, new Vector2(0.05f, 0.95f), 0, new Vector2(0, 0.01f), 0, _GroundLayerMask);

            _HitKill = Physics2D.Raycast(_FrontRayCastPoint.position,Vector2.down * 2,0.5f,_KillZoneLayer);

            if (_HitKill.collider != null)
            {
                ResetDash();
            }

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

        private void OnTriggerEnter2D(Collider2D pCollision)
        {
            if (_PlayerController.playerDied)
                return;

            // Game elements (curtain / killzones / collectibles)
            if (((1 << pCollision.gameObject.layer) & _GameElementLayer) != 0)
                pCollision.gameObject.GetComponent<GameElement>().Effect(_PlayerController);

            // Light
            if (((1 << pCollision.gameObject.layer) & _LightLayerMask) != 0 && _PlayerController.isGliding)
            {
                SpriteLight2D lLight = pCollision.GetComponent<SpriteLight2D>();
                if (lLight == null)
                    return;

                Vector2 lNormal = ((Vector2)transform.position - lLight.EmissionPoint).normalized;
                if (Vector3.Angle(transform.up, lNormal) < _GlideProtectAngle / 2f)
                {
                    _PlayerController.StartTimer();
                }
                else
                {
                    //burn
                    GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().dieBurn);
                    transform.parent = _PlayerController.gameContainer;
                    lLight.Effect(_PlayerController);
                }

            }
        }
    
        private void ResetDash()
        {
            _PlayerController.posCorrection = Vector3.zero;
            _PlayerController.isWallAhead = true;
            _PlayerController.isDashing = false;
            _PlayerController.elapsedTime = 0;
        }

        private void OnTriggerStay2D(Collider2D pCollision)
        {
            if (_PlayerController.playerDied)
                return;

            // Killzones
            if (((1 << pCollision.gameObject.layer) & _KillZoneLayer) != 0 && !_PlayerController.isDashing)
            {
                ResetDash();
                pCollision.gameObject.GetComponent<GameElement>()?.Effect(_PlayerController);

                if (pCollision.gameObject.CompareTag(_MaidTag))
                {
                    GetComponent<PlayerSFX>().PlayOnShotSFX(GetComponent<PlayerSFX>().dieMaid);
                    SoundManager.SoundManager.GetInstance().UpdateMusic(_ResetMusicParam);
                }
                else if (pCollision.gameObject.CompareTag(_VoidTag))
                    GetComponent<PlayerSFX>().PlayOnShotSFX(GetComponent<PlayerSFX>().dieVoid);
                else if (pCollision.gameObject.CompareTag(_GarlicTag))
                    GetComponent<PlayerSFX>().PlayOnShotSFX(GetComponent<PlayerSFX>().dieGarlic);
            }

            // Light
            if (((1 << pCollision.gameObject.layer) & _LightLayerMask) != 0
               && !_PlayerController.isGliding
               && !_PlayerController.isDashing)
            {
                ResetDash();
                //burn
                transform.parent = _PlayerController.gameContainer;
                GetComponent<PlayerSFX>().PlayOnShotSFX(GetComponent<PlayerSFX>().dieBurn);
                pCollision.GetComponent<Killzone>().Effect(_PlayerController);
            }

            // Game elements (curtain / killzones / collectibles)
            if (((1 << pCollision.gameObject.layer) & _GameElementLayer) != 0)
                pCollision.gameObject.GetComponent<GameElement>().Effect(_PlayerController);
        }

        public void PlaySFXFootStep()
        {
            if (_Surface == null) return;

            if (_Surface.CompareTag(_StoneTag))
            {
                GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().footstepStone);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepWood);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepLustre);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepLantern);
            }
            else if (_Surface.CompareTag(_LustreTag))
            {
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepStone);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepWood);
                GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().footstepLustre);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepLantern);
            }
            else if (_Surface.CompareTag(_LanternTag))
            {
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepStone);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepWood);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepLustre);
                GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().footstepLantern);
            }
            else
            {
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepStone);
                GetComponent<PlayerSFX>().PlaySXF(GetComponent<PlayerSFX>().footstepWood);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepLustre);
                GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepLantern);
            }
        }


        public void StopSFXFootStep()
        {
            GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepStone);
            GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepWood);
            GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepLustre);
            GetComponent<PlayerSFX>().StopSFX(GetComponent<PlayerSFX>().footstepLantern);
        }
        private void OnTriggerExit2D(Collider2D pCollision)
        {
            if (_PlayerController.playerDied)
                return;

            if (((1 << pCollision.gameObject.layer) & _LightLayerMask) != 0
                && _PlayerController.timer != null)
                _PlayerController.StopTimer();
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.GetComponent<AutomaticPlatform>() || collision.collider.GetComponent<MobilePlatform>())
                transform.parent = collision.transform;
        }


        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.GetComponent<AutomaticPlatform>() || collision.collider.GetComponent<MobilePlatform>())
                transform.parent = (_PlayerController != null) ? _PlayerController.gameContainer : null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_FrontRayCastPoint.position, Vector2.down*2);
            //Physics2D.Raycast(_CeilingRayCastPoint.transform.position, Vector2.up);
            if (collider == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector2(collider.bounds.center.x, collider.bounds.max.y), new Vector2(collider.bounds.size.x, 0.1f));

            Gizmos.DrawWireCube(new Vector2(collider.bounds.center.x, collider.bounds.min.y), new Vector2(1 , 0.2f));
        }

    }
}