using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image fillImage; // ������ �� Image � ����� Fill
    public float maxValue = 100f; // ������������ �������� ���������
    private float currentValue = 0f; // ������� �������� ���������

    void Start()
    {
        // �������������, ���� �����
        SetProgress(0f);
    }

    public void SetProgress(float value)
    {
        currentValue = Mathf.Clamp(value, 0f, maxValue);
        fillImage.fillAmount = currentValue / maxValue;
    }
}
