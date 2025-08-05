using UnityEngine;

public class AlarmLightController : MonoBehaviour
{
    [SerializeField] Light pointLightSource;
	[SerializeField] Light spotLightSource;
    [SerializeField] Animator animator;
    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField] Material noEmissionMaterial, emissionMaterial;
    bool lightEnabled = false;

    private void Start()
    {
        UpdateAnimator();
    }
    public void ChangeState(bool state)
    {
        lightEnabled = state;
        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        animator.SetBool("enabled", lightEnabled);
        pointLightSource.gameObject.SetActive(lightEnabled);
		spotLightSource.gameObject.SetActive(lightEnabled);
        if (lightEnabled)
        {
            meshRenderer.material = emissionMaterial;
        }
        else
        {
            meshRenderer.material = noEmissionMaterial;
        }
    }
}
