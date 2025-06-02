using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image fillImage; // Ссылка на Image с типом Fill
    public float maxValue = 100f; // Максимальное значение прогресса
    private float currentValue = 0f; // Текущее значение прогресса

    void Start()
    {
        // Инициализация, если нужно
        SetProgress(0f);
    }

    public void SetProgress(float value)
    {
        currentValue = Mathf.Clamp(value, 0f, maxValue);
        fillImage.fillAmount = currentValue / maxValue;
    }
}
