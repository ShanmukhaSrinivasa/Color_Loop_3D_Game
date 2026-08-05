using UnityEngine;

public class LogoFloat : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float floatHeight = 0.8f;
    [SerializeField] private float floatspeed = 1.5f;

    [Header("Rotation")]
    [SerializeField] private float rotationAngle = 2f;
    [SerializeField] private float rotationSpeed = 1f;

    [Header("Scale")]
    [SerializeField] private float scaleAmount = 0.03f;
    [SerializeField] private float scaleSpeed = 1.2f;

    private Vector3 startPosition;
    private Vector3 startScale;

    private void Start()
    {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
    }

    private void Update()
    {
        float t = Time.unscaledTime;

        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(t * floatspeed) * floatHeight;
        transform.localPosition = pos;

        float angle = Mathf.Sin(t * rotationSpeed) * rotationAngle;
        transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);

        float scale = 1f + Mathf.Sin(t * scaleSpeed) * scaleAmount;
        transform.localScale = startScale * scale;
    }
}
