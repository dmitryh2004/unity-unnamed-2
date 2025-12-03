using System.Collections.Generic;
using UnityEngine;

public class JewerlyTableCraftManager : MonoBehaviour
{
    public static JewerlyTableCraftManager Instance = null;
    [SerializeField] List<JewerlyTableCraft> crafts = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public JewerlyTableCraft GetCraftByName(string name)
    {
        return crafts.Find((x) => x.craftName == name) ?? null;
    }
}
