using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[DisallowMultipleComponent]
public class MapGenerator : MonoBehaviour
{
    [BoxGroup("Map Settings")] public Vector2 mapSize = new Vector2(10, 10);
    [BoxGroup("Map Settings")] public MapTileset tileSet;

    public Dictionary<Vector3, TileData> tiles = new Dictionary<Vector3, TileData>();
    int _tileSetWeight = 0;
    int _currentMapPiece = 0;

    [Button]
    public void GenerateMap()
    {
        SetTileSetWeight();
        if (tileSet.mapTiles.Count < 1) { return; }
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                GameObject tile = Instantiate(GetRandomTile().tilePrefab, new Vector3(x, 0, y), Quaternion.Euler(new Vector3(0, GetRandomTileRotation(), 0)));
                tile.transform.SetParent(transform);
            }
        }
        CombineMeshes();
        MapManager.Instance.mapSize += new Vector2(mapSize.x, 0);
        _currentMapPiece++;
    }
    [Button]
    public void GenerateMapData()
    {
        Debug.Log("Starting Map Generation...");
        tiles = new Dictionary<Vector3, TileData>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                tiles.Add(new Vector3(x, 0, y), new TileData(GetRandomTile(), new Vector3(x, 0, y)));
            }
        }
        Debug.Log("Generated " + tiles.Count + " total tiles.");

        //Insert preset tiles here? 

        for (int i = 0; i < tiles.Count; i++)
        {

        }
    }
    public void SetTileSetWeight()
    {
        _tileSetWeight = 0;
        for (int i = 0; i < tileSet.mapTiles.Count; i++)
        {
            _tileSetWeight += tileSet.mapTiles[i].weight;
        }
    }
    public MapTile GetRandomTile()
    {
        if (tileSet.mapTiles.Count < 1) { return tileSet.basicTile; }

        int rand = Random.Range(0, _tileSetWeight);
        for (int i = 0; i < tileSet.mapTiles.Count; i++)
        {
            rand -= tileSet.mapTiles[i].weight;
            if (rand <= 0) { return tileSet.mapTiles[i]; }
        }
        return tileSet.basicTile;
    }

    #region Path
    [Button]
    public void GeneratePaths()
    {
        Debug.Log("Generating Paths...");
    }
    #endregion

    #region Mesh
    [Button]
    public MeshFilter CombineMeshes()
    {
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.transform == transform) { continue; }
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (!meshRenderer || !meshFilter.sharedMesh || meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount) { continue; }
            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = ContainsMaterial(materials, meshRenderer.sharedMaterials[s].name);
                if (materialArrayIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    materialArrayIndex = materials.Count - 1;
                }
                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }
        GameObject obj = new GameObject("Map Piece: " + _currentMapPiece);
        // Get / Create mesh filter & renderer
        MeshFilter meshFilterCombine = obj.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
        {
            meshFilterCombine = obj.AddComponent<MeshFilter>();
        }
        MeshRenderer meshRendererCombine = obj.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
        {
            meshRendererCombine = obj.AddComponent<MeshRenderer>();
        }
        MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = obj.AddComponent<MeshCollider>();
        }
        // Combine by material index into per-material meshes
        // also, Create CombineInstance array for next step
        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }
        // Combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        //meshFilterCombine.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);
        meshFilterCombine.sharedMesh.Optimize();
        meshCollider.sharedMesh = meshFilterCombine.sharedMesh;
        // Destroy other meshes
        foreach (Mesh oldMesh in meshes)
        {
            oldMesh.Clear();
            DestroyImmediate(oldMesh);
        }
        // Assign materials
        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter == null || meshFilter.transform == transform) { continue; }
            DestroyImmediate(meshFilter.gameObject);
        }

        StaticBatchingUtility.Combine(obj);
        obj.transform.position = new Vector3(mapSize.x * _currentMapPiece, 0, 0);
        MapManager.Instance.maps.Add(meshFilterCombine);
        return meshFilterCombine;
    }
    private int ContainsMaterial(ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).name == searchName)
            {
                return i;
            }
        }
        return -1;
    }
    int[] _tileRots = new int[] { 90, 180, -90, -180 };
    public int GetRandomTileRotation()
    {
        return _tileRots[Random.Range(0, _tileRots.Length)];
    }
    #endregion

    #region Getters
    public TileData GetAdjacentTile(TileData tile, Direction dir)
    {
        Vector3 pos = tile.worldPos;
        switch (dir)
        {
            case Direction.North:
                pos += Vector3.forward;
                break;
            case Direction.East:
                pos += Vector3.right;
                break;
            case Direction.South:
                pos += -Vector3.forward;
                break;
            case Direction.West:
                pos += -Vector3.right;
                break;
            default:
                break;
        }
        if (tiles.ContainsKey(pos))
        {
            return tiles[pos];
        }
        return new TileData();
    }
    #endregion

    #region Editor
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(mapSize.x / 2f - 0.5f, 0, mapSize.y / 2f - 0.5f), new Vector3(mapSize.x, 1, mapSize.y));
    }
    #endregion
}