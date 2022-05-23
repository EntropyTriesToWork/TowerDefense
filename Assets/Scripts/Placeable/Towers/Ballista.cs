using Entropy.TD.Entities;
using Entropy.TD.Tower.Projectiles;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Tower
{
    public class Ballista : Tower
    {
        [BoxGroup("Rotation")] public Transform ballistaBase;
        [BoxGroup("Rotation")] public Transform ballistaBow;

        [BoxGroup("Firing")] public Transform ballistaArrowSpawnPoint;

        public override bool CanPlace()
        {
            throw new System.NotImplementedException();
        }

        public override void FireProjectile()
        {
            Projectile arrow = Instantiate(projectilePrefab, ballistaArrowSpawnPoint.position, ballistaArrowSpawnPoint.rotation).GetComponent<Projectile>();
            arrow.Initialize(currentTarget.transform.position, ProjectileSpeed, GetDamageInfo());
            arrow.transform.forward = ballistaArrowSpawnPoint.forward;
        }

        public override void LookAtTarget()
        {
            ballistaBase.LookAt(currentTarget.transform);
            ballistaBase.localEulerAngles = new Vector3(0, ballistaBase.localEulerAngles.y, 0);

            ballistaBow.LookAt(currentTarget.transform);
            ballistaBow.localEulerAngles = new Vector3(ballistaBow.localEulerAngles.x, 0, 0);
        }

        public override void OnPlaceDown()
        {
            throw new System.NotImplementedException();
        }
    }
}