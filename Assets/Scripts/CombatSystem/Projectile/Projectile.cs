using Entropy.TD.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Tower.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Projectile : MonoBehaviour
    {
        public Vector3 targetPosition;
        public float moveSpeed;
        public DamageInfo damageInfo;
        Rigidbody _rb;
        public GameObject projectileHitEffect;
        public float areaEffectSize = 2.5f;

        public bool hasGravity;
        public bool areaOfEffect;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public virtual void Initialize(Vector3 targetPosition, float moveSpeed, DamageInfo damageInfo, float lifespan = 10f)
        {
            _rb = GetComponent<Rigidbody>();
            this.targetPosition = targetPosition;
            this.moveSpeed = moveSpeed;
            this.damageInfo = damageInfo;

            if (hasGravity)
            {
                _rb.AddForce((targetPosition - transform.position).normalized * moveSpeed, ForceMode.Impulse);
            }

            Destroy(gameObject, lifespan);
        }

        public void Update()
        {
            if (!hasGravity)
            {
                _rb.MovePosition((targetPosition - transform.position).normalized * moveSpeed * Time.deltaTime + transform.position);

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ground"))
            {
                Destroy(gameObject);
                Destroy(Instantiate(projectileHitEffect, transform.position, Quaternion.identity));
            }
            if (other.CompareTag("Enemy") && !areaOfEffect)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    other.GetComponent<IDamageable>().TakeDamage(damageInfo);
                    Destroy(gameObject);
                }
            }
            Destroy(Instantiate(projectileHitEffect, transform.position, Quaternion.identity), 4f);
            if (areaOfEffect)
            {
                foreach (var hit in Physics.OverlapSphere(transform.position, areaEffectSize, LayerMask.GetMask("Enemy")))
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        other.GetComponent<IDamageable>().TakeDamage(damageInfo);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}