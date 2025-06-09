using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    int id;
    bool pointerOnItem = false;
    [SerializeField] Sprite emptySprite;
    [SerializeField] Image image;
    [SerializeField] TMP_Text itemCount;
    [SerializeField] InventoryUIController uiController;
    [SerializeField] Animator tooltipAnimator;
    [SerializeField] TMP_Text tooltipHeader, tooltipText, tooltipCost;

    void Start()
    {
        HideTooltip();
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
        tooltipCost.text = $"÷ена за одну шт.: {NumberFormatter.FormatNumberWithGrouping(lc.cost)} руб.";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOnItem = true;
        ShowTooltip();
        uiController.SetActiveItem(id);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOnItem = false;
        HideTooltip();
        uiController.SetActiveItem(-1);
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
