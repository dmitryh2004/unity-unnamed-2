using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClearSlotButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public float holdTime = 3f; // Время удержания в секундах
    private float timer = 0f;
    private bool isHolding = false;

    [SerializeField] private Image progressBar;
    [SerializeField] int slotNumber;
    [SerializeField] SaveManager saveManager;
    [SerializeField] MainMenuSlotUIController slotUIController;

    void Start()
    {

    }

    private void Update()
    {
        float progressValue = Mathf.Clamp01(timer / holdTime);

        progressBar.fillAmount = progressValue;

        if (isHolding)
        {
            timer += Time.deltaTime;

            if (timer >= holdTime)
            {
                isHolding = false;
                timer = 0f;
                OnHoldComplete();
            }
        }
    }

    // Событие при начале нажатия ЛКМ на кнопку
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isHolding = true;
            timer = 0f;
        }
    }

    // Событие при отпускании ЛКМ
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isHolding = false;
            timer = 0f;
        }
    }

    // Событие при уходе курсора с кнопки (отмена удержания)
    public void OnPointerExit(PointerEventData eventData)
    {
        isHolding = false;
        timer = 0f;
    }

    // Метод, вызываемый после успешного удержания 3 секунды
    private void OnHoldComplete()
    {
        Debug.Log("Кнопка нажата удержанием 3 секунды!");
        saveManager.ClearSave(slotNumber);
        slotUIController.RefreshData();
    }
}
