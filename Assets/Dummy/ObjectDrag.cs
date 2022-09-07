using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;

    private void OnMouseDown()
    {
        offset = transform.position - BuildingSystem.GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        var pos = BuildingSystem.GetMouseWorldPosition() + offset;
        transform.position = BuildingSystem.currentSystem.SnapCoordinateToGrid(pos);
    }
}
