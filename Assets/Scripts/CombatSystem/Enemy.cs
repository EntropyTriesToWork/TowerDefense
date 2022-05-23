using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Entropy.TD.Tower;

namespace Entropy.TD.Entities
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [ReadOnly] public Transform targetPosition;

        [BoxGroup("Stats")] public EnemyStats stats;

        [BoxGroup("Required")] public Transform enemyBody;

        #region Stats & Getters
        [BoxGroup("Read Only")] [SerializeField] [ReadOnly] int _hp;
        public int Health
        {
            get => _hp;
            set
            {
                _hp = Mathf.Clamp(value, 0, Mathf.RoundToInt(stats.health.Value));
            }
        }
        public int Damage { get => Mathf.RoundToInt(stats.damage.Value); }
        public int Armor { get => Mathf.RoundToInt(stats.armor.Value); }
        public float MoveSpeed { get => Mathf.Max(0f, stats.moveSpeed.Value); }
        public int Money { get => Mathf.RoundToInt(stats.money.Value); }

        public bool Dead => Health < 1;
        public Vector3 MoveDirection => (targetPosition.position - transform.position).normalized;
        #endregion

        public void EnemyDeath()
        {
            GameManager.Instance.Money += Money;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(Shrink());

            IEnumerator Shrink()
            {
                while (transform.localScale.magnitude > 0.1f)
                {
                    transform.localScale *= 0.9f;
                    yield return new WaitForEndOfFrame();
                }
                Destroy(gameObject);
            }
        }

        #region Interface
        public void Heal(int healAmount)
        {
            throw new System.NotImplementedException();
        }
        public DamageReport TakeDamage(DamageInfo damageInfo)
        {
            if (Dead) { return new DamageReport(); }
            int damage = damageInfo.damage - (Mathf.Max(0, Armor - damageInfo.pierce));
            Health -= damage;

            if (Dead) { EnemyDeath(); }

            DamageReport report = new DamageReport();
            report.damageTaken = damage;
            report.isDead = Dead;
            return report;
        }
        #endregion

        #region
        void Start()
        {
            targetPosition = GameManager.Instance.GetNextPathPoint(targetPosition);
            _hp = Mathf.RoundToInt(stats.health.Value);
        }
        void Update()
        {
            if (Dead) { return; }
            if (Vector3.Distance(transform.position, targetPosition.position) > 0.1f)
            {
                transform.Translate(MoveDirection * stats.moveSpeed.Value * Time.deltaTime);

                if (Vector3.Distance(transform.position, GameManager.Instance.endPath.position) < 0.1f)
                {
                    GameManager.Instance.Lives--;
                    Destroy(gameObject);
                }
            }
            else
            {
                targetPosition = GameManager.Instance.GetNextPathPoint(targetPosition);
                enemyBody.rotation = Quaternion.LookRotation(targetPosition.position - transform.position);
            }
        }
        #endregion
    }
    [System.Serializable]
    public class EnemyStats
    {
        public Stat moveSpeed;
        public Stat health;
        public Stat damage;
        public Stat armor;
        public Stat money;
    }
}