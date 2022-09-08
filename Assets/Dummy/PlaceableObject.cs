using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlaceableObject : MonoBehaviour
{
    public Renderer renderer;
    public MaterialPropertyBlock PropertyBlock;
    public bool Placed { get; private set; }
    public Vector3Int Size { get; private set; }
    private Vector3[] _vertices;
    private bool zaa = false;

    public Color ghostColor;
    public Color normalColor;

    private void Start()
    {
        PropertyBlock ??= new MaterialPropertyBlock();
        renderer = GetComponentInChildren<Renderer>();

        //normalColor = myRenderer.material.color;
        //ghostColor = normalColor - new Color(0, 0, 0, 128);
        PropertyBlock.SetColor("_Color", ghostColor);
        renderer.SetPropertyBlock(PropertyBlock);
        
        GetColliderVertexPositionsLocal();
        CalculateSizeInCells();
    }

    private void GetColliderVertexPositionsLocal()
    {
        var boxCollider = GetComponent<BoxCollider>();
        var size = boxCollider.size;
        var center = boxCollider.center;
        size *= 0.99f;

        _vertices = new Vector3[4];

        /*
         var localBounds = new Bounds(center, size);
         _vertices[0] = new Vector3(localBounds.min.x, localBounds.min.y, localBounds.min.z);
        _vertices[1] = new Vector3(localBounds.min.x, localBounds.min.y, localBounds.max.z);
        _vertices[2] = new Vector3(localBounds.max.x, localBounds.min.y, localBounds.max.z);
        _vertices[3] = new Vector3(localBounds.max.x, localBounds.min.y, localBounds.min.z);
        */

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
            vertices[i] = BuildingManager.Instance.gridLayout.WorldToCell(worldPos);
        }

        var xValue = Mathf.Abs((vertices[0] - vertices[1]).x);
        var yValue = Mathf.Abs((vertices[0] - vertices[3]).y);
        Size = new Vector3Int(xValue, yValue, 1);
    }

    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(_vertices[0]);
    }

    public virtual void Place()
    {
        gameObject.TryGetComponent<ObjectDrag>(out var objectDrag);
        Destroy(objectDrag);
        Placed = true;
        
        PropertyBlock.SetColor("_Color", normalColor);
        renderer.SetPropertyBlock(PropertyBlock);

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

    private void OnDrawGizmos()
    {
        var boxCollider = GetComponent<BoxCollider>();

        var localBounds = new Bounds(boxCollider.center, boxCollider.size);

        const float radius = 0.1f;

        Gizmos.color = Color.green;

        Gizmos.DrawSphere(
            transform.TransformPoint(new Vector3(localBounds.min.x, localBounds.min.y, localBounds.min.z)), radius);
        Gizmos.DrawSphere(
            transform.TransformPoint(new Vector3(localBounds.min.x, localBounds.min.y, localBounds.max.z)), radius);
        Gizmos.DrawSphere(
            transform.TransformPoint(new Vector3(localBounds.max.x, localBounds.min.y, localBounds.max.z)), radius);
        Gizmos.DrawSphere(
            transform.TransformPoint(new Vector3(localBounds.max.x, localBounds.min.y, localBounds.min.z)), radius);

        /*Gizmos.color = Color.red;
        var boxCollider = GetComponent<BoxCollider>();
        var size = boxCollider.size;
        var center = boxCollider.center;
        Debug.Log(center);

        var vertices = new Vector3[4];

        vertices[0] = center + new Vector3(-size.x, -size.y, -size.z) * 0.5f;
        vertices[1] = center + new Vector3(size.x, -size.y, -size.z) * 0.5f;
        vertices[2] = center + new Vector3(size.x, -size.y, size.z) * 0.5f;
        vertices[3] = center + new Vector3(-size.x, -size.y, size.z) * 0.5f;

        foreach (var t in vertices)
        {
            Gizmos.DrawSphere(t, 0.2f);
        }
        if (!zaa)
        {
            zaa = true;
            for (var i = 0; i < vertices.Length; i++)
            {
                var t = vertices[i];
                var asd = new GameObject
                {
                    transform =
                    {
                        position = t
                    },
                    name = i.ToString()
                };
            }
        }*/
    }
}