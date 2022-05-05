using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Map
{
    [System.Serializable]
    public class PathTile
    {
        public GameObject pathTile;
        public int weight;
        public PathTileData tileData;
    }
    [System.Serializable]
    public class PathTileData
    {
        public PathTileType pathTileType;
        public bool N, E, S, W;

        public PathTileData()
        {
            pathTileType = PathTileType.X;
            N = true;
            E = true;
            S = true;
            W = true;
        }
    }
    public enum PathTileType
    {
        X, //4 Directions
        L, //Two Directions next to each other
        T, //Three direction
        I, //Two Directions parallel
        O // End point tile
    }
}
