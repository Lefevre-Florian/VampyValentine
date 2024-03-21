using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Platformer
{
	public class RandomOffsetVFX : MonoBehaviour
	{
        [SerializeField] protected Animator m_Animator;
        [SerializeField] protected string m_AnimationTrigger;
        [SerializeField] protected string m_OffsetFloatParameter;

        void Start()
        {
            m_Animator.SetFloat(m_OffsetFloatParameter, Random.Range(0f, 1f));
            m_Animator.SetTrigger(m_AnimationTrigger);
        }
    }
}
