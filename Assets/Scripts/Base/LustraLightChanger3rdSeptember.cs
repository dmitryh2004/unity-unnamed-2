using System.Collections.Generic;
using UnityEngine;

public class LustraLightChanger3rdSeptember : MonoBehaviour
{
    [SerializeField] Color changedColor;
    [SerializeField] Light lustraLight;
    [SerializeField] List<MeshRenderer> renderers = new();
    private void Start()
    {
        if (DayCheck.Instance != null && DayCheck.Instance.IsSeptember3)
        {
            foreach(var renderer in renderers)
            {
                Material material = renderer.material;
                material.SetColor("_EmissionColor", changedColor);
            }
            lustraLight.color = changedColor;
        }
    }
}
