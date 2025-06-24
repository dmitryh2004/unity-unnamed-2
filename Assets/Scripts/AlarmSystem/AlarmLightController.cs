using UnityEngine;

public class AlarmLightController : MonoBehaviour
{
    [SerializeField] Light pointLightSource;
	[SerializeField] Light spotLightSource;
    [SerializeField] Animator animator;
    [SerializeField] MeshRenderer meshRenderer;
    Material material;
    bool lightEnabled = false;

    private void Start()
    {
        material = meshRenderer.material;
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
            material.EnableKeyword("_EMISSION");
        }
        else
        {
            material.DisableKeyword("_EMISSION");
        }
    }
}
