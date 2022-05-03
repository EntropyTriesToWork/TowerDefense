using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public Vector2 mapSize;
    public Vector3 worldPosition;
    Plane plane = new Plane(Vector3.up, 0);
    public GameObject selectionIndicator;
    public GameObject selectionIndicatorPrefab;

    public List<MeshFilter> maps;

    private void Awake()
    {
        Instance = this;
        selectionIndicator = Instantiate(selectionIndicatorPrefab, Vector3.zero, Quaternion.identity);
        maps = new List<MeshFilter>();
    }
    void Update()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
        }
        MoveSelectionToMousePos();
    }
    public void MoveSelectionToMousePos()
    {
        if (Cursor.visible) { selectionIndicator.SetActive(true); }
        else { selectionIndicator.SetActive(false); }
        selectionIndicator.transform.position = GetClosestPoint();
    }
    public Vector3 GetClosestPoint()
    {
        return new Vector3(Mathf.Clamp(Mathf.RoundToInt(worldPosition.x), 0, mapSize.x - 1), 0, Mathf.Clamp(Mathf.RoundToInt(worldPosition.z), 0, mapSize.y - 1));
    }
}