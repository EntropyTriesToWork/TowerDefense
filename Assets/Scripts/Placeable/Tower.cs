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
        [BoxGroup("Tower")] public GameObject projectilePrefab;
        [BoxGroup("Tower")] public float cooldown = 2f;

        [BoxGroup("Read Only")] [ReadOnly] public Enemy currentTarget;
        [BoxGroup("Read Only")] [ReadOnly] public float cooldownRemaining;
        [BoxGroup("Read Only")] public Transform testTarget;
        #endregion

        private void Update()
        {
            LookAtTarget();
        }

        #region Attacking
        public abstract void FireProjectile();
        #endregion

        #region Targeting
        public abstract void LookAtTarget();
        public abstract List<IDamageable> GetTargets();
        #endregion
    }
    public class TowerStats
    {
        public Stat baseDamage;
        public Stat attackSpeed;
        public Stat armorPierce;
        public Stat attackRange;

        public TowerStats()
        {
            baseDamage = new Stat() { BaseValue = 5 };
            attackSpeed = new Stat() { BaseValue = 1 };
            armorPierce = new Stat() { BaseValue = 0 };
            attackRange = new Stat() { BaseValue = 2 };
        }
        public TowerStats(int damage, float attackSpeed, int armorPierce, float range)
        {
            this.baseDamage = new Stat() { BaseValue = damage };
            this.attackSpeed = new Stat() { BaseValue = attackSpeed };
            this.armorPierce = new Stat() { BaseValue = armorPierce };
            this.attackRange = new Stat() { BaseValue = range };
        }
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