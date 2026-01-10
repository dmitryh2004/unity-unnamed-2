using UnityEngine;

public class PressurePlateEmissionController : MonoBehaviour
{
    bool emitting = false;
    [SerializeField] Gradient activeEmissionGradient, notActiveEmissionGradient;
    [SerializeField] float emissionCycleTime = 1f;
    float emissionCycleTimer = 0f;

    [SerializeField] Renderer[] objectRenderers = new Renderer[0];
    [SerializeField] ParticleSystem[] particleSystems = new ParticleSystem[0];
    private Material[] instanceMaterials;

    private void Start()
    {
        instanceMaterials = new Material[objectRenderers.Length];
        for (int i = 0; i < objectRenderers.Length; i++)
        {
            instanceMaterials[i] = objectRenderers[i].material;
        }
    }

    private void Update()
    {
        emissionCycleTimer += Time.deltaTime;
        if (emissionCycleTimer > emissionCycleTime) emissionCycleTimer -= emissionCycleTime;

        UpdateColors();
    }

    void UpdateColors()
    {
        Color color = Color.black;
        float ratio = emissionCycleTimer / emissionCycleTime;
        color = emitting ? activeEmissionGradient.Evaluate(ratio) : notActiveEmissionGradient.Evaluate(ratio);

        //emission
        for (int i = 0; i < instanceMaterials.Length; i++)
            instanceMaterials[i].SetColor("_EmissionColor", color);

        //particles
        for (int i = 0; i < particleSystems.Length; i++)
        {
            var ps = particleSystems[i].main;
            ps.startColor = color;
        }
    }

    public void SetEmitting(bool emitting) => this.emitting = emitting;
    public bool IsEmitting() => emitting;
}
