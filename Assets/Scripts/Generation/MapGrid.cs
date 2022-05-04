using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    public GameObject mapObj;
    public MeshFilter mapMesh;
    public List<PathData> path;
}
public class PathData
{
    public List<Vector3> pathPoints;
    public Vector3 spawnPoint;
    public Vector3 endPoint;
}