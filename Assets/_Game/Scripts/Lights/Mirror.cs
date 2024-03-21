using System;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Platformer.Light
{
    public class Mirror : MonoBehaviour
    {
        private const float MARGIN = 0.01f;

        [Header("Settings")]
        [SerializeField][Range(-180f, 180f)] private float _NormalAngle = 0f;
        [SerializeField][Min(1f)] private float _ReflectionDistance = 1f;

        [Header("Prefabs")]
        [SerializeField] private GameObject _LightRay = null;

        private float _Width = 0f;

        private void Start()
        {
            Sprite lTexture = GetComponent<SpriteRenderer>().sprite;
            _Width = (lTexture.rect.width / lTexture.pixelsPerUnit) * transform.localScale.x;
        }

        public Vector2 GetNormal() => (Quaternion.AngleAxis(_NormalAngle, Vector3.forward) * transform.right).normalized;

        /// <summary>
        /// Calculate the distance from the center to the bounds (include diagonal)
        /// </summary>
        /// <returns></returns>
        public float GetOffsetDistance() => Mathf.Abs((_Width / 2f) / Mathf.Cos(_NormalAngle));

        public void OnSurfaceReflect(SpriteLight2D pSource)
        {
            if (_LightRay == null)
                return;
            Transform lReflection = Instantiate(_LightRay, transform.parent).transform;

            SpriteLight2D lLightRay = lReflection.GetComponent<SpriteLight2D>();
            lLightRay.SetLightSource(pSource, transform);

            Vector2 lNormal = GetNormal();

            lReflection.position = (Vector2)transform.position + lNormal * ((lLightRay.TextureHeight * (_ReflectionDistance * lReflection.localScale.y / 2f)) + MARGIN);
            lReflection.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(lNormal.x, -lNormal.y) * Mathf.Rad2Deg);
            lReflection.localScale = new Vector2(lReflection.localScale.x,
                                                 lReflection.localScale.y * _ReflectionDistance);
        }

        #region IN-EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position, .05f);
                Gizmos.DrawRay(transform.position, 
                               Quaternion.AngleAxis(_NormalAngle, Vector3.forward) * transform.right);
            }
        }
        #endregion
    }
}
