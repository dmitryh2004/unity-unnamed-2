using UnityEngine;

public class ObjectPivotAdjuster : MonoBehaviour
{
    public RectTransform canvasRectTransform; // Assign Canvas RectTransform in Inspector
    public RectTransform parentRectTransform; // Assign Parent RectTransform in Inspector
    private RectTransform rectTransform;

    // Positions relative to parent: (pivot, localPosition)
    private Vector2[] pivots = {
        new Vector2(0, 1), // Top-Left
        new Vector2(1, 1), // Top-Right
        new Vector2(0, 0), // Bottom-Left
        new Vector2(1, 0)  // Bottom-Right
    };
    private Vector2[] offsets;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        offsets = new Vector2[] {
            new Vector2(0, 0),    // Top-Left
            new Vector2(parentRectTransform.rect.width, 0),       // Top-Right
            new Vector2(0, -parentRectTransform.rect.height),     // Bottom-Left
            new Vector2(parentRectTransform.rect.width, -parentRectTransform.rect.height) // Bottom-Right
        };
    }

    private void LateUpdate()
    {
        if (canvasRectTransform != null && parentRectTransform != null)
            CheckAndChangePivot();
    }

    void CheckAndChangePivot()
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
                return; // Found a suitable pivot and position
        }
        // If none fit, keep the last tried pivot and position
    }
}
