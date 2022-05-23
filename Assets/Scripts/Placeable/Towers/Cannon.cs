using Entropy.TD.Entities;
using Entropy.TD.Tower.Projectiles;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Tower
{
    public class Cannon : Tower
    {
        [BoxGroup("Rotation")] public Transform cannonBase;
        [BoxGroup("Rotation")] public Transform cannonBody;

        [BoxGroup("Firing")] public Transform cannonballSpawnpoint;

        public override bool CanPlace()
        {
            throw new System.NotImplementedException();
        }

        public override void FireProjectile()
        {
            Projectile arrow = Instantiate(projectilePrefab, cannonballSpawnpoint.position, cannonballSpawnpoint.rotation).GetComponent<Projectile>();
            arrow.Initialize(currentTarget.transform.position, ProjectileSpeed, GetDamageInfo());
            arrow.transform.forward = cannonballSpawnpoint.forward;
        }

        public override void LookAtTarget()
        {
            cannonBase.LookAt(currentTarget.transform);
            cannonBase.localEulerAngles = new Vector3(0, cannonBase.localEulerAngles.y, 0);

            cannonBody.LookAt(currentTarget.transform);
            cannonBody.localEulerAngles = new Vector3(cannonBody.localEulerAngles.x + DistanceToTarget, 0, 0);
        }

        public override void OnPlaceDown()
        {
            throw new System.NotImplementedException();
        }
    }
}