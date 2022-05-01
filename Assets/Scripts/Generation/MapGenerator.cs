using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[DisallowMultipleComponent]
public class MapGenerator : MonoBehaviour
{
    [BoxGroup("Map Settings")] public Vector2 mapSize = new Vector2(10, 10);
    [BoxGroup("Map Settings")] public List<MapTile> mapTiles;

    [BoxGroup("Read Only")] [ReadOnly] public List<GameObject> mapTileList = new List<GameObject>();

    [Button]
    public void CreateNewMap()
    {
        ClearMapData();
        if (mapTiles.Count < 1) { return; }
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                GameObject prefab = mapTiles[Random.Range(0, mapTiles.Count)].tilePrefab;

                if (prefab != null)
                {
                    GameObject tile = Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity);
                    tile.transform.SetParent(transform);
                    mapTileList.Add(tile);
                }
            }
        }
        CombineMeshes();
    }
    [Button]
    public void RecreateMap()
    {
        if (mapTileList.Count < 1) { return; }
        for (int i = 0; i < mapTileList.Count; i++)
        {
            mapTileList[i].SetActive(true);
        }
        CombineMeshes();
    }
    [Button]
    public void ClearMapData()
    {
        for (int i = 0; i < mapTileList.Count; i++)
        {
            Destroy(mapTileList[i]);
        }
        mapTileList.Clear();
    }
    [Button]
    public void DisableMapTiles()
    {
        for (int i = 0; i < mapTileList.Count; i++)
        {
            mapTileList[i].SetActive(false);
        }
    }
    [Button]
    public void CombineMeshes()
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
                int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].name);
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
        // Get / Create mesh filter & renderer
        MeshFilter meshFilterCombine = gameObject.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
        {
            meshFilterCombine = gameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
        {
            meshRendererCombine = gameObject.AddComponent<MeshRenderer>();
        }
        // Combine by material index into per-material meshes
        // also, Create CombineInstance array for next step
        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }
        // Combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);
        // Destroy other meshes
        foreach (Mesh oldMesh in meshes)
        {
            oldMesh.Clear();
            DestroyImmediate(oldMesh);
        }
        // Assign materials
        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;
        //foreach (MeshFilter meshFilter in meshFilters)
        //{
        //    if (meshFilter == null || meshFilter.transform == transform) { continue; }
        //    DestroyImmediate(meshFilter.gameObject);
        //}
        DisableMapTiles();
    }
    private int Contains(ArrayList searchList, string searchName)
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

    #region Editor
    [BoxGroup("Editor")] public float gizmoSize = 0.1f;
    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Gizmos.DrawSphere(new Vector3(x, 0, y), gizmoSize);
            }
        }
    }
    #endregion
}
