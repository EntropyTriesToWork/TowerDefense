using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Entities
{
    public interface IDamageable
    {
        public DamageReport TakeDamage(DamageInfo damageInfo);
        public void Heal(int healAmount);
    }
}