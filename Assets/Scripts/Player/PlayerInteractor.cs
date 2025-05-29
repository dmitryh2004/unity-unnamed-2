using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask interactionLayer;

    private PlayerControls controls;
    private InputAction interactAction;

    private void Awake()
    {
        controls = new PlayerControls();
        interactAction = controls.Gameplay.Interact;
    }
    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        if (interactAction.WasPerformedThisFrame())
        {
            //Debug.Log($"player: attempting to interact...");
            TryInteract();
        }
        else
        {
            UpdateInteractionHints();
        }
    }

    private void TryInteract()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit,
            10f,
            interactionLayer))
        {
            //Debug.Log($"player: found {hit.collider.gameObject.name}");
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                Debug.Log($"{hit.collider.gameObject.name} is interactable");
                if (hit.distance < interactable.GetInteractionRange())
                {
                    interactable.Interact();
                }
                else
                {
                    //Debug.Log($"{interactable.gameObject.name}: out of interaction range ({interactable.GetInteractionRange()})");
                }
            }
        }
    }

    private void UpdateInteractionHints()
    {
        bool openDoorHintActive = false, closeDoorHintActive = false, pickUpHintActive = false,
            inventoryFullActive = false;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit,
            10f,
            interactionLayer))
        {
            //Debug.Log($"player: found {hit.collider.gameObject.name}");
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                //Debug.Log($"{hit.collider.gameObject.name} is interactable");
                if (hit.distance < interactable.GetInteractionRange())
                {
                    if (interactable is DoorController doorController)
                    {
                        if (doorController.IsOpen())
                        {
                            closeDoorHintActive = true;
                        }
                        else
                        {
                            openDoorHintActive = true;
                        }
                    }
                    else if (interactable is LootableItem lootableItem)
                    {
                        if (InventorySystem.Instance.CanAddItem(lootableItem.GetLootCategory()))
                        {
                            pickUpHintActive = true;
                            TMP_Text hintText = HintManager.Instance.GetHintText(HintManager.Instance.GetHintByName("PickUpHint"));
                            if (hintText)
                            {
                                hintText.text = $"[E]: подобрать\n({lootableItem.GetLootCategory().lootName})";
                            }
                        }
                        else
                        {
                            inventoryFullActive = true;
                        }
                    }
                }
                else
                {
                    //Debug.Log($"{interactable.gameObject.name}: out of interaction range ({interactable.GetInteractionRange()})");
                }
            }
        }
        HintManager.Instance.ActivateHint(HintManager.Instance.GetHintByName("OpenDoorHint"), openDoorHintActive);
        HintManager.Instance.ActivateHint(HintManager.Instance.GetHintByName("CloseDoorHint"), closeDoorHintActive);
        HintManager.Instance.ActivateHint(HintManager.Instance.GetHintByName("PickUpHint"), pickUpHintActive);
        HintManager.Instance.ActivateHint(HintManager.Instance.GetHintByName("InventoryFullHint"), inventoryFullActive);
    }
}
