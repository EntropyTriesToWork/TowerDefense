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
        [BoxGroup("Tower")] public TowerStats towerStats = new TowerStats();
        [BoxGroup("Tower")] public GameObject projectilePrefab;
        [BoxGroup("Tower")] public GameObject rangeIndicator;

        [BoxGroup("Read Only")] [ReadOnly] public Enemy currentTarget;
        [BoxGroup("Read Only")] [ReadOnly] public float cooldownRemaining;
        [BoxGroup("Read Only")] [ReadOnly] public bool canAttack;
        [BoxGroup("Read Only")] [ReadOnly] [SerializeField] float _cooldown;
        #endregion

        public float DistanceToTarget => Vector3.Distance(currentTarget.transform.position, transform.position);

        #region Messages
        public virtual void Update()
        {
            if (!canAttack) { return; }
            if (_cooldown > 0f) { _cooldown -= Time.deltaTime; }
            if (currentTarget == null || currentTarget.Dead) { SetNewTarget(); return; }
            if (DistanceToTarget > Range) { SetNewTarget(); return; }
            LookAtTarget();
            if (_cooldown <= 0f && currentTarget != null) { FireProjectile(); _cooldown = 1f / AttackSpeed; }
        }
        public virtual void Start()
        {
            ToggleRangeEffect(true);
        }
        private void OnMouseDown()
        {
            ToggleRangeEffect(true);
        }
        private void OnMouseExit()
        {
            ToggleRangeEffect(false);
        }
        #endregion

        #region Attacking
        public abstract void FireProjectile();
        public DamageInfo GetDamageInfo()
        {
            return new DamageInfo()
            {
                damage = Damage,
                pierce = ArmorPierce,
                isCrit = false
            };
        }
        #endregion

        #region Stats
        public int Damage => Mathf.RoundToInt(towerStats.baseDamage.Value);
        public int ArmorPierce => Mathf.RoundToInt(towerStats.armorPierce.Value);
        public float AttackSpeed => Mathf.Max(0.1f, towerStats.attackSpeed.Value);
        public float ProjectileSpeed => Mathf.Max(0.1f, towerStats.projectileSpeed.Value);
        public float Range => Mathf.Max(1f, towerStats.attackRange.Value);
        #endregion

        #region Targeting
        public abstract void LookAtTarget();
        public void SetNewTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, Range, LayerMask.GetMask("Enemy"));
            if (hits.Length > 0)
            {
                float dist = Range + 1;
                var closest = hits[0];
                foreach (var hit in hits)
                {
                    if (Vector3.Distance(transform.position, hit.transform.position) < dist)
                    {
                        closest = hit;
                    }
                }
                currentTarget = closest.GetComponent<Enemy>();
            }
        }
        public virtual void UpdateRangeIndicator()
        {
            rangeIndicator.transform.localScale = new Vector3(Range * 2, 0.01f, Range * 2);
        }
        public void ToggleRangeEffect(bool visible)
        {
            rangeIndicator.SetActive(visible);
            UpdateRangeIndicator();
        }
        #endregion
    }
    [System.Serializable]
    public class TowerStats
    {
        public Stat baseDamage;
        public Stat attackSpeed;
        public Stat projectileSpeed;
        public Stat armorPierce;
        public Stat attackRange;

        public TowerStats()
        {
            baseDamage = new Stat() { BaseValue = 5 };
            attackSpeed = new Stat() { BaseValue = 1 };
            projectileSpeed = new Stat() { BaseValue = 10 };
            armorPierce = new Stat() { BaseValue = 0 };
            attackRange = new Stat() { BaseValue = 5 };
        }
        public TowerStats(int damage, float attackSpeed, float projectileSpeed, int armorPierce, float range)
        {
            this.baseDamage = new Stat() { BaseValue = damage };
            this.attackSpeed = new Stat() { BaseValue = attackSpeed };
            this.projectileSpeed = new Stat() { BaseValue = projectileSpeed };
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