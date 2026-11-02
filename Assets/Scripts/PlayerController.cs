using UnityEngine;
using UnityEngine.InputSystem;

// Lane-based player movement. The car never moves forward (the road scrolls),
// it only changes lanes and tilts its model while doing so.
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform model;   // visual child that tilts

    [Header("Lane change")]
    [SerializeField] private float laneSmoothTime = 0.1f;

    [Header("Tilt")]
    [SerializeField] private float maxYaw = 12f;
    [SerializeField] private float maxRoll = 6f;
    [SerializeField] private float tiltReferenceSpeed = 15f;
    [SerializeField] private float tiltSmoothing = 10f;

    private GameInput input;
    private int currentLane = 1;   // 0 = left, 1 = middle, 2 = right
    private float velocityX;

    public int CurrentLane => currentLane;

    private void Awake()
    {
        input = new GameInput();
    }

    private void OnEnable()
    {
        input.Driving.Enable();
        input.Driving.MoveLeft.performed += OnMoveLeft;
        input.Driving.MoveRight.performed += OnMoveRight;
    }

    private void OnDisable()
    {
        input.Driving.MoveLeft.performed -= OnMoveLeft;
        input.Driving.MoveRight.performed -= OnMoveRight;
        input.Driving.Disable();
    }

    private void OnMoveLeft(InputAction.CallbackContext ctx) { ChangeLane(-1); }
    private void OnMoveRight(InputAction.CallbackContext ctx) { ChangeLane(1); }

    private void ChangeLane(int direction)
    {
        if (Time.timeScale == 0f) return;   // paused: ignore input
        currentLane = Mathf.Clamp(currentLane + direction, 0, RoadConfig.LaneCount - 1);
    }

    private void Update()
    {
        // Glide toward the target lane center.
        Vector3 position = transform.position;
        position.x = Mathf.SmoothDamp(position.x, RoadConfig.LaneX(currentLane), ref velocityX, laneSmoothTime);
        transform.position = position;

        // Tilt the model based on how fast we are moving sideways.
        if (model != null)
        {
            float t = Mathf.Clamp(velocityX / tiltReferenceSpeed, -1f, 1f);
            Quaternion target = Quaternion.Euler(0f, t * maxYaw, -t * maxRoll);
            model.localRotation = Quaternion.Slerp(model.localRotation, target, tiltSmoothing * Time.deltaTime);
        }
    }
}