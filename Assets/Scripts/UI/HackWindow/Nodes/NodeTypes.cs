using System.Collections.Generic;
using UnityEngine;

public class NodeTypes : MonoBehaviour
{
    public static NodeTypes Instance = null;
    public NodeType CentralCore, WallNode, AntivirusNode, RepairNode, PacifierNode;
    Dictionary<BonusType, NodeType> BonusNodes = new();
    [SerializeField] List<BonusType> bonusTypes = new();
    [SerializeField] List<NodeType> bonusNodes = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        if (bonusTypes.Count != bonusNodes.Count)
        {
            Debug.LogError($"{gameObject.name} - NodeTypes: amount of BonusType and NodeType not equal");
            return;
        }

        for (int i = 0; i < bonusTypes.Count; i++)
        {
            BonusNodes.Add(bonusTypes[i], bonusNodes[i]);
        }
    }

    public NodeType BonusNode(BonusType type)
    {
        return BonusNodes[type];
    }
}
