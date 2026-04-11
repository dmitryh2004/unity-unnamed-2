using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerCameraBlurController : MonoBehaviour
{
    [SerializeField] Volume volume;
    DepthOfField dof;
    MotionBlur mb;
    bool blurred = false;
    [SerializeField] float blurAnimationTime = 1f;
    [SerializeField] float minBlurRadius = 0.5f, maxBlurRadius = 1.5f;

    private void Awake()
    {
        volume.profile.TryGet<DepthOfField>(out dof);
        volume.profile.TryGet<MotionBlur>(out mb);

        dof.active = false;
        dof.gaussianMaxRadius = new ClampedFloatParameter(minBlurRadius, minBlurRadius, maxBlurRadius);
        mb.intensity = new ClampedFloatParameter(0f, 0f, 1f);
    }
    public void EnableBlur()
    {
        if (!blurred)
        {
            dof.active = true;
            StartCoroutine(ChangeBlurRadius(true, () => { blurred = true; }));
        }
    }

    public void DisableBlur()
    {
        if (blurred)
            StartCoroutine(ChangeBlurRadius(false, () => { dof.active = false; blurred = false; }));
    }

    private IEnumerator ChangeBlurRadius(bool blur, Action callback = null)
    {
        float timer = 0f;
        while (timer < blurAnimationTime)
        {
            timer += Time.deltaTime;
            if (blur)
            {
                dof.gaussianMaxRadius = new ClampedFloatParameter(minBlurRadius + (maxBlurRadius - minBlurRadius) * timer, minBlurRadius, maxBlurRadius);
                mb.intensity = new ClampedFloatParameter(timer / blurAnimationTime, 0f, 1f);
            }
            else
            {
                dof.gaussianMaxRadius = new ClampedFloatParameter(maxBlurRadius - (maxBlurRadius - minBlurRadius) * timer, minBlurRadius, maxBlurRadius);
                mb.intensity = new ClampedFloatParameter(1f - timer / blurAnimationTime, 0f, 1f);
            }
            yield return new WaitForEndOfFrame();
        }
        callback.Invoke();
    }
}