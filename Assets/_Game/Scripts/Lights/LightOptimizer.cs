using System;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer.Light
{
    public class LightOptimizer : MonoBehaviour
    {
        [SerializeField] private LayerMask _PlayerLayer = ~0;

        [SerializeField] private Transform _INLightContainer = null;
        [SerializeField] private Transform _OUTLightContainer = null;

        private void OnTriggerEnter2D(Collider2D pCollider)
        {
            if(((1 << pCollider.gameObject.layer) & _PlayerLayer) != 0)
            {
                ChangeLightsState(_INLightContainer, true);
                ChangeLightsState(_OUTLightContainer, false);

                Permute();
            }   
        }

        private void Permute()
        {
            if (_INLightContainer == null || _OUTLightContainer == null)
                return;

            Transform lTemp = _INLightContainer;

            _INLightContainer = _OUTLightContainer;
            _OUTLightContainer = lTemp;
        }

        private void ChangeLightsState(Transform pContainer, bool pState)
        {
            if (pContainer == null)
                return;

            SpriteLight2D lLight2D = null;

            int lLength = pContainer.childCount;
            for (int i = 0; i < lLength; i++)
            {
                lLight2D = pContainer.GetChild(i).GetComponent<SpriteLight2D>();
                lLight2D.SwitchLightControl(pState);
            }
        }

    }
}
