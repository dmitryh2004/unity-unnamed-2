using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HintManager : MonoBehaviour
{
    [SerializeField] List<TMP_Text> hints;
    [SerializeField] List<string> hintNames;
    Dictionary<int, TMP_Text> hintMapping = new();
    Dictionary<string, int> hintNamesMapping = new();
    public static HintManager Instance = null;

    private void Start()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (hints.Count != hintNames.Count)
        {
            Debug.LogError($"hint manager: count of hints not equal to hint names count");
            return;
        }
        for (int i = 0; i < hints.Count; i++)
        {
            hintMapping.Add(i, hints[i]);
            hintNamesMapping.Add(hintNames[i], i);
            ActivateHint(i, false);
        }
    }

    public int GetHintByName(string name)
    {
        return (hintNamesMapping.ContainsKey(name)) ? hintNamesMapping[name] : -1;
    }

    public void ActivateHint(int index, bool active)
    {
        hintMapping[index].gameObject.SetActive(active);
    }

    public TMP_Text GetHintText(int index)
    {
        return (hintMapping.ContainsKey(index)) ? hintMapping[index] : null;
    }

    public void HideAll()
    {
        foreach (int i in hintMapping.Keys)
        {
            ActivateHint(i, false);
        }
    }
 }
