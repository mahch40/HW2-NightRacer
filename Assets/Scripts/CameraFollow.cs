using UnityEngine;

// Third-person chase camera: behind and above the player, widens its FOV with speed,
// and can shake (called by the hit feedback later).
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 3.2f, -7f);
    [SerializeField, Range(0f, 1f)] private float sidewaysFollow = 0.6f;
    [SerializeField] private float followSmoothTime = 0.15f;

    [Header("Look")]
    [SerializeField] private float lookAheadZ = 8f;
    [SerializeField] private float lookHeight = 1f;

    [Header("Speed FOV")]
    [SerializeField] private float baseFov = 60f;
    [SerializeField] private float maxFov = 72f;
    [SerializeField] private float minSpeed = 20f;
    [SerializeField] private float maxSpeed = 45f;
    [SerializeField] private float fovSmoothing = 3f;

    private Camera cam;
    private float currentX;
    private float velocityX;

    private float shakeTime;
    private float shakeDuration = 0.01f;
    private float shakeMagnitude;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = baseFov;
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = Mathf.Max(0.01f, duration);
        shakeTime = duration;
        shakeMagnitude = magnitude;
    }

    [ContextMenu("Test Shake")]
    private void TestShake()
    {
        Shake(0.4f, 0.3f);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Follow the player sideways, only partly and smoothly.
        float desiredX = target.position.x * sidewaysFollow;
        currentX = Mathf.SmoothDamp(currentX, desiredX, ref velocityX, followSmoothTime);

        Vector3 position = new Vector3(
            currentX + offset.x,
            target.position.y + offset.y,
            target.position.z + offset.z);

        // Shake fades out over its duration.
        if (shakeTime > 0f)
        {
            shakeTime -= Time.deltaTime;
            float fade = Mathf.Clamp01(shakeTime / shakeDuration);
            position += Random.insideUnitSphere * shakeMagnitude * fade;
        }

        transform.position = position;
        transform.LookAt(target.position + new Vector3(0f, lookHeight, lookAheadZ));

        // Wider field of view as the game speeds up.
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, GameSpeed.Current);
        float targetFov = Mathf.Lerp(baseFov, maxFov, t);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, fovSmoothing * Time.deltaTime);
    }
}