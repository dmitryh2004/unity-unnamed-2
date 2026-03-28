using UnityEngine;

public class WallCollider1AprilTroll : MonoBehaviour
{
    [SerializeField] Collider wallCollider;

    private void Start()
    {
        if (DayCheck.Instance.IsApril1)
        {
            wallCollider.enabled = false;
        }
    }
}
