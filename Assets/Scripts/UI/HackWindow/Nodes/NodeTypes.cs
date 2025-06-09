using UnityEngine;

public class NodeTypes : MonoBehaviour
{
    public static NodeTypes Instance = null;
    public NodeType CentralCore, WallNode, AntivirusNode, RepairNode, PacifierNode;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
