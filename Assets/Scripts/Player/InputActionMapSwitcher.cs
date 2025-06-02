using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionMapSwitcher : MonoBehaviour
{
    public PlayerInput playerInput;
    public static InputActionMapSwitcher Instance = null;
    [SerializeField] List<string> mapNames = new();
    [SerializeField] List<bool> cursorVisibleOnMap = new();

    [SerializeField] string startMap;

    Dictionary<string, bool> cursorVisibility = new();

    public void SwitchMap(string mapName)
    {
        Debug.Log($"������� ������������ �� �����: {mapName}");
        playerInput.currentActionMap?.Disable();
        playerInput.SwitchCurrentActionMap(mapName);
        Cursor.visible = cursorVisibility[mapName];
        Cursor.lockState = cursorVisibility[mapName] ? CursorLockMode.None : CursorLockMode.Locked;
        playerInput.currentActionMap.Enable();
        Debug.Log($"������� �����: {playerInput.currentActionMap?.name}");
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (mapNames.Count != cursorVisibleOnMap.Count)
        {
            Debug.LogError("Input action map switcher: count of maps not equal to count of cursor visibility entries");
        }
        else
        {
            for (int i = 0; i < mapNames.Count; i++)
            {
                cursorVisibility.Add(mapNames[i], cursorVisibleOnMap[i]);
            }
        }

        // ��������� ��� ����� � �������� ����� Gameplay
        playerInput.actions.Disable();
        SwitchMap(startMap);
    }

    private void Start()
    {
        foreach (InputActionMap map in playerInput.actions.actionMaps)
        {
            Debug.Log($"Map '{map.name}': {(map.enabled ? "Active" : "Inactive")}");
        }
    }
}
