using System;
using UnityEngine;

// Author : 
namespace Com.IsartDigital.Platformer
{
    public class LineCreation : MonoBehaviour
    {
        private LineRenderer _LineRenderer;
        private Transform[] _PointsList;
        private int _PointsCount;

        public void SetUpLine(Transform[] pPoints)
        {
            _LineRenderer = GetComponent<LineRenderer>();
            _LineRenderer.positionCount = pPoints.Length;
            _PointsList = pPoints;
            _PointsCount = _PointsList.Length;
        }

        private void Update()
        {
            for (int i = 0; i < _PointsCount; i++)
            {
                _LineRenderer.SetPosition(i, _PointsList[i].position);
            }
        }
    }
}
