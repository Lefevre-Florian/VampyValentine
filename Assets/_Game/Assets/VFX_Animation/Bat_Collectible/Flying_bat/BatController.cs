using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Platformer
{
    public class BatController : MonoBehaviour
    {
        public float flapForce = 5f; 
        public float maxVerticalSpeed = 7f; 

        private Rigidbody2D rb;

        private IEnumerator KillBat()
        {
            yield return new WaitForSeconds(10);

            Destroy(gameObject);

        }

        void Start()
        {
            StartCoroutine(KillBat());

            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {

        }

        void Flap()
        {
            rb.velocity = new Vector2(rb.velocity.x + 1, Mathf.Clamp(rb.velocity.y + flapForce, -maxVerticalSpeed, maxVerticalSpeed));
        }
    }
}
