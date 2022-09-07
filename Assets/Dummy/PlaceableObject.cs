using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public bool Placed { get; private set; }
    public Vector3Int Size { get; private set; }
    private Vector3[] _vertices;

    private void GetColliderVertexPositionsLocal()
    {
        var boxCollider = GetComponent<BoxCollider>();
        var size = boxCollider.size;
        var center = boxCollider.center;

        _vertices = new Vector3[4];

        _vertices[0] = center + new Vector3(-size.x, -size.y, -size.z) * 0.5f;
        _vertices[1] = center + new Vector3(size.x, -size.y, -size.z) * 0.5f;
        _vertices[2] = center + new Vector3(size.x, -size.y, size.z) * 0.5f;
        _vertices[3] = center + new Vector3(-size.x, -size.y, size.z) * 0.5f;
    }

    private void CalculateSizeInCells()
    {
        var vertices = new Vector3Int[_vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            var worldPos = transform.TransformPoint(_vertices[i]);
            vertices[i] = BuildingSystem.currentSystem.gridLayout.WorldToCell(worldPos);
        }

        var xValue = Mathf.Abs((vertices[0] - vertices[1]).x);
        var yValue = Mathf.Abs((vertices[0] - vertices[3]).y);
        Size = new Vector3Int(xValue, yValue, 1);
    }

    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(_vertices[0]);
    }

    private void Start()
    {
        GetColliderVertexPositionsLocal();
        CalculateSizeInCells();
    }

    public virtual void Place()
    {
        gameObject.TryGetComponent<ObjectDrag>(out var objectDrag);
        Destroy(objectDrag);
        Placed = true;

        //Placement events=???
    }

    public void Rotate() // CAN USE WITH BUTTON?
    {
        transform.Rotate(new Vector3(0, 90, 0));
        Size = new Vector3Int(Size.y, Size.x, 1);
        var vertices = new Vector3[_vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] = _vertices[(i + 1) % _vertices.Length];
        }

        _vertices = vertices;
    }
}