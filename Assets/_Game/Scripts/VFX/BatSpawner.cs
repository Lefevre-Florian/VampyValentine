using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Platformer
{
    public class BatSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _Bat= null;
        [SerializeField] private Transform VFXContainer = null;




        public void SpawnAndHide()
        {
            Instantiate(_Bat, transform.position, Quaternion.identity, VFXContainer);
            gameObject.SetActive(false);
        }





    }
}
