using Com.IsartDigital.Platformer.Game;
using UnityEngine;

// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.InGameElement.Collectible
{
    [CreateAssetMenu(
    menuName = "Scriptable/CollectibleInfo",
    fileName = "CollectibleInfo",
    order = 1)]
    public class CollectibleInfo : ScriptableObject
    {
        public int score;
        public GameManager.CollectibleType type;
    }
}
