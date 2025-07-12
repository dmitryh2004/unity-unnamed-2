using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceshipPanelController : Interactable
{
    [Header("Camera")]
    [SerializeField] Transform cameraPosition;
    [Header("Links")]
    [SerializeField] Animator animator;
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera panelCamera;
    
    [Header("First screen")]
    [SerializeField] TMP_Text currentComplex;
    [SerializeField] TMP_Text currentComplexDifficulty;
    [SerializeField] TMP_Text currentComplexRoomsAmount;
    [SerializeField] TMP_Text currentComplexGuardiansAmount;
    [SerializeField] TMP_Text currentComplexReinforcementTimer;
    [SerializeField] TMP_Text currentComplexDescription;
    [Header("Second screen")]
    [SerializeField] List<Complex> complexList = new();
    [SerializeField] List<TMP_Text> complexTextsList = new();

    Vector3 cameraLastPosition, cameraLastRotation;
    int currentScreen = 1;

    public override void Interact()
    {
        cameraLastPosition = playerCamera.transform.position;
        cameraLastRotation = playerCamera.transform.eulerAngles;

        playerCamera.gameObject.SetActive(false);
        panelCamera.gameObject.SetActive(true);

        StartCoroutine(TranslatePanelCamera(true, 1f, () =>
        {
            // change input map
        }));
    }

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
    IEnumerator TranslatePanelCamera(bool direction, float duration = 1f, Action onComplete = null)
    {
        Vector3 moveDirection = cameraPosition.position - cameraLastPosition;
        Vector3 rotateAngles = cameraPosition.eulerAngles - cameraLastRotation;

        Debug.Log(rotateAngles);

        rotateAngles = Clamp(rotateAngles, 180f);
        
        if (!direction)
        {
            moveDirection *= -1;
            rotateAngles *= -1;
            panelCamera.transform.position = cameraPosition.position;
            panelCamera.transform.rotation = cameraPosition.rotation;
        }
        else
        {
            panelCamera.transform.position = cameraLastPosition;
            panelCamera.transform.rotation = Quaternion.Euler(cameraLastRotation);
        }

        Vector3 startPos = panelCamera.transform.position, startRot = panelCamera.transform.eulerAngles;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            panelCamera.transform.position = startPos + timer / duration * moveDirection;
            panelCamera.transform.rotation = Quaternion.Euler(startRot + timer / duration * rotateAngles);

            yield return new WaitForEndOfFrame();
        }

        panelCamera.transform.position = startPos + moveDirection;
        panelCamera.transform.rotation = Quaternion.Euler(startRot + rotateAngles);

        onComplete?.Invoke();
    }
}
