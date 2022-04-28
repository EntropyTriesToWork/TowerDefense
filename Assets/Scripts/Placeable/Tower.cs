using Entropy.TD.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Entropy.TD.Tower
{
    public abstract class Tower : Placeable
    {
        #region Tower Variables
        [BoxGroup("Tower")] public Targeting currentTargetingType;
        #endregion

        #region Targeting
        public abstract List<IDamageable> GetTargets();
        #endregion
    }
    public enum Targeting
    {
        Close,
        Furthest,
        Healthiest,
        NearDeath,
        Random
    }
}