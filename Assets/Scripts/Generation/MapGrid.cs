using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Map
{
    public class MapGrid
    {
        public GameObject mapObj;
        public MeshFilter mapMesh;
        public List<PathData> path;
        public Dictionary<Vector3, TileData> tiles = new Dictionary<Vector3, TileData>();
    }
    public class PathData
    {
        public List<Vector3> pathPoints;
        public Vector3 spawnPoint;
        public Vector3 endPoint;
    }
}
