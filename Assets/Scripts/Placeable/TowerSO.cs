using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Entropy.TD.Tower
{
    [CreateAssetMenu(menuName = "Tower Data")]
    public class TowerSO : ScriptableObject
    {
        public int buildCost;
        [Required] [AssetsOnly] public Sprite towerSprite;
        [Required] [AssetsOnly] public GameObject towerPrefab;
    }
}