using System;
using UnityEngine;

public class ObjectPivotAdjuster : MonoBehaviour
{
    public RectTransform canvasRectTransform; // Assign Canvas RectTransform in Inspector
    public RectTransform anchorRectTransform, anchorContainerRectTransform; // Assign Parent RectTransform in Inspector
    public Camera camera;
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
        if (canvasRectTransform != null && anchorRectTransform != null)
            CheckAndChangePivot();
    }

    public void RecalculateOffsets()
    {
        Debug.Log($"{gameObject.name}: parent width = {anchorRectTransform.rect.width}, height = {anchorRectTransform.rect.height}");
        float offsetX = anchorRectTransform.rect.width / 2,
            offsetY = anchorRectTransform.rect.height / 2;
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
            float minExcessX = float.MaxValue;
            float minExcessY = float.MaxValue;
            int bestIndex = -1;
            Vector3 bestPosition = Vector3.zero;
            Vector2 bestPivot = rectTransform.pivot;

            // Try all 4 pivots
            for (int i = 0; i < 4; i++)
            {
                rectTransform.pivot = pivots[i];
                rectTransform.localPosition = anchorContainerRectTransform.localPosition + anchorRectTransform.localPosition + (Vector3)offsets[i];

                Vector3[] worldCorners = new Vector3[4];
                rectTransform.GetWorldCorners(worldCorners);

                bool inCanvas = true;
                float excessX = 0f;
                float excessY = 0f;

                for (int c = 0; c < 4; c++)
                {
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRectTransform,
                        RectTransformUtility.WorldToScreenPoint(camera, worldCorners[c]),
                        camera,
                        out localPoint
                    );

                    if (!canvasRectTransform.rect.Contains(localPoint))
                    {
                        inCanvas = false;

                        // Вычисляем, сколько заходит за границы canvas по X
                        if (localPoint.x < canvasRectTransform.rect.xMin)
                            excessX = Mathf.Max(excessX, canvasRectTransform.rect.xMin - localPoint.x);
                        else if (localPoint.x > canvasRectTransform.rect.xMax)
                            excessX = Mathf.Max(excessX, localPoint.x - canvasRectTransform.rect.xMax);

                        // И по Y
                        if (localPoint.y < canvasRectTransform.rect.yMin)
                            excessY = Mathf.Max(excessY, canvasRectTransform.rect.yMin - localPoint.y);
                        else if (localPoint.y > canvasRectTransform.rect.yMax)
                            excessY = Mathf.Max(excessY, localPoint.y - canvasRectTransform.rect.yMax);
                    }
                }

                if (inCanvas)
                {
                    Debug.Log($"{gameObject.name}: i = {i}");
                    return; // Нашли подходящий вариант — возвращаемся
                }
                else
                {
                    // Сохраняем вариант с наименьшим выходом за границы
                    // Сравним по суммарному избыточному выходу по X и Y
                    float totalExcess = excessX + excessY;
                    float bestTotalExcess = minExcessX + minExcessY;
                    if (totalExcess < bestTotalExcess)
                    {
                        minExcessX = excessX;
                        minExcessY = excessY;
                        bestIndex = i;
                        bestPosition = anchorContainerRectTransform.localPosition + anchorRectTransform.localPosition + (Vector3)offsets[i];
                        bestPivot = pivots[i];
                    }
                }
            }

            // Если ни один вариант не поместился, применяем вариант с минимальным выходом за границы
            if (bestIndex != -1)
            {
                rectTransform.pivot = bestPivot;
                rectTransform.localPosition = bestPosition;
                Debug.Log($"excess x,y: {minExcessX}, {minExcessY}");
            }
        }
        catch (NullReferenceException e)
        {
            return;
        }
    }

}
