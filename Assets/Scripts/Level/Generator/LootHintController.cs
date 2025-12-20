using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorByValue
{
    public int value;
    public Color color;
}

public class ColorByValueComparer : IComparer<ColorByValue>
{
    public int Compare(ColorByValue a, ColorByValue b)
    {
        return a.value.CompareTo(b.value);
    }
}

public class LootHintController : MonoBehaviour
{
    [SerializeField] LootContainer lootContainer;
    [SerializeField] Material material;
    [SerializeField] List<ColorByValue> colors = new();
    [SerializeField] Transform player;
    [SerializeField] float maxRenderDistance = 5f;

    private Renderer objectRenderer;
    private Material instanceMaterial;  // Instance материала

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("Renderer component not found!");
            return;
        }

        // Создаем instance материала и присваиваем его Renderer'у
        if (material == null) material = objectRenderer.sharedMaterial;
        instanceMaterial = objectRenderer.material;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        ColorByValueComparer comparer = new();
        colors.Sort(comparer);
    }

    public void UpdateLootSum()
    {
        int lootSum = lootContainer.GetSpawnedLootSum();

        instanceMaterial.color = colors[colors.Count - 1].color; // по умолчанию считаем, что стоимость лута максимальна

        for (int i = 0; i < colors.Count; i++)
        {
            if (lootSum > colors[i].value) continue;
            else if (lootSum == colors[i].value)
            {
                Color color = colors[i].color;
                instanceMaterial.color = color;
                break;
            }
            else
            {
                Color colorLess = colors[i].color;
                if (i > 0)
                {
                    Color colorGreater = colors[i - 1].color;
                    float ratio = ((float)lootSum - colors[i - 1].value) / (colors[i].value - colors[i - 1].value);
                    Color resultColor = Color.Lerp(colorGreater, colorLess, ratio);
                    instanceMaterial.color = resultColor;
                }
                else
                {
                    instanceMaterial.color = colors[0].color;
                }
                break;
            }
        }

        UpdateColor();
    }

    void UpdateColor()
    {
        Color color = instanceMaterial.color;
        color.a = Mathf.Clamp01(1 - Vector3.Distance(transform.position, player.position) / maxRenderDistance);
        instanceMaterial.color = color;

        //emission
        instanceMaterial.SetColor("_EmissionColor", color);
    }

    private void Update()
    {
        UpdateColor();
    }
}