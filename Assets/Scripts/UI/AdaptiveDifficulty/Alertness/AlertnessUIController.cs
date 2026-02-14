using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertnessUIController : MonoBehaviour
{
    [SerializeField] List<Image> images = new();

    [Header("Settings")]
    [SerializeField] List<Color> alertnessColors = new List<Color> { Color.green, Color.yellow, new Color(1f, 0.5f, 0f), Color.red, Color.red * 0.5f };

    public void UpdateUI(int alertness)
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].gameObject.SetActive(i < alertness);
            if (alertness > 0)
                images[i].color = alertnessColors[alertness - 1];
        }
    }
}
