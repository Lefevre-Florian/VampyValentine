using Cinemachine;
using Com.IsartDigital.Platformer.SoundManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

//Author : Dany & Bastien
namespace Com.IsartDigital.Platformer.Game
{
    public class CameraSwitcher : MonoBehaviour
    {
        public bool isBlocking;

        public GameObject currentLevelPlatform;

        public GameObject cameraFrom;
        public GameObject cameraTo;

        [NonSerialized] public CameraDetector detectorFrom;
        [NonSerialized] public CameraDetector detectorTo;

        public static UnityEvent<GameObject> cameraEvent = new UnityEvent<GameObject>();

        public bool isCameraToForced;
        public AnimationClip anim;
        public Animator animPlayer;

        public Animator previousAnimator;

        public ScriptSFX maidSFX => GetComponent<ScriptSFX>();

        [SerializeField] private Transform _AutomaticPlatformsContainer = null;

        public Transform AutomaticPlatformsContainer { get { return _AutomaticPlatformsContainer; } }

        private void Start()
        {
          
            
            detectorFrom = transform.GetChild(0).GetComponent<CameraDetector>();
            detectorTo = transform.GetChild(1).GetComponent<CameraDetector>();
        }

    }
}