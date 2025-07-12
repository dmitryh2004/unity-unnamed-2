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
    [SerializeField] bool isInChestUI = false;
    [SerializeField] bool isChestItem = false;
    [SerializeField] Sprite emptySprite;
    [SerializeField] Image image;
    [SerializeField] TMP_Text itemCount;
    [SerializeField] InventoryUIController uiController;
    [SerializeField] ChestUIController chestUIController;
    [SerializeField] Animator tooltipAnimator;
    [SerializeField] TMP_Text tooltipHeader, tooltipText, tooltipCost, tooltipActions;

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

    public void SetActive(bool active)
    {
        this.active = active;

        image.gameObject.SetActive(active);
        itemCount.gameObject.SetActive(active);

        if (!active) HideTooltip();
    }

    public void Initialize(int id, Sprite newSprite, int count)
    {
        this.id = id;
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
        tooltipAnimator.SetBool("visible", true);
    }

    public void HideTooltip()
    {
        tooltipAnimator.SetBool("visible", false);
    }

    public void UpdateTooltip(LootCategory lc)
    {
        tooltipHeader.text = $"{lc.lootName}";
        tooltipText.text = lc.lootDesc;
        tooltipCost.text = $"Цена за одну шт.: {NumberFormatter.FormatNumberWithGrouping(lc.cost)} руб.";
        
        tooltipActions.text = "";
        if (!isInChestUI)
        {
            string dropBind = controls.InventoryUI.DropItem.GetBindingDisplayString();
            tooltipActions.text += $"\n[{dropBind}] - выкинуть";
        }
        else
        {
            string transferBind = controls.ChestUI.TransferItem.GetBindingDisplayString();
            if (isChestItem)
            {
                tooltipActions.text += $"\n[{transferBind}] - взять в инвентарь";
            }
            else
            {
                tooltipActions.text += $"\n[{transferBind}] - положить в сундук";
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"cursor on the inventory item {gameObject.name} (active={active})");
        pointerOnItem = true;
        if (active)
        {
            ShowTooltip();
            if (isInChestUI)
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
        if (isInChestUI)
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
