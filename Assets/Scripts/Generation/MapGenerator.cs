using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Entropy.TD.Map
{
    [DisallowMultipleComponent]
    public class MapGenerator : MonoBehaviour
    {
        [BoxGroup("Map Settings")] public Vector2 mapSize = new Vector2(10, 10);
        [BoxGroup("Map Settings")] public MapTileset tileSet;

        private List<PathTile> _straightPaths;
        private List<PathTile> _cornerPaths;
        private List<PathTile> _threewayPaths;
        private List<PathTile> _intersectionPaths;

        public Dictionary<Vector3, TileData> tiles = new Dictionary<Vector3, TileData>();
        [ReadOnly] public List<Vector3> openSpots = new List<Vector3>();
        [ReadOnly] public PathData pathData = new PathData();
        int _tileSetWeight = 0;
        int _currentMapPiece = 0;

        public void CachePathTiles()
        {
            _straightPaths = new List<PathTile>();
            _cornerPaths = new List<PathTile>();
            _threewayPaths = new List<PathTile>();
            _intersectionPaths = new List<PathTile>();

            for (int i = 0; i < tileSet.pathTiles.Count; i++)
            {
                if (tileSet.pathTiles[i].tileData.pathTileType == PathTileType.I) { _straightPaths.Add(tileSet.pathTiles[i]); }
                if (tileSet.pathTiles[i].tileData.pathTileType == PathTileType.L) { _cornerPaths.Add(tileSet.pathTiles[i]); }
                if (tileSet.pathTiles[i].tileData.pathTileType == PathTileType.T) { _threewayPaths.Add(tileSet.pathTiles[i]); }
                if (tileSet.pathTiles[i].tileData.pathTileType == PathTileType.X) { _intersectionPaths.Add(tileSet.pathTiles[i]); }
            }
        }
        public MapGrid GenerateMapGrid()
        {
            PrepareForMapGeneration();

            GeneratePathTiles();
            GenerateMapTiles();

            MapGrid mapGrid = CombineMeshesAndCreateMapGrid();
            mapGrid.tiles = tiles;
            _currentMapPiece++;
            return mapGrid;
        }
        public void PrepareForMapGeneration()
        {
            tiles = new Dictionary<Vector3, TileData>();
            openSpots = new List<Vector3>();
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    openSpots.Add(new Vector3(x, 0, y));
                }
            }
        }
        public MapGrid GenerateInitialMapGrid()
        {
            Debug.Log("Temporary Map Generation!");
            SetTileSetWeight();
            return GenerateMapGrid();
        }
        public void SetTileSetWeight()
        {
            _tileSetWeight = 0;
            for (int i = 0; i < tileSet.mapTiles.Count; i++)
            {
                _tileSetWeight += tileSet.mapTiles[i].weight;
            }
        }
        public MapTile GetRandomMapTile()
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
        public void GeneratePathTiles()
        {
            if (tileSet.pathTiles.Count < 1) { Debug.LogError("Map Generator trying to create path without a TileSet!"); return; }

            GameObject prefab = Instantiate(_straightPaths[Random.Range(0, _straightPaths.Count)].pathTile, Vector3.zero, Quaternion.identity);
            foreach(var pos in prefab.GetComponentsInChildren<Transform>())
            {
                if(prefab != pos.gameObject)
                {
                    openSpots.Remove(pos.position);
                    pos.transform.SetParent(gameObject.transform);
                }
            }
            Destroy(prefab);
        }
        #endregion

        #region Mesh
        private void GenerateMapTiles()
        {
            if (tileSet.mapTiles.Count < 1) { Debug.LogError("Map Generator trying to create map tiles without a TileSet!"); return; }
            for (int i = 0; i < openSpots.Count; i++)
            {
                GameObject tile = Instantiate(GetRandomMapTile().tilePrefab, openSpots[i], Quaternion.Euler(new Vector3(0, GetRandomTileRotation(), 0)));
                tile.transform.SetParent(transform);
            }
            //for (int x = 0; x < mapSize.x; x++)
            //{
            //    for (int y = 0; y < mapSize.y; y++)
            //    {
            //        GameObject tile = Instantiate(GetRandomMapTile().tilePrefab, new Vector3(x, 0, y), Quaternion.Euler(new Vector3(0, GetRandomTileRotation(), 0)));
            //        tile.transform.SetParent(transform);
            //    }
            //}
        }
        private MapGrid CombineMeshesAndCreateMapGrid()
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
            return new MapGrid()
            {
                mapMesh = meshFilterCombine,
                mapObj = obj,
            };
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
}