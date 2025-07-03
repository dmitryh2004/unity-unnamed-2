using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVisibilityManager : MonoBehaviour
{
    [SerializeField] List<GameObject> hideables = new();
    [SerializeField] Camera playerCamera;
    [SerializeField] string hiddenLayerName = "HiddenFromCamera";
    [SerializeField] string hiddenTag = "HiddenFromCamera";
    PlayerControls controls;
    InputAction checkFOV;

    List<KeyValuePair<GameObject, int>> _hideables = new();
    bool currentVisible = false;
    int hiddenLayer = 0;

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Awake()
    {
        controls = new();
        checkFOV = controls.Gameplay.CheckFOV;
        hiddenLayer = LayerMask.NameToLayer(hiddenLayerName);

        if (playerCamera == null) playerCamera = Camera.main;

        // auto find objects to hide by tag
        var temp = GameObject.FindGameObjectsWithTag(hiddenTag).ToList<GameObject>();

        foreach (GameObject t in temp)
        {
            if (!hideables.Contains(t)) hideables.Add(t);
        }

        // add hideable objects to kvp
        foreach (GameObject hideable in hideables)
        {
            _hideables.Add(new(hideable, hideable.layer));
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HideObjects();
    }

    // Update is called once per frame
    void Update()
    {
        bool isPressed = checkFOV.ReadValue<float>() > 0.5f;

        if (currentVisible != isPressed)
        {
            currentVisible = isPressed;
            if (currentVisible) ShowObjects(); else HideObjects();
        }
    }

    void HideObjects()
    {
        foreach(var hideable in _hideables)
        {
            hideable.Key.layer = hiddenLayer;
            Light light;
            if (hideable.Key.TryGetComponent<Light>(out light)) hideable.Key.SetActive(false);
        }
    }

    void ShowObjects()
    {
        foreach (var hideable in _hideables)
        {
            hideable.Key.layer = hideable.Value;
            Light light;
            if (hideable.Key.TryGetComponent<Light>(out light)) hideable.Key.SetActive(true);
        }
    }
}
