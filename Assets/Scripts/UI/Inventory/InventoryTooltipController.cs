using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTooltipController : MonoBehaviour
{
    [SerializeField] Animator tooltipAnimator;
    [SerializeField] TMP_Text tooltipHeader, tooltipText, tooltipCost, tooltipActions;
    [Space]
    [SerializeField] ObjectPivotAdjuster tooltipParent;
    PlayerControls controls;
    InventoryItem currentInventoryItem;


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

    public void SetCurrentInventoryItem(InventoryItem ii) => currentInventoryItem = ii;

    void UpdateTooltipParent()
    {
        tooltipParent.anchorRectTransform = currentInventoryItem.GetComponent<RectTransform>();
        tooltipParent.anchorContainerRectTransform = currentInventoryItem.transform.parent.GetComponent<RectTransform>();
        tooltipParent.RecalculateOffsets();
    }

    public void ShowTooltip()
    {
        UpdateTooltipParent();
        tooltipAnimator.SetBool("visible", true);
    }

    public void HideTooltip()
    {
        tooltipAnimator.SetBool("visible", false);
    }

    public void UpdateTooltip(LootCategory lc)
    {
        bool isInTraderUI = currentInventoryItem.IsInTraderUI();
        bool isInChestUI = currentInventoryItem.IsInChestUI();
        bool isChestItem = currentInventoryItem.IsChestItem();

        tooltipHeader.text = $"{lc.lootName}";
        tooltipText.text = lc.lootDesc;
        tooltipCost.text = $"Цена за одну шт.: {NumberFormatter.FormatNumberWithGrouping(lc.cost)} UMU";

        tooltipActions.text = "";
        if (isInTraderUI)
        {
            string sellBind = controls.TraderUI.SellItem.GetBindingDisplayString();
            tooltipActions.text += $"\n[{sellBind}] - продать 1 шт. (+Shift - продать всё)";
        }
        else if (isInChestUI)
        {
            string transferBind = controls.ChestUI.TransferItem.GetBindingDisplayString();
            if (isChestItem)
            {
                tooltipActions.text += $"\n[{transferBind}] - взять в инвентарь (+Shift - взять всё)";
            }
            else
            {
                tooltipActions.text += $"\n[{transferBind}] - положить в сундук (+Shift - положить всё)";
            }
        }
        else
        {
            string dropBind = controls.InventoryUI.DropItem.GetBindingDisplayString();
            tooltipActions.text += $"\n[{dropBind}] - выкинуть 1 шт. (+Shift - выкинуть всё)";
        }
    }
}
