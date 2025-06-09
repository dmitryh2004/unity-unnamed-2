using UnityEngine;

public class HackWindowController : MonoBehaviour
{
    public static HackWindowController Instance = null;

    [SerializeField] string emptyVisitedNodeTitle, UnvisitedNodeTitle;
    [TextArea(6, 10)]
    [SerializeField] string emptyVisitedNodeText, UnvisitedNodeText;

    [SerializeField] GridController gridController;

    int difficulty;

    public void SetDifficulty(int diff)
    {
        difficulty = Mathf.Clamp(diff, 1, 10);
    }

    public int GetDifficulty()
    {
        return difficulty;
    }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
