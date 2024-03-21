using Com.IsartDigital.Platformer.InGameElement.Killzone;
using Com.IsartDigital.Platformer.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer.Game.Scrolling
{
    [Serializable]
    public class ParallaxBackground
    {
        public List<Plan> plans;
    }

    [Serializable]
    public class Plan
    {
        public GameObject plan ;
        public float speed;
    }

    public class Parallax : MonoBehaviour, IResetableObject
    {
        [Header("Parallax")]
        [SerializeField] private ParallaxBackground _Parallax = null;

        [Space(2)]
        [SerializeField] private bool _IsCorridor = false;

        private Vector2[] _ParallaxInitialPositions = null;

        public Vector3 initialPosition => throw new System.NotImplementedException();

        private Coroutine _ParallaxEffect = null;

        private Vector2 _Left = Vector2.left;

        private void Start()
        {
            int lLength = _Parallax.plans.Count;
            _ParallaxInitialPositions = new Vector2[lLength];
            for (int i = 0; i < lLength; i++)
                _ParallaxInitialPositions[i] = _Parallax.plans[i].plan
                                                                 .transform
                                                                 .position;
        }

        private void Run() => StartCoroutine(Scrolling());

        private IEnumerator Scrolling()
        {
            while (true)
            {
                foreach (Plan lPlan in _Parallax.plans)
                    lPlan.plan.transform.position += (Vector3)_Left * lPlan.speed * Time.deltaTime;
                yield return null;
            }
        }

        public void Enable()
        {
            Killzone.resetLevel.AddListener(ResetObject);

            StopAllCoroutines();
            Run();
        }

        public void Disable()
        {
            Killzone.resetLevel.RemoveListener(ResetObject);
            StopAllCoroutines();

            int lLength = _ParallaxInitialPositions.Length;
            for (int i = 0; i < lLength; i++)
                _Parallax.plans[i].plan
                                  .transform
                                  .position = _ParallaxInitialPositions[i];

            _ParallaxEffect = null;
        }

        public void ResetObject()
        {
            StopAllCoroutines();
            int lLength = _ParallaxInitialPositions.Length;
            for (int i = 0; i < lLength; i++)
                _Parallax.plans[i].plan
                                  .transform
                                  .position = _ParallaxInitialPositions[i];

            if (!_IsCorridor)
                Run();
        }

        private void OnDestroy()
        {
            Killzone.resetLevel?.RemoveListener(ResetObject);
            StopAllCoroutines();
            _ParallaxEffect = null;
        }
    }

}
