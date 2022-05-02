using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public Vector2 mapSize;
    public List<Path> paths;
}
public class Path
{
    public List<Vector3> pathPoints;
    public Vector3 spawnPoint;
    public Vector3 endPoint;
}