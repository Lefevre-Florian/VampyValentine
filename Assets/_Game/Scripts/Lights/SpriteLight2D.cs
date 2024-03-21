using System;
using System.Collections.Generic;
using Com.IsartDigital.Platformer.InGameElement.Killzone;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Platformer.Light
{
    [RequireComponent(typeof(SpriteRenderer), typeof(PolygonCollider2D))]
    public class SpriteLight2D : Killzone
    {
        private const int ACCURACY = 10;
        private const int COLLISION_PADDING = 2;

        #region Shader Keys
        private const string KEY_UV_ARRAY = "_UVPositions";
        private const string KEY_UV_ARRAY_LENGTH = "_UVLength";
        #endregion

        [Header("Physics")]
        [SerializeField] private LayerMask _CollisionMask = ~0;
        [SerializeField] private LayerMask _ReflectionMask = default;

        [Header("Gameplay")]
        [SerializeField] private LayerMask _PlayerMask = default;
        [SerializeField] private bool _IsTurnedOn = true;

        // Variables
        private float _Step = 0f;

        private float _Distance = 0f;
        private float _HSize = 0f;

        private int _Precision = 0;

        private RaycastHit2D _Hit = default;

        // Buffer for the light informations
        private Vector3 _BufferPosition = Vector2.zero;
        private Vector2 _UVPositionBuffer = Vector2.one;
        private Quaternion _BufferRotation = Quaternion.identity;
        
        private Vector2 _FCastPosition = Vector2.zero;
        private Vector2 _RayDirection = Vector2.zero;

        private Vector2[] _UVPoints = null;
        private Vector2[] _PhysicsPoints = null;

        private List<Transform> _CollisionBuffer = null;
        private List<Transform> _ReflectionSources = new List<Transform>();
    
        private SpriteRenderer _SpriteRenderer = null;

        private Material _ShaderMaterial = default;

        private bool _IsColliding = false;
        private bool _IsLightingColling = false;

        private bool _UpdateLightRenderer = false;
        private bool _UpdateLightCollider = false;

        private PolygonCollider2D _Collider2D = null;
        
        // Only used in reflection process
        private SpriteLight2D _Source = null;
        private Transform _Surface = null;

        private Vector2 _Offset = Vector2.zero;

        // Get/Set
        public float TextureHeight { 
            get 
            {
                Sprite lTexture = GetComponent<SpriteRenderer>().sprite;
                return lTexture.rect.height / lTexture.pixelsPerUnit; 
            } 
        }
        public float TextureWidth {
            get 
            {
                Sprite lTexture = GetComponent<SpriteRenderer>().sprite;
                return lTexture.rect.width / lTexture.pixelsPerUnit; 
            } 
        }
        public Vector2 EmissionPoint
        {
            get { return transform.position - transform.up * _Distance; }
        }

        // Events
        public event Action OnLightSourceInfoChanged;

        public event Action<Transform> OnReflectionSurfaceCollisionEntered;
        public event Action<Transform> OnReflectionSurfaceCollisionExited;
        protected override void Start()
        {
            // References
            _Collider2D = GetComponent<PolygonCollider2D>();

            // Initialize
            _SpriteRenderer = GetComponent<SpriteRenderer>();

            // Computation
            _HSize = TextureWidth * Mathf.Abs(transform.localScale.x);
            _Precision = (int)(ACCURACY * _HSize);
            _Step = _HSize / (ACCURACY * _HSize);

            _Distance = TextureHeight * Mathf.Abs(transform.localScale.y);

            if(_Source != null || _Surface != null)
                ComputeLightInformation();

            // Compute the UV list size
            _UVPoints = new Vector2[_Precision + 1];
            int lLength = _UVPoints.Length;

            _PhysicsPoints = new Vector2[lLength + COLLISION_PADDING];

            for (int i = 0; i < lLength; i++)
            {
                _UVPoints[i] = new Vector2(_Step * i, 1.0f);
                _PhysicsPoints[i + 1] = new Vector2(_UVPoints[i].x - (TextureWidth / 2f),
                                                    -(TextureHeight / 2f));    
            }

            // Apply shader for light computation
            _ShaderMaterial = Instantiate(_SpriteRenderer.material);
            _SpriteRenderer.material = _ShaderMaterial;
            RedrawLigthRendering();

            // End physics computation
            _PhysicsPoints[0] = new Vector2(-TextureWidth / 2f,
                                            TextureHeight / 2f);
            _PhysicsPoints[_PhysicsPoints.Length - 1] = new Vector2(TextureWidth / 2f,
                                                                    TextureHeight / 2f);
            _Collider2D.points = _PhysicsPoints;
            _Collider2D.enabled = false;

            // Allocate collision buffer
            _CollisionBuffer = new List<Transform>();
        }

        private void Update()
        {
            // Primary detection (optimization)
            if (!_IsLightingColling)
            {
                _Hit = Physics2D.BoxCast(transform.position, 
                                         new Vector2(_HSize, _Distance), 
                                         transform.eulerAngles.z, 
                                         _RayDirection, 
                                         0f,
                                         ~(1 << gameObject.layer));

                if (_Hit.collider != null)
                {
                    _IsColliding = true;
                    _Collider2D.enabled = true;
                }
                    
                CleanCollisionBuffer();
            }
            
            if (_IsTurnedOn && _IsColliding)
            {
                _IsLightingColling = false;

                // Computation of lights
                for (int i = 0; i <= _Precision; i++)
                {
                    _Hit = Physics2D.Raycast(_FCastPosition + (Vector2)transform.right * (_Step * i),
                                             _RayDirection,
                                             _Distance,
                                             _CollisionMask & ~gameObject.layer);

                    if (_Hit.collider != null)
                    {
                        // Global update of the rendering
                        _UVPositionBuffer = new Vector2(_Step * i,
                                                        Mathf.Abs(Vector2.Distance(_FCastPosition + _Offset + (Vector2)transform.right * (_Step * i),_Hit.point)) / _Distance);
                        
                        _CollisionBuffer.Add(_Hit.transform);

                        // Check if the surface can reflect the light (update reflection)
                        if (((1 << _Hit.transform.gameObject.layer) & _ReflectionMask) != 0)
                            Reflect(_Hit);
                        
                        // Check if the colliding entity is not a player (update physics)
                        if (((1 << _Hit.transform.gameObject.layer) & _PlayerMask) == 0)
                        {
                            _PhysicsPoints[i + 1] = new Vector2(_UVPoints[i].x / transform.localScale.x - (TextureWidth / 2f),
                                                                (TextureHeight / 2f) - _UVPoints[i].y * TextureHeight);
                        }

                        _IsLightingColling = true;
                    }
                    else
                    {
                        // Colliding nothing or colliding a layer that is not include in the light computation
                        _UVPositionBuffer = new Vector2(_Step * i, 1.0f);
                        _PhysicsPoints[i + 1] = new Vector2(_UVPoints[i].x / transform.localScale.x - (TextureWidth / 2f), 
                                                            -TextureHeight / 2f);
                    }

                    if (_UVPoints[i] != _UVPositionBuffer)
                    {
                        _UVPoints[i] = _UVPositionBuffer;
                        _UpdateLightRenderer = true;
                    }

                    if (_PhysicsPoints[i + 1] != _Collider2D.points[i + 1])
                        _UpdateLightCollider = true;
                        
                }

                // Update physics (homemade & Unity)
                if (_UpdateLightCollider)
                {
                    _UpdateLightCollider = false;
                    _Collider2D.points = _PhysicsPoints;
                }

                CleanCollisionBuffer();

                if (!_IsLightingColling)
                {
                    _IsColliding = false;
                    _Collider2D.enabled = false;
                }

                // Update Light rendering
                if (_UpdateLightRenderer)
                {
                    RedrawLigthRendering();
                    _UpdateLightRenderer = false;
                }
            }

            if (_BufferPosition != transform.position 
                || _BufferRotation != transform.rotation)
            {
                ComputeLightInformation();
                OnLightSourceInfoChanged?.Invoke();
            }
                
        }

        #region Physics & Collision
        private void CleanCollisionBuffer()
        {
            if (_CollisionBuffer == null || _CollisionBuffer.Count == 0)
                return;

            int lLength = _ReflectionSources.Count;
            for (int i = 0; i < lLength; i++)
            {
                if (!_CollisionBuffer.Contains(_ReflectionSources[i]))
                {
                    OnReflectionSurfaceCollisionExited?.Invoke(_ReflectionSources[i]);
                    _ReflectionSources.RemoveAt(i);
                }
            }
            _CollisionBuffer.Clear();
        }
        #endregion

        #region Light update
        private void ComputeLightInformation()
        {
            // Recompute light info if the position or rotation has changed since the last frame
            _BufferPosition = transform.position;
            _BufferRotation = transform.rotation;

            if(_Source != null || _Surface != null)
                _Offset = (Vector2)transform.up * _Surface.GetComponent<Mirror>().GetOffsetDistance();

            _FCastPosition = transform.position - transform.right * (_HSize / 2f) + transform.up * (_Distance / 2f);
            _FCastPosition -= _Offset;

            _RayDirection = -transform.up;
        }

        /// <summary>
        /// Contact shader to order a redraw of the sprite
        /// </summary>
        private void RedrawLigthRendering()
        {
            int lLength = _UVPoints.Length;
            float[] lArray = new float[_UVPoints.Length];

            for (int i = 0; i < lLength; i++)
                lArray[i] = 1 - _UVPoints[i].y;

            _ShaderMaterial.SetFloatArray(KEY_UV_ARRAY, lArray);
            _ShaderMaterial.SetFloat(KEY_UV_ARRAY_LENGTH, lLength);
        }
        #endregion

        #region Reflection
        /// <summary>
        /// Calculate the light reflection depending of the contact surface
        /// </summary>
        private void Reflect(RaycastHit2D pSurface)
        {
            if (!_ReflectionSources.Contains(pSurface.transform)
                && _Surface != pSurface.transform)
            {
                Mirror lMirror = pSurface.transform.GetComponent<Mirror>();
                _ReflectionSources.Add(pSurface.transform);
                lMirror.OnSurfaceReflect(this);
            }
        }

        /// <summary>
        /// Begin light reflection (associate source to it's reflection)
        /// </summary>
        /// <param name="pSource"></param>
        /// <param name="pSurface"></param>
        public void SetLightSource(SpriteLight2D pSource, Transform pSurface)
        {
            _Source = pSource;
            _Surface = pSurface;

            _Source.OnReflectionSurfaceCollisionExited += RemoveLightSource;
        }

        /// <summary>
        /// Destroy the light if it's source is not in contact with the reflection surface
        /// </summary>
        /// <param name="pSurfaceExited"></param>
        public void RemoveLightSource(Transform pSurfaceExited)
        {
            if (_Source == null || pSurfaceExited != _Surface)
                return;

            int lLength = _ReflectionSources.Count;
            for (int i = 0; i < lLength; i++)
                OnReflectionSurfaceCollisionExited?.Invoke(_ReflectionSources[i]);
            Destroy(gameObject);
        }
        #endregion

        public void SwitchOffLight()
        {
            int lLength = _ReflectionSources.Count;
            for (int i = 0; i < lLength; i++)
            {
                OnReflectionSurfaceCollisionExited?.Invoke(_ReflectionSources[i]);
                _ReflectionSources.RemoveAt(i);
            }

            CleanCollisionBuffer();
            _IsTurnedOn = false;

            enabled = false;
            if(_Collider2D != null)
                _Collider2D.enabled = false;

            gameObject.SetActive(false);
        }

        public void SwitchOnLight()
        {
            gameObject.SetActive(true);

            enabled = true;
            if(_Collider2D != null)
                _Collider2D.enabled = true;

            _IsTurnedOn = true;
        }

        public void SwitchLightControl(bool pState)
        {
            if (pState)
                SwitchOnLight();
            else
                SwitchOffLight();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_FCastPosition, 0.1f);
            }
        }

        /// <summary>
        /// Proceed a complete cleaning of the objects before destroying it
        /// </summary>
        private void OnDestroy()
        {
            _UVPoints = null;
            if (_Source != null)
                _Source.OnReflectionSurfaceCollisionExited -= RemoveLightSource;
            _Source = null;
            _Surface = null;

            _CollisionBuffer?.Clear();

            if (_ReflectionSources != null)
                _ReflectionSources.Clear();
            _ReflectionSources = null;
        }
    }
}
