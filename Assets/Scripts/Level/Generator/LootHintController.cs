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
    [SerializeField] List<ColorByValue> colors = new();
    [SerializeField] Transform player;
    [SerializeField] float maxRenderDistance = 5f;

    [SerializeField] Renderer[] objectRenderers = new Renderer[0];
    private Material[] instanceMaterials;  // Instance материала
    private Color currentColor;
    private bool emitting = false;

    private void Start()
    {
        instanceMaterials = new Material[objectRenderers.Length];
        for (int i = 0; i < objectRenderers.Length; i++)
        {
            instanceMaterials[i] = objectRenderers[i].material;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;

        ColorByValueComparer comparer = new();
        colors.Sort(comparer);
    }

    public void SetActive(bool active)
    {
        emitting = active;
    }

    public void UpdateLootSum()
    {
        int lootSum = lootContainer.GetSpawnedLootSum();

        Color resultColor = colors[colors.Count - 1].color; // по умолчанию считаем, что стоимость лута максимальна

        for (int i = 0; i < colors.Count; i++)
        {
            if (lootSum > colors[i].value) continue;
            else if (lootSum == colors[i].value)
            {
                Color color = colors[i].color;
                resultColor = color;
                break;
            }
            else
            {
                resultColor = colors[(i > 0) ? i - 1 : 0].color;
                break;
            }
        }
        currentColor = resultColor;

        UpdateColors();
    }

    void UpdateColors()
    {
        Color color = Color.black;
        if (emitting)
        {
            color = currentColor;
            float intensity = Mathf.Clamp(1 - Vector3.Distance(transform.position, player.position) / maxRenderDistance, 0, 1f);
            color = color * intensity;
        }
        //emission
        for (int i = 0; i < instanceMaterials.Length; i++)
            instanceMaterials[i].SetColor("_EmissionColor", color);
    }

    private void Update()
    {
        UpdateColors();
    }
}