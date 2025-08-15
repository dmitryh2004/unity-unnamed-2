using System;
using UnityEngine;

public class ObjectPivotAdjuster : MonoBehaviour
{
    public RectTransform canvasRectTransform; // Assign Canvas RectTransform in Inspector
    public RectTransform parentRectTransform; // Assign Parent RectTransform in Inspector
    private RectTransform rectTransform;

    // Positions relative to parent: (pivot, localPosition)
    private Vector2[] pivots = {
        new Vector2(0, 1),  // Bottom-Right
        new Vector2(1, 1), // Bottom-Left
        new Vector2(0, 0), // Top-Right
        new Vector2(1, 0) // Top-Left
    };
    private Vector2[] offsets;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        
    }

    private void LateUpdate()
    {
        if (canvasRectTransform != null && parentRectTransform != null)
            CheckAndChangePivot();
    }

    public void RecalculateOffsets()
    {
        Debug.Log($"{gameObject.name}: parent width = {parentRectTransform.rect.width}, height = {parentRectTransform.rect.height}");
        float offsetX = parentRectTransform.rect.width / 2,
            offsetY = parentRectTransform.rect.height / 2;
        offsets = new Vector2[] {
            new Vector2(offsetX, -offsetY), // Bottom-Right
            new Vector2(-offsetX, -offsetY), // Bottom-Left
            new Vector2(offsetX, offsetY), // Top-Right
            new Vector2(-offsetX, offsetY) // Top-Left
        };
    }

    void CheckAndChangePivot()
    {
        try
        {
            // Try all 4 pivots
            for (int i = 0; i < 4; i++)
            {
                rectTransform.pivot = pivots[i];
                rectTransform.localPosition = offsets[i];

                Vector3[] worldCorners = new Vector3[4];
                rectTransform.GetWorldCorners(worldCorners);

                Vector3[] canvasCorners = new Vector3[4];
                canvasRectTransform.GetWorldCorners(canvasCorners);

                bool inCanvas = true;
                for (int c = 0; c < 4; c++)
                {
                    if (worldCorners[c].x < canvasCorners[0].x || worldCorners[c].x > canvasCorners[2].x ||
                        worldCorners[c].y < canvasCorners[0].y || worldCorners[c].y > canvasCorners[2].y)
                    {
                        inCanvas = false;
                        break;
                    }
                }

                if (inCanvas)
                {
                    Debug.Log($"{gameObject.name}: i = {i}");
                    return; // Found a suitable pivot and position
                }
            }
        }
        catch (NullReferenceException e)
        {
            return;
        }
        // If none fit, keep the last tried pivot and position
    }
}
