using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TraderUIWindowController : UIWindowCameraTransitioning
{
    [Header("Input")]
    [SerializeField] PlayerInput playerInput;
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

    int currentScreen = 0;

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

    protected override void ChangeToMainMenu()
    {
        SetScreen(0);
    }

    private void Start()
    {
        ChangeToMainMenu();
    }

    public void CloseWindow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "TraderUI")
            HideWindow();
    }

    protected override void UpdateCurrentInputMap()
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

    public void Number1Press(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "TraderUI")
        {
            if (currentScreen == 0)
            {
                ((TraderUIMainScreenController)mainMenuScreen).ChangeToQuotaScreen();
            }
            else if (currentScreen == 2)
            {
                ((TraderUINoQuotaScreenController)noQuotaScreen).SelectTask(1);
            }
            else if (currentScreen == 4)
            {
                ((TraderUIEquipmentScreenController)upgradeEquipmentScreen).SetSelectedItem(0);
            }
        }
    }

    public void Number2Press(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "TraderUI")
        {
            if (currentScreen == 0)
            {
                ((TraderUIMainScreenController)mainMenuScreen).ChangeToSellScreen();
            }
            else if (currentScreen == 2)
            {
                ((TraderUINoQuotaScreenController)noQuotaScreen).SelectTask(2);
            }
            else if (currentScreen == 4)
            {
                ((TraderUIEquipmentScreenController)upgradeEquipmentScreen).SetSelectedItem(1);
            }
        }
    }

    public void Number3Press(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "TraderUI")
        {
            if (currentScreen == 0)
            {
                ((TraderUIMainScreenController)mainMenuScreen).ChangeToEquipmentScreen();
            }
            else if (currentScreen == 2)
            {
                ((TraderUINoQuotaScreenController)noQuotaScreen).SelectTask(3);
            }
            else if (currentScreen == 4)
            {
                ((TraderUIEquipmentScreenController)upgradeEquipmentScreen).SetSelectedItem(2);
            }
        }
    }

    public void BackPressed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "TraderUI")
        {
            ChangeToMainMenu();
        }
    }

    public void EnterPressed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentActionMap.name == "TraderUI")
        {
            if (currentScreen == 2)
            {
                ((TraderUINoQuotaScreenController)noQuotaScreen).AcceptTask();
            }
            else if (currentScreen == 4)
            {
                ((TraderUIEquipmentScreenController)upgradeEquipmentScreen).UpgradeEquipment();
            }
        }
    }
}
