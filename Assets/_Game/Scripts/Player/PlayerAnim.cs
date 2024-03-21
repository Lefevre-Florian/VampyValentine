using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.IsartDigital.Platformer.PlayerBis;
using UnityEngine.UIElements;
using Cinemachine;

namespace Com.IsartDigital.Platformer
{
    public class PlayerAnim : MonoBehaviour
    {
        [SerializeField] private Transform _VFXContainer;
        [SerializeField] private GameObject _JumpPulseAnim;
        [SerializeField] private GameObject _JumpLandingAnim;
        [SerializeField] private GameObject _DashDustAnim;
        [SerializeField] private GameObject _DashArrowAnim;
        [SerializeField] private GameObject _DeathSmokeAnim;
        [SerializeField] private Animator _RunDustAnim;

        [Range(0.1f, 1), SerializeField] private float ScreenShakeForce = 0.3f;

        public Animator animator;

        private Quaternion _defaultRotation;

        private PlayerController _PlayerController => PlayerController.GetInstance();

        private const string A_PLAYER_IDLE = "A_player_idle";
        private const string A_PLAYER_JUMP = "A_Player_Jump";
        private const string A_PLAYER_RUN = "A_Player_Run";
        private const string A_PLAYER_DASH = "A_Player_Dash";
        private const string A_PLAYER_GLIDE = "A_Player_Glide";
        private const string A_PLAYER_JFALL = "A_Player_JFall";
        private const string A_PLAYER_CONTROL = "A_Player_Control";
        private const string A_PLAYER_DEATH_MAID = "A_player_death_01";

        private CinemachineImpulseSource _ImpulseSource => GetComponent<CinemachineImpulseSource>();



        #region Singleton
        private static PlayerAnim _Instance = null;

        public static PlayerAnim GetInstance()
        {
            if(_Instance == null) _Instance = new PlayerAnim();
            return _Instance;
        }

    private PlayerAnim() : base() {}
        #endregion



        


        private void Awake()
        {

            if (_Instance != null)
            {
                Destroy(this);
                return;
            }

            _Instance = this;
            _defaultRotation = Quaternion.identity;
        }


        public void PlayDashScreenShake()
        {
            _ImpulseSource.GenerateImpulseWithForce(ScreenShakeForce);
        }


        public void PlayPulseAnim(Vector3 pPosition)
        {
            Instantiate<GameObject>(_JumpPulseAnim, pPosition,Quaternion.identity, _VFXContainer);
        }

        public void PlayLandingAnim(Vector3 pPosition)
        {
            Instantiate<GameObject>(_JumpLandingAnim, pPosition, Quaternion.identity, _VFXContainer);
        }

        public void PlayRunDustAnim()
        {
            _RunDustAnim.gameObject.SetActive(true);
            _RunDustAnim.Play("Dust_Run_VFX");
        }

        public void PlayDustVFX(Vector3 pPosition)
        {
            GameObject lDashDust = _DashDustAnim;
            _DashDustAnim.transform.localScale =  PlayerController.GetInstance().transform.localScale;
            Instantiate<GameObject>(lDashDust, pPosition, Quaternion.identity, _VFXContainer);
        }



        public void PlayArrowVFX(Vector3 pPosition, Vector3 pDashEnd)
        {
            GameObject lArrowDash = Instantiate<GameObject>(_DashArrowAnim, pPosition, Quaternion.identity, _PlayerController.transform);

            

            Vector3 lDirection = pDashEnd - pPosition;
            float lAngle = Mathf.Atan2(lDirection.y, lDirection.x) * Mathf.Rad2Deg;
            lArrowDash.transform.localScale = _PlayerController.transform.localScale;
            lArrowDash.transform.rotation = Quaternion.AngleAxis(lAngle, Vector3.forward);
        }


        public void PlayDeathSmokeVFX()
        {

            Instantiate<GameObject>(_DeathSmokeAnim, transform.position, Quaternion.identity, _VFXContainer);

        }

        public void PlayRunAnim()
        {
            animator.Play(A_PLAYER_RUN);
            animator.transform.rotation = _defaultRotation;

        }




        public void PlayControlAnim()
        {
            animator.Play(A_PLAYER_CONTROL);
            animator.transform.rotation = _defaultRotation;

        }


        public void PlayJumpAnim()
        {
            animator.Play(A_PLAYER_JUMP);
            animator.transform.rotation = _defaultRotation;

        }

        public void PlayFallAnim()
        {
            animator.Play(A_PLAYER_JFALL);
            animator.transform.rotation = _defaultRotation;

        }

        public void PlayIdleAnim()
        {
            animator.Play(A_PLAYER_IDLE);
            animator.transform.rotation = _defaultRotation;
        }


        public void PlayGlideAnim()
        {
            animator.Play(A_PLAYER_GLIDE);
            animator.transform.rotation = _defaultRotation;
        }

        public void PlayDashAnim(Vector3 pDashEnd)
        {
            Vector3 lDirection = pDashEnd - transform.position;
            float lAngle = Mathf.Atan2(lDirection.y, lDirection.x) * Mathf.Rad2Deg;
            animator.gameObject.transform.rotation = Quaternion.AngleAxis(lAngle, Vector3.forward);
            animator.Play(A_PLAYER_DASH);

        }

        
        public void PlayDeathMaidAnim()
        {
            animator.Play(A_PLAYER_DEATH_MAID);
            animator.transform.rotation = _defaultRotation;
        }


        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }
    }
}
