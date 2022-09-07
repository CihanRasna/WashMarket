using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem currentSystem;

    public GridLayout gridLayout;
    private Grid _grid;
    [SerializeField] private Tilemap MainTilemap;
    [SerializeField] private TileBase whiteTile;

    public PlaceableObject prefab1;
    public PlaceableObject prefab2;

    private PlaceableObject objectToPlace;

    private static Camera _mainCamera;

    private void Awake()
    {
        currentSystem = this;
        _mainCamera = Camera.main;
        _grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            InitializeWithObject(prefab1);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            InitializeWithObject(prefab2);
        }
    }

    public static Vector3 GetMouseWorldPosition()
    {
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray,out var raycastHit) ? raycastHit.point : Vector3.zero;
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        var cellPos = gridLayout.WorldToCell(position);
        position = _grid.GetCellCenterWorld(cellPos);
        return position;
    }

    public void InitializeWithObject(PlaceableObject prefab)
    {
        var pos = SnapCoordinateToGrid(Vector3.zero);
        var obj = Instantiate(prefab, pos, Quaternion.identity);
        objectToPlace = obj;
        obj.AddComponent<ObjectDrag>();
    }
}