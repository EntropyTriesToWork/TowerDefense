using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapGeneratorNoise : MonoBehaviour
{
    [FoldoutGroup("Noise Settings")] [MinValue(1)] public int mapSize = 7;
    [FoldoutGroup("Noise Settings")] [MinValue(0.01f)] public float mapScale = 1;
    [FoldoutGroup("Noise Settings")] [MinValue(1)] public int octaves = 4;
    [FoldoutGroup("Noise Settings")] [MinValue(0.01f)] public float persistance = 0.5f;
    [FoldoutGroup("Noise Settings")] [MinValue(0.01f)] public float lacunarity = 2;
    [FoldoutGroup("Noise Settings")] public int seed = 0;
    [FoldoutGroup("Noise Settings")] public Vector2 offset;

    [FoldoutGroup("Noise Settings")] [Button] public void RandomizeSeed() { seed = Random.Range(-100000, 100000); }

    [FoldoutGroup("Map Settings")] public float highlandHeight = 0.5f;
    [FoldoutGroup("Map Settings")] public float middlelandHeight = 0f;
    [FoldoutGroup("Map Settings")] public float lowlandHeight = -0.5f;

    [FoldoutGroup("Map Tile")] public GameObject highlandTile;
    [FoldoutGroup("Map Tile")] public GameObject middlelandTile;
    [FoldoutGroup("Map Tile")] public GameObject lowlandTile;
    [FoldoutGroup("Map Tile")] public GameObject pathTile;

    [FoldoutGroup("Map Tile")] public float tileSnappingDistance = 0.2f;
    List<GameObject> _tiles = new List<GameObject>();
    public List<Vector2> pathPositions = new List<Vector2>();

    [Button]
    public void GenerateMap()
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            DestroyImmediate(_tiles[i]);
        }
        _tiles = new List<GameObject>();

        float[,] noiseMap = NoiseGenerator.GenerateNoise(mapSize, mapScale, octaves, offset, persistance, lacunarity, seed);

        int tilesPerMeter = Mathf.RoundToInt(1f / tileSnappingDistance);
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                float height = Mathf.Round(tilesPerMeter * noiseMap[x, y]) / tilesPerMeter;
                Vector3 pos = new Vector3(x - mapSize / 2f, height, y - mapSize / 2f);
                GameObject obj;
                if (pathPositions.Contains(new Vector2(x, y)))
                {
                    obj = Instantiate(pathTile, transform);
                    obj.transform.position = new Vector3(pos.x, lowlandHeight, pos.z);
                }
                else
                {
                    if (height > middlelandHeight) { obj = Instantiate(highlandTile, transform); }
                    else if (height > lowlandHeight) { obj = Instantiate(middlelandTile, transform); }
                    else { obj = Instantiate(lowlandTile, transform); }

                    obj.transform.position = pos;
                }
                obj.transform.localScale = new Vector3(1, tilesPerMeter, 1);
                _tiles.Add(obj);
            }
        }
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        if (mapDisplay != null)
        {
            mapDisplay.GenerateMapDisplay(noiseMap, mapSize);
        }
    }
    [Button]
    public void GenerateMapWithRandomSeed()
    {
        RandomizeSeed();
        GenerateMap();
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
        GameObject obj = new GameObject("Seed: " + seed);
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
}
