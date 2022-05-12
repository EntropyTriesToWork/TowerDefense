using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Entities
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        public void Heal(int healAmount)
        {
            throw new System.NotImplementedException();
        }

        public DamageReport TakeDamage(DamageInfo damageInfo)
        {
            throw new System.NotImplementedException();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}