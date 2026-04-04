using UnityEngine;

public class SpaceshipLampController : MonoBehaviour
{
    [SerializeField] Light lampLight;
    [SerializeField] Color baseColor = Color.white;
    [SerializeField] Color alarmColor = Color.red;
    [SerializeField] bool alarmed = false;
    [SerializeField] float alarmBlinkTime = 2f;
    float currentIntensity = 1f;
    float timer = 0f;
    Material material;
    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
        UpdateLight();
    }

    private void Update()
    {
        if (alarmed)
        {
            timer += Time.deltaTime;
            if (timer >= alarmBlinkTime) timer -= alarmBlinkTime;
            currentIntensity = Mathf.Pow(Mathf.Cos(Mathf.PI * (timer / alarmBlinkTime)), 2f);

            UpdateLight();
        }
    }

    void UpdateLight(bool resetTimer = false)
    {
        if (resetTimer)
        {
            currentIntensity = 1f;
            timer = 0f;
        }
        Color color = (alarmed ? alarmColor : baseColor) * currentIntensity;
        lampLight.color = color;
        material.SetColor("_EmissionColor", color);
    }

    public void SetAlarmed(bool a)
    {
        alarmed = a;
        UpdateLight(resetTimer: true);
    }
}
