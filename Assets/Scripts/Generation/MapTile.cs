using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapTile
{
    public GameObject tilePrefab;
    public int weight;
    public MapTileData tileData;
}
[System.Serializable]
public class MapTileData
{
    public MapTileType mapTileType;
    public bool N, E, S, W;

    public MapTileData()
    {
        mapTileType = MapTileType.X;
        N = true;
        E = true;
        S = true;
        W = true;
    }
}
public enum MapTileType
{
    X, //4 Directions
    L, //Two Directions next to each other
    T, //Three direction
    I, //Two Directions parallel
    O // End point tile
}
public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
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
    public List<MapTile> pathTiles;
    public List<MapTile> mapTiles;
}