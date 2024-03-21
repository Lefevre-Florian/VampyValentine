// Author : Renaudin Matteo
using System.Collections;
using UnityEngine.Events;
using UnityEngine;

namespace Com.IsartDigital.Platformer.InGameElement.Killzone
{
    public class Killzone : GameElement
    {
        public static UnityEvent resetLevel = new UnityEvent();
        private Collider2D _Collider2D => GetComponent<Collider2D>();


        public override void Effect(PlayerBis.PlayerController pPlayer)
        {
            if (_Collider2D != null)
            {
                _Collider2D.enabled = false;
            }
            pPlayer.OnKillZone();
            StartCoroutine(OnDeathCoroutine());
        }

        protected IEnumerator OnDeathCoroutine()
        {
            yield return new WaitForSeconds(GameManager.GetInstance().secondOnDeath);
            resetLevel.Invoke();
            _Collider2D.enabled = true;
            StopCoroutine(OnDeathCoroutine());
        }
    }
}