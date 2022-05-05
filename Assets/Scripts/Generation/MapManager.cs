using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Entropy.TD.Map
{
    [RequireComponent(typeof(MapManager))]
    [DisallowMultipleComponent]
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance;

        [BoxGroup("Map Settings")] [ValueDropdown("MapSizes")] public int mapSize;
        Plane plane = new Plane(Vector3.up, 0);

        GameObject selectionIndicator;
        [BoxGroup("Prefabs")] public GameObject selectionIndicatorPrefab;
        [BoxGroup("Prefabs")] public GameObject expansionArrowPrefab;
        List<ExpansionArrow> _expansionArrows = new List<ExpansionArrow>();

        [BoxGroup("Read Only")] [ReadOnly] public Vector3 mousePosition;
        [BoxGroup("Read Only")] [ReadOnly] public List<MapGrid> mapGrids;

        public Dictionary<Vector3, MapGrid> AllMapGrids { get; set; }
        private MapGenerator _mapGenerator;

        #region Messages
        private void Awake()
        {
            Instance = this;
            selectionIndicator = Instantiate(selectionIndicatorPrefab, Vector3.zero, Quaternion.identity);
            AllMapGrids = new Dictionary<Vector3, MapGrid>();
            mapGrids = new List<MapGrid>();
            _mapGenerator = GetComponent<MapGenerator>();
            _mapGenerator.mapSize = Vector2.one * mapSize;
            _mapGenerator.CachePathTiles();

            MapGrid mapGrid = _mapGenerator.GenerateInitialMapGrid();
            AllMapGrids.Add(Vector3.zero, mapGrid);
            mapGrids.Add(mapGrid);
        }
        void Update()
        {
            float distance;
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                mousePosition = ray.GetPoint(distance);
            }
            MoveIndicatorToMousePos();
        }
        #endregion

        #region Expansion + New MapGrids
        public void ShowExpansionArrows()
        {
            DeleteExpansionArrows();
            for (int i = 0; i < mapGrids.Count; i++)
            {
                ExpansionArrow north = Instantiate(expansionArrowPrefab, new Vector3(mapSize / 2f - 0.5f, 0, mapSize / 2f - 0.5f + mapSize) + mapGrids[i].mapObj.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<ExpansionArrow>();
                north.Initialize(Direction.North, DirectionToVector(Direction.North) * mapSize + mapGrids[i].mapObj.transform.position, mapSize);

                ExpansionArrow east = Instantiate(expansionArrowPrefab, new Vector3(mapSize / 2f - 0.5f + mapSize, 0, mapSize / 2f - 0.5f) + mapGrids[i].mapObj.transform.position, Quaternion.Euler(0, 90, 0)).GetComponent<ExpansionArrow>();
                east.Initialize(Direction.East, DirectionToVector(Direction.East) * mapSize + mapGrids[i].mapObj.transform.position, mapSize);

                ExpansionArrow south = Instantiate(expansionArrowPrefab, new Vector3(mapSize / 2f - 0.5f, 0, mapSize / 2f - 0.5f - mapSize) + mapGrids[i].mapObj.transform.position, Quaternion.Euler(0, 180, 0)).GetComponent<ExpansionArrow>();
                south.Initialize(Direction.South, DirectionToVector(Direction.South) * mapSize + mapGrids[i].mapObj.transform.position, mapSize);

                ExpansionArrow west = Instantiate(expansionArrowPrefab, new Vector3(mapSize / 2f - 0.5f - mapSize, 0, mapSize / 2f - 0.5f) + mapGrids[i].mapObj.transform.position, Quaternion.Euler(0, -90, 0)).GetComponent<ExpansionArrow>();
                west.Initialize(Direction.West, DirectionToVector(Direction.West) * mapSize + mapGrids[i].mapObj.transform.position, mapSize);

                _expansionArrows.Add(north);
                _expansionArrows.Add(east);
                _expansionArrows.Add(south);
                _expansionArrows.Add(west);
            }
        }
        public void DeleteExpansionArrows()
        {
            if (_expansionArrows.Count > 0)
            {
                for (int i = 0; i < _expansionArrows.Count; i++)
                {
                    Destroy(_expansionArrows[i].gameObject);
                }
                _expansionArrows = new List<ExpansionArrow>();
            }
        }
        public void AddNewGridMap(Direction direction, Vector3 position)
        {
            MapGrid mapGrid = _mapGenerator.GenerateMapGrid();
            mapGrid.mapMesh.GetComponent<Transform>().position = position;
            AllMapGrids.Add(position, mapGrid);
            mapGrids.Add(mapGrid);

            DeleteExpansionArrows();
        }
        #endregion

        public void MoveIndicatorToMousePos()
        {
            if (Cursor.visible) { selectionIndicator.SetActive(true); }
            else { selectionIndicator.SetActive(false); }
            selectionIndicator.transform.position = Vector3.Slerp(selectionIndicator.transform.position, GetClosestPoint(), 0.2f);
        }

        #region Getters
        public List<MapGrid> GetOpenPaths()
        {
            List<MapGrid> mapGrids = new List<MapGrid>();



            return mapGrids;
        }
        public Vector3 GetClosestPoint()
        {
            return new Vector3(Mathf.RoundToInt(mousePosition.x), 0, Mathf.RoundToInt(mousePosition.z));
        }
        #endregion

        #region Static
        public static Vector3 DirectionToVector(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Vector3.forward;
                case Direction.East:
                    return Vector3.right;
                case Direction.South:
                    return -Vector3.forward;
                case Direction.West:
                    return -Vector3.right;
                default:
                    Debug.LogError("Undefined Direction selected!");
                    return Vector3.zero;
            }
        }
        #endregion

        #region Editor
        private static int[] MapSizes = new int[] { 3, 5, 7, 9, 11, 13, 15 };
        #endregion
    }
}