using UnityEngine;

public class Virus : MonoBehaviour
{
    public static Virus Instance;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    public int GetCurrentAttack()
    {
        throw new System.NotImplementedException();
    }
}
