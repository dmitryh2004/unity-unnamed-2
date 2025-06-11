using UnityEngine;

public class NodeImages : MonoBehaviour
{
    public static NodeImages Instance = null;
    public Sprite notAccessibleNode, accessibleNode, visitedNode;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
