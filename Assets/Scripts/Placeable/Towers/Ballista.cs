using Entropy.TD.Entities;
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

        [Button]
        public override void FireProjectile()
        {
            GameObject arrow = Instantiate(projectilePrefab, ballistaArrowSpawnPoint.position, ballistaArrowSpawnPoint.rotation);
        }

        public override List<IDamageable> GetTargets()
        {
            throw new System.NotImplementedException();
        }

        public override void LookAtTarget()
        {
            ballistaBase.LookAt(testTarget.transform);
            ballistaBase.localEulerAngles = new Vector3(0, ballistaBase.localEulerAngles.y, 0);

            ballistaBow.LookAt(testTarget.transform);
            ballistaBow.localEulerAngles = new Vector3(ballistaBow.localEulerAngles.x, 0, 0);
        }

        public override void OnPlaceDown()
        {
            throw new System.NotImplementedException();
        }
    }
}