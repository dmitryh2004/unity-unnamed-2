using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TraderUIWindowController : MonoBehaviour
{
    [Header("Input")]
    private PlayerControls controls;
    [SerializeField] PlayerInput playerInput;
    [Header("Links")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject gameplayCross;
    [Space(10)]
    [SerializeField] TraderUIBaseScreenController mainMenuScreen; // 0
    [SerializeField] TraderUIBaseScreenController quotaScreen; // 1
    [SerializeField] TraderUIBaseScreenController noQuotaScreen; // 2
    [SerializeField] TraderUIBaseScreenController sellItemsScreen; // 3
    [SerializeField] TraderUIBaseScreenController upgradeEquipmentScreen; // 4
    [Space(10)]
    [SerializeField] GameObject backButton;
    [Header("Backgrounds")]
    [SerializeField] Image background;
    [SerializeField] Sprite mainMenuBackground;
    [SerializeField] Sprite quotaBackground;
    [SerializeField] Sprite noQuotaBackground;
    [SerializeField] Sprite sellItemsBackground;
    [SerializeField] Sprite upgradeEquipmentBackground;
    [Header("Camera translation")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera uiCamera;
    [SerializeField] Transform cameraPosition;

    Vector3 cameraLastPosition, cameraLastRotation;

    int currentScreen = 0;
    bool visible = false;

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

    void UpdateScreen()
    {
        if (currentScreen < 0 || currentScreen > 4)
        {
            Debug.LogWarning($"Trader UI window controller: current screen was {currentScreen} (not in range 0-4)");
            currentScreen = 0;
        }
        if (mainMenuScreen != null)
            mainMenuScreen.gameObject.SetActive(currentScreen == 0);
        if (quotaScreen != null)
            quotaScreen.gameObject.SetActive(currentScreen == 1);
        if (noQuotaScreen != null)
            noQuotaScreen.gameObject.SetActive(currentScreen == 2);
        if (sellItemsScreen != null)
            sellItemsScreen.gameObject.SetActive(currentScreen == 3);
        if (upgradeEquipmentScreen != null)
            upgradeEquipmentScreen.gameObject.SetActive(currentScreen == 4);

        backButton.SetActive(currentScreen != 0);

        switch(currentScreen)
        {
            case 0:
                background.sprite = mainMenuBackground;
                mainMenuScreen.OnShow();
                break;
            case 1:
                background.sprite = quotaBackground;
                quotaScreen.OnShow();
                break;
            case 2:
                background.sprite = noQuotaBackground;
                noQuotaScreen.OnShow();
                break;
            case 3:
                background.sprite = sellItemsBackground;
                sellItemsScreen.OnShow();
                break;
            case 4:
                background.sprite = upgradeEquipmentBackground;
                upgradeEquipmentScreen.OnShow();
                break;
        }
    }

    public void SetScreen(int screen)
    {
        currentScreen = screen;
        UpdateScreen();
    }

    public int GetCurrentScreen() => currentScreen;

    public void ChangeToMainMenu()
    {
        SetScreen(0);
    }

    private void Start()
    {
        ChangeToMainMenu();
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

            ChangeToMainMenu();
        }));
    }

    public void CloseWindow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "TraderUI")
            HideWindow();
    }

    void UpdateCurrentInputMap()
    {
        if (visible)
        {
            InputActionMapSwitcher.Instance.SwitchMap("TraderUI");
        }
        else
        {
            InputActionMapSwitcher.Instance.SwitchMap("Gameplay");
        }
    }

    public void SellItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "TraderUI")
        {
            var keyboard = Keyboard.current;
            bool shiftPressed = keyboard != null && keyboard.leftShiftKey.isPressed;
            if (currentScreen == 1)
            {
                ((TraderUIQuotaScreenController)quotaScreen).SellActiveItem(shiftPressed);
            }
            else if (currentScreen == 3)
            {
                ((TraderUISellItemsController)sellItemsScreen).SellActiveItem(shiftPressed);
            }
        }
    }
}
