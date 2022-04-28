using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Tower
{
    public abstract class Tower : MonoBehaviour
    {
        public abstract bool CanPlace();
        public abstract void CalculateBaseStats();
    }
}