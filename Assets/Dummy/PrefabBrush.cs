using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefab Brush", menuName = "Brushes/Prefab Brush")]
[CustomGridBrush(false, true, false, "Prefab Brush")]
public class PrefabBrush : GameObjectBrush
{
    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget.layer == 31)
        {
            return;
        }

        var erased = GetObjectInCell(gridLayout, brushTarget.transform, new Vector3Int(position.x,position.y,0));

        if (erased)
        {
            Undo.DestroyObjectImmediate(erased.gameObject);
        }
    }

    private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
    {
        var childCount = parent.childCount;
        var min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
        var max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one));
        var bounds = new Bounds((min + max) * 0.5f, max - min);

        for (var i = 0; i < childCount; i++)
        {
            var child = parent.GetChild(i);
            if (bounds.Contains(child.position))
            {
                return child;
            }
        }

        return null;
    }
}