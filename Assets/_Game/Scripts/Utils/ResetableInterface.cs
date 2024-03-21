
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


//Author : Dany
namespace Com.IsartDigital.Platformer.Utils
{
    public interface IResetableObject

    {
        Vector3 initialPosition { get; }

        void ResetObject();

    }
}
