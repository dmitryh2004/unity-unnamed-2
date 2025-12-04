using UnityEngine;

public class SelectiveContentSizeFitter : MonoBehaviour
{
    [SerializeField] float padding = 5f;
    [SerializeField] RectTransform child;
    RectTransform self;
    float oldHeight = 0f;

    private void Awake()
    {
        self = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (child == null) return;

        // Вычисляем новую высоту: высота child + 2 * padding
        float newHeight = child.rect.height + 2f * padding;
        if (newHeight != oldHeight)
        {
            oldHeight = newHeight;
            // Устанавливаем высоту self (sizeDelta.y управляет размером относительно anchors)
            Vector2 currentSize = self.sizeDelta;
            self.sizeDelta = new Vector2(currentSize.x, newHeight);

            // Позиционируем child: y = padding (относительно pivot родителя)
            // Предполагаем стандартный pivot (0.5, 0.5) и anchor (stretch или top)
            child.anchoredPosition = new Vector2(child.anchoredPosition.x, padding);
        }
    }
}

