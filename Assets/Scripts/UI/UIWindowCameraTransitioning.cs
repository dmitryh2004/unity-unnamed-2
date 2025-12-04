using System;
using System.Collections;
using UnityEngine;

public class UIWindowCameraTransitioning : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject gameplayCross;

    [Header("Camera translation")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera uiCamera;
    [SerializeField] Transform cameraPosition;

    Vector3 cameraLastPosition, cameraLastRotation;

    protected bool visible = false;

    Vector3 Clamp(Vector3 vector, float degs)
    {
        while (vector.x < -degs)
        {
            vector.x += 2 * degs;
        }
        while (vector.x > degs)
        {
            vector.x -= 2 * degs;
        }

        while (vector.y < -degs)
        {
            vector.y += 2 * degs;
        }
        while (vector.y > degs)
        {
            vector.y -= 2 * degs;
        }

        while (vector.z < -degs)
        {
            vector.z += 2 * degs;
        }
        while (vector.z > degs)
        {
            vector.z -= 2 * degs;
        }

        return vector;
    }

    /// <summary>
    /// Плавно перемещает камеру панели от или к панели
    /// </summary>
    /// <param name="direction">True - к панели, False - к игроку</param>
    /// <param name="duration">Время перемещения</param>
    /// <param name="onComplete">Коллбэк</param>
    /// <returns></returns>
    IEnumerator TranslateUiCamera(bool direction, float duration = 1f, Action onComplete = null)
    {
        Vector3 moveDirection = cameraPosition.position - cameraLastPosition;
        Vector3 rotateAngles = cameraPosition.eulerAngles - cameraLastRotation;

        Debug.Log(rotateAngles);

        rotateAngles = Clamp(rotateAngles, 180f);

        if (!direction)
        {
            moveDirection *= -1;
            rotateAngles *= -1;
            uiCamera.transform.position = cameraPosition.position;
            uiCamera.transform.rotation = cameraPosition.rotation;
        }
        else
        {
            uiCamera.transform.position = cameraLastPosition;
            uiCamera.transform.rotation = Quaternion.Euler(cameraLastRotation);
        }

        Vector3 startPos = uiCamera.transform.position, startRot = uiCamera.transform.eulerAngles;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            uiCamera.transform.position = startPos + timer / duration * moveDirection;
            uiCamera.transform.rotation = Quaternion.Euler(startRot + timer / duration * rotateAngles);

            yield return new WaitForEndOfFrame();
        }

        uiCamera.transform.position = startPos + moveDirection;
        uiCamera.transform.rotation = Quaternion.Euler(startRot + rotateAngles);

        onComplete?.Invoke();
    }

    public void ShowWindow()
    {
        InputActionMapSwitcher.Instance.DisableAllMaps();

        gameplayCross.SetActive(false);

        cameraLastPosition = playerCamera.transform.position;
        cameraLastRotation = playerCamera.transform.eulerAngles;

        playerCamera.gameObject.SetActive(false);
        uiCamera.gameObject.SetActive(true);

        visible = true;
        animator.SetBool("visible", visible);
        StartCoroutine(TranslateUiCamera(true, 1f, () => {
            UpdateCurrentInputMap();
            ChangeToMainMenu();
        }));
    }

    public void HideWindow()
    {
        InputActionMapSwitcher.Instance.DisableAllMaps();

        StartCoroutine(TranslateUiCamera(false, 1f, () =>
        {
            InputActionMapSwitcher.Instance.SwitchMap("Gameplay");
            playerCamera.gameObject.SetActive(true);
            uiCamera.gameObject.SetActive(false);

            gameplayCross.SetActive(true);

            visible = false;
            animator.SetBool("visible", visible);

            OnClosed();
        }));
    }

    protected virtual void UpdateCurrentInputMap()
    {

    }

    protected virtual void OnClosed()
    {
        ChangeToMainMenu();
    }

    protected virtual void ChangeToMainMenu()
    {

    }
}
