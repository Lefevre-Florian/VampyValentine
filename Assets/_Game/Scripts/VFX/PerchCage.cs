using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Platformer
{
    public class PerchCage : MonoBehaviour
    {

        private Rigidbody2D _RB => GetComponent<Rigidbody2D>();
        private PlayerBis.PlayerController playerController => PlayerBis.PlayerController.GetInstance();
        
        [SerializeField] private Animator _BatAnimator;

        public void PlayerEntered()
        {
            _RB.bodyType = RigidbodyType2D.Dynamic;
            _RB.AddForce(new Vector2(Mathf.Sign(playerController.rb.velocity.x) * 100, 50));
            _BatAnimator.SetTrigger("Fly");
        }

    }
}
