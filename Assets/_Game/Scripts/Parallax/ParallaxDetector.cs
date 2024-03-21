using UnityEngine;

namespace Com.IsartDigital.Platformer.Game.Scrolling
{
    public class ParallaxDetector : MonoBehaviour
    {
        [Header("Gameplay")]
        [SerializeField] private bool _IsExit = false;
        [SerializeField] private Parallax _Parallax = null;

        [Header("Physics")]
        [SerializeField] private LayerMask _PlayerLayer = default;

        private void OnTriggerExit2D(Collider2D pCollider)
        {
            if (((1 << pCollider.gameObject.layer) & _PlayerLayer) != 0)
                Execute();
        }

        protected virtual void Execute()
        {
            if (!_IsExit)
                _Parallax.Enable();
            else
                _Parallax.Disable();
        }

    }
}