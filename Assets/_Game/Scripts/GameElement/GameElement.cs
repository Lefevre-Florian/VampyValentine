using UnityEngine;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.InGameElement
{
    public class GameElement : MonoBehaviour
    {
        protected virtual void Start() { }
        public virtual void Effect(PlayerBis.PlayerController pPlayer) { }
    }
}