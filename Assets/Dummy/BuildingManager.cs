using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingManager : RSNManagers.Singleton<BuildingManager>
{
    public GridLayout gridLayout;
    private Grid _grid;
    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private TileBase whiteTile;

    public PlaceableObject prefab1;
    public PlaceableObject prefab2;

    private PlaceableObject _objectToPlace;

    private static Camera _mainCamera;

    #region Life Cycle

    protected override void Awake()
    {
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

        if (!_objectToPlace)
        {
            return;
        }
        

        if (Input.GetKeyDown(KeyCode.R))
        {
            _objectToPlace.Rotate();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CanPlaceableAtPosition(_objectToPlace))
            {
                _objectToPlace.Place();
                var start = gridLayout.WorldToCell(_objectToPlace.GetStartPosition());
                TakeArea(start,_objectToPlace.Size);
            }
            else
            {
                Destroy(_objectToPlace.gameObject);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(_objectToPlace.gameObject);
        }
    }

    #endregion

    #region Utils

    public static Vector3 GetMouseWorldPosition()
    {
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out var raycastHit) ? raycastHit.point : Vector3.zero;
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        var cellPos = gridLayout.WorldToCell(position);
        position = _grid.GetCellCenterWorld(cellPos);
        return position + Vector3.up;
    }

    private static IEnumerable<TileBase> GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        var size = area.size;
        var array = new TileBase[size.x * size.y * size.z];
        var counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            var pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter += 1;
        }

        return array;
    }

    #endregion

    #region Placement

    private void InitializeWithObject(PlaceableObject prefab)
    {
        var pos = SnapCoordinateToGrid(GetMouseWorldPosition());
        var obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.AddComponent<ObjectDrag>();
        _objectToPlace = obj;
    }

    private bool CanPlaceableAtPosition(PlaceableObject placeableObject)
    {
        var area = new BoundsInt
        {
            position = gridLayout.WorldToCell(_objectToPlace.GetStartPosition()),
            size = placeableObject.Size
        };

        var baseArray = GetTilesBlock(area, mainTilemap);

        foreach (var tileBase in baseArray)
        {
            if (tileBase == whiteTile)
            {
                return false;
            }
        }

        return true;
    }

    private void TakeArea(Vector3Int start, Vector3Int size)
    {
        mainTilemap.BoxFill(start, whiteTile, start.x, start.y, start.x + size.x, start.y + size.y);
    }

    #endregion
}