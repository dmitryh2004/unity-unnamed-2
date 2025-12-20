using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScannerController : MonoBehaviour
{
    [SerializeField] List<GameObject> hideables = new();
    [SerializeField] Camera playerCamera;
    [SerializeField] string hiddenLayerName = "HiddenFromCamera";
    [SerializeField] string hiddenTag = "HiddenFromCamera";
    [Space]
    [Header("Loot cost hints")]
    [SerializeField] Camera playerHintCamera;
    List<LootHintController> lootHintControllers = new();
    PlayerControls controls;
    InputAction useScanner;

    List<KeyValuePair<GameObject, int>> _hideables = new();
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
        useScanner = controls.Gameplay.UseScanner;
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
        bool isPressed = useScanner.ReadValue<float>() > 0.5f;

        if (PlayerScanner.Instance.InUse() != isPressed)
        {
            PlayerScanner.Instance.SetInUse(isPressed);
        }

        bool isActive = PlayerScanner.Instance.IsActive();
        if (isActive) ShowObjects(); else HideObjects();
    }

    public void FindLootCostHints()
    {
        lootHintControllers = FindObjectsByType<LootHintController>(FindObjectsSortMode.None).ToList();
    }

    void HideObjects()
    {
        foreach(var hideable in _hideables)
        {
            hideable.Key.layer = hiddenLayer;
            Light light;
            if (hideable.Key.TryGetComponent<Light>(out light)) hideable.Key.SetActive(false);
        }

        playerHintCamera.gameObject.SetActive(false);
        foreach (var l in lootHintControllers)
        {
            l.gameObject.SetActive(false);
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

        playerHintCamera.gameObject.SetActive(true);
        foreach (var l in lootHintControllers)
        {
            l.gameObject.SetActive(true);
            l.UpdateLootSum();
        }
    }
}
