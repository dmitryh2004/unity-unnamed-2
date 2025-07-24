using UnityEngine;
using UnityEngine.InputSystem;

public class TraderUIWindowController : MonoBehaviour
{
    [Header("Input")]
    private PlayerControls controls;
    [SerializeField] PlayerInput playerInput;
    [Header("Links")]
    [SerializeField] Animator animator;
    [Space(10)]
    [SerializeField] GameObject mainMenuScreen; // 0
    [SerializeField] GameObject quotaScreen; // 1
    [SerializeField] GameObject noQuotaScreen; // 2
    [SerializeField] GameObject sellItemsScreen; // 3
    [SerializeField] GameObject upgradeEquipmentScreen; // 4

    int currentScreen = 0;
    bool visible = false;

    void UpdateScreen()
    {
        if (currentScreen < 0 || currentScreen > 4)
        {
            Debug.LogWarning($"Trader UI window controller: current screen was {currentScreen} (not in range 0-4)");
            currentScreen = 0;
        }
        if (mainMenuScreen != null)
            mainMenuScreen.SetActive(currentScreen == 0);
        if (quotaScreen != null)
            quotaScreen.SetActive(currentScreen == 1);
        if (noQuotaScreen != null)
            noQuotaScreen.SetActive(currentScreen == 2);
        if (sellItemsScreen != null)
            sellItemsScreen.SetActive(currentScreen == 3);
        if (upgradeEquipmentScreen != null)
            upgradeEquipmentScreen.SetActive(currentScreen == 4);
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
        visible = true;
        animator.SetBool("visible", visible);
        UpdateCurrentInputMap();
        ChangeToMainMenu();
    }

    public void HideWindow()
    {
        visible = false;
        animator.SetBool("visible", visible);
        UpdateCurrentInputMap();
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
}
