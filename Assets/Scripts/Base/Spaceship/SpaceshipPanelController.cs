using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipPanelController : Interactable
{
    [Header("Camera")]
    [SerializeField] Transform cameraPosition;
    [Header("Links")]
    [SerializeField] Animator animator;
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera panelCamera;
    [SerializeField] PlayerInput playerInput;
    [Header("First screen")]
    [SerializeField] TMP_Text currentComplex;
    [SerializeField] TMP_Text currentComplexDifficulty;
    [SerializeField] TMP_Text currentComplexRoomsAmount;
    [SerializeField] TMP_Text currentComplexGuardiansAmount;
    [SerializeField] TMP_Text currentComplexReinforcementTimer;
    [SerializeField] TMP_Text currentComplexDescription;

    [SerializeField] string currentComplexTemplate, currentComplexDifficultyTemplate, currentComplexRoomsAmountTemplate,
        currentComplexGuardiansAmountTemplate, currentComplexReinforcementTimerTemplate, currentComplexDescriptionTemplate;
    [Header("Second screen")]
    [SerializeField] List<Complex> complexList = new();
    [SerializeField] List<TMP_Text> complexTextsList = new();

    Vector3 cameraLastPosition, cameraLastRotation;
    int currentScreen = 0;

    int currentComplexIndex = 0;
    int screen2SelectedComplexIndex = 0;

    private void Start()
    {
        UpdateScreen1();
    }

    public int GetCurrentComplexIndex()
    {
        return currentComplexIndex;
    }

    public void SetCurrentComplexIndex(int cci)
    {
        if (cci < 0 || cci >= complexList.Count)
        {
            currentComplexIndex = 0;
            Debug.LogWarning($"Spaceship panel: can not set current complex to {cci}, setted to 0");
            return;
        }
        currentComplexIndex = cci;
        UpdateScreen1();
        UpdateScreen2();
    }

    public override void Interact()
    {
        InputActionMapSwitcher.Instance.DisableAllMaps();

        cameraLastPosition = playerCamera.transform.position;
        cameraLastRotation = playerCamera.transform.eulerAngles;

        playerCamera.gameObject.SetActive(false);
        panelCamera.gameObject.SetActive(true);

        StartCoroutine(TranslatePanelCamera(true, 1f, () =>
        {
            InputActionMapSwitcher.Instance.SwitchMap("SpaceshipPanelUI");
            ChangeScreen(1);
        }));
    }

    void ChangeScreen(int newScreen)
    {
        if (currentScreen != 0 && newScreen != 0) animator.SetTrigger("swap");
        currentScreen = newScreen;

        if (currentScreen == 1)
        {
            UpdateScreen1();
        }
        else if (currentScreen == 2)
        {
            UpdateScreen2();
        }
        else
        {

        }
    }

    private void UpdateScreen1()
    {
        //text
        currentComplex.text = currentComplexTemplate.Replace("A", complexList[currentComplexIndex].complexName);
        currentComplexDifficulty.text = currentComplexDifficultyTemplate.Replace("A", $"{complexList[currentComplexIndex].difficulty}");
        currentComplexRoomsAmount.text = currentComplexRoomsAmountTemplate.Replace("A", $"{complexList[currentComplexIndex].minRooms}").Replace("B", $"{complexList[currentComplexIndex].maxRooms}");
        
        if (complexList[currentComplexIndex].guardiansMinCount == complexList[currentComplexIndex].guardiansMaxCount)
        {
            currentComplexGuardiansAmount.text = currentComplexGuardiansAmountTemplate.Replace("A", $"{complexList[currentComplexIndex].guardiansMinCount}").Replace("-B", "");
        }
        else
        {
            currentComplexGuardiansAmount.text = currentComplexGuardiansAmountTemplate.Replace("A", $"{complexList[currentComplexIndex].guardiansMinCount}")
                .Replace("-B", $"{complexList[currentComplexIndex].guardiansMaxCount}");
        }
        
        currentComplexReinforcementTimer.text = currentComplexReinforcementTimerTemplate.Replace("A", complexList[currentComplexIndex].reinforcementTimer);
        currentComplexDescription.text = currentComplexDescriptionTemplate.Replace("A", complexList[currentComplexIndex].description);

        //update spaceship
        SpaceshipController.Instance.SetCurrentComplex(complexList[currentComplexIndex]);
    }

    private void UpdateScreen2()
    {
        for (int i = 0; i < complexTextsList.Count; i++)
        {
            complexTextsList[i].text = $"{complexList[i].complexName} ({complexList[i].difficulty} / 10)";
            if (i == screen2SelectedComplexIndex)
            {
                complexTextsList[i].text = $"> {complexTextsList[i].text} <";
            }
        }
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

    public void Exit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "SpaceshipPanelUI" && currentScreen == 1)
        {
            ChangeScreen(0);
            InputActionMapSwitcher.Instance.DisableAllMaps();

            StartCoroutine(TranslatePanelCamera(false, 1f, () =>
            {
                InputActionMapSwitcher.Instance.SwitchMap("Gameplay");
                playerCamera.gameObject.SetActive(true);
                panelCamera.gameObject.SetActive(false);
            }));
        }
    }

    public void ComplexList(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "SpaceshipPanelUI" && currentScreen == 1)
        {
            screen2SelectedComplexIndex = currentComplexIndex;
            ChangeScreen(2);
        }
    }

    public void Back(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "SpaceshipPanelUI" && currentScreen == 2)
        {
            ChangeScreen(1);
        }
    }

    public void SelectComplex(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "SpaceshipPanelUI" && currentScreen == 2)
        {
            currentComplexIndex = screen2SelectedComplexIndex;
            ChangeScreen(1);
        }
    }

    public void NavigateUp(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "SpaceshipPanelUI" && currentScreen == 2)
        {
            screen2SelectedComplexIndex--;
            if (screen2SelectedComplexIndex < 0)
            {
                screen2SelectedComplexIndex = complexList.Count - 1;
            }
            UpdateScreen2();
        }
    }
    public void NavigateDown(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "SpaceshipPanelUI" && currentScreen == 2)
        {
            screen2SelectedComplexIndex++;
            if (screen2SelectedComplexIndex >= complexList.Count)
            {
                screen2SelectedComplexIndex = 0;
            }
            UpdateScreen2();
        }
    }
}
