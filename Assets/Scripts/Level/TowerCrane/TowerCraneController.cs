using UnityEngine;

public class TowerCraneController : MonoBehaviour
{
    [SerializeField] Transform rotatingPart;
    [Header("Settings")]
    [SerializeField] float checkTimer = 10f;
    [SerializeField] float rotateChance = 1f;
    [Range(0, 179)] [SerializeField] float minRotationAngle = 60f;
    [Range(0, 179)] [SerializeField] float maxRotationAngle = 150f;
    [SerializeField] float rotationSpeed = 15f;

    float currentRotation = 0f;
    float targetRotation = 0f;
    bool isRotating = false;

    private void Start()
    {
        InvokeRepeating(nameof(SetNewTargetRotation), checkTimer, checkTimer);
    }

    private void SetNewTargetRotation()
    {
        if (isRotating) return;

        float diff = Random.Range(minRotationAngle, maxRotationAngle);

        if (Random.Range(0f, 1f) < 0.5f) diff *= -1;

        targetRotation += diff;
    }

    private void Update()
    {
        float diff = 0f;
        if (Mathf.Abs(currentRotation - targetRotation) >= 1f)
        {
            isRotating = true;
            diff = rotationSpeed * Time.deltaTime * Mathf.Sign(targetRotation - currentRotation);
        }
        else
        {
            isRotating = false;
            if (currentRotation != targetRotation)
            {
                diff = targetRotation - currentRotation;
            }
        }
        
        if (diff != 0f)
        {
            rotatingPart.Rotate(Vector3.up, diff);
            currentRotation += diff;
        }
    }
}
