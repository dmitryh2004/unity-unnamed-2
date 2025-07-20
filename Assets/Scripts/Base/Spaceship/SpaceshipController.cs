using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public static SpaceshipController Instance = null;
    Complex currentComplex;
    [SerializeField] SpaceshipPanelController panel;
    [SerializeField] SpaceshipLeverController lever;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public SpaceshipPanelController GetPanelController() => panel;
    public SpaceshipLeverController GetLeverController() => lever;

    public Complex GetCurrentComplex() => currentComplex;
    public void SetCurrentComplex(Complex complex) => currentComplex = complex;
}
