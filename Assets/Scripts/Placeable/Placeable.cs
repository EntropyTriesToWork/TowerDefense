using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Tower
{
    public abstract class Placeable : MonoBehaviour
    {
        public abstract bool CanPlace();
        public abstract void OnPlaceDown();
    }
}