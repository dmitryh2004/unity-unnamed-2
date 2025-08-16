using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    int id;
    PlayerControls controls;
    bool pointerOnItem = false;
    bool active = false;
    LootCategory lc;
    [SerializeField] bool isInChestUI = false;
    [SerializeField] bool isChestItem = false;
    [SerializeField] bool isInTraderUI = false;
    [SerializeField] Sprite emptySprite;
    [SerializeField] Image image;
    [SerializeField] TMP_Text itemCount;
    [SerializeField] InventoryUIController uiController;
    [SerializeField] ChestUIController chestUIController;
    [SerializeField] TraderUIQuotaScreenController traderUIQuotaController;
    [SerializeField] TraderUISellItemsController traderUISellItemsController;
    [Space]
    [SerializeField] InventoryTooltipController tooltipController;

    void Awake()
    {
        controls = new();
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    void Start()
    {
        HideTooltip();
    }

    public bool IsActive() => active;
    public bool IsInTraderUI() => isInTraderUI;
    public bool IsInChestUI() => isInChestUI;
    public bool IsChestItem() => isChestItem;

    public void SetActive(bool active)
    {
        this.active = active;

        image.gameObject.SetActive(active);
        itemCount.gameObject.SetActive(active);

        if (!active) HideTooltip();
    }

    public void Initialize(int id, Sprite newSprite, int count, LootCategory lc)
    {
        this.id = id;
        this.lc = lc;
        UpdateImage(newSprite);
        UpdateCount(count);
    }

    public void UpdateImage(Sprite newSprite)
    {
        image.sprite = newSprite;
    }

    public void UpdateCount(int count)
    {
        itemCount.text = $"{count}";
    }

    public void ShowTooltip()
    {
        tooltipController.SetCurrentInventoryItem(this);
        tooltipController.UpdateTooltip(lc);
        tooltipController.ShowTooltip();
    }

    public void HideTooltip()
    {
        tooltipController.HideTooltip();
    }

    public void UpdateTooltip()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"cursor on the inventory item {gameObject.name} (active={active})");
        pointerOnItem = true;
        if (active)
        {
            ShowTooltip();
            if (isInTraderUI)
            {
                if (traderUIQuotaController != null)
                {
                    traderUIQuotaController.SetActiveItem(id);
                }
                else if (traderUISellItemsController != null)
                {
                    traderUISellItemsController.SetActiveItem(id);
                }
            }
            else if (isInChestUI)
            {
                if (isChestItem)
                {
                    chestUIController.SetActiveChestItem(id);
                }
                else
                {
                    chestUIController.SetActiveItem(id);
                }
            }
            else
            {
                uiController.SetActiveItem(id);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOnItem = false;
        HideTooltip();
        if (isInTraderUI)
        {
            if (traderUIQuotaController != null)
            {
                traderUIQuotaController.SetActiveItem(-1);
            }
            else if (traderUISellItemsController != null)
            {
                traderUISellItemsController.SetActiveItem(-1);
            }
        }
        else if (isInChestUI)
        {
            if (isChestItem)
            {
                chestUIController.SetActiveChestItem(-1);
            }
            else
            {
                chestUIController.SetActiveItem(-1);
            }
        }
        else
        {
            uiController.SetActiveItem(-1);
        }
    }

    public bool IsPointerOnItem()
    {
        return pointerOnItem;
    }

    public int GetID()
    {
        return id;
    }
}
