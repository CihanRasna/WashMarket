using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;

    private void Awake()
    {
        offset = transform.position - BuildingManager.GetMouseWorldPosition();
    }

    private void Update()
    {
        var pos = BuildingManager.GetMouseWorldPosition() + offset;
        transform.position = BuildingManager.Instance.SnapCoordinateToGrid(pos);
    }

    private void OnMouseDrag()
    {
        var pos = BuildingManager.GetMouseWorldPosition() + offset;
        transform.position = BuildingManager.Instance.SnapCoordinateToGrid(pos);
    }
}
