using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.TD.Map
{
    [System.Serializable]
    public class MapTile
    {
        public GameObject tilePrefab;
        public int weight;
    }
    public struct TileData
    {
        public MapTile tileData;
        public Vector3 worldPos;

        public TileData(MapTile mapTile, Vector3 worldPos)
        {
            tileData = mapTile;
            this.worldPos = worldPos;
        }
    }
    [System.Serializable]
    public struct MapTileset
    {
        public MapTile basicTile;
        public MapTile playerCastle;
        public PathTile enemySpawnTile;
        public List<PathTile> pathTiles;
        public List<MapTile> mapTiles;
    }
}
public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}