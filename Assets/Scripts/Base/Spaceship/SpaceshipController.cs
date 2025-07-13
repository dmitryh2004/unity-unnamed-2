using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public static SpaceshipController Instance = null;
    Complex currentComplex;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Complex GetCurrentComplex() => currentComplex;
    public void SetCurrentComplex(Complex complex) => currentComplex = complex;
}
