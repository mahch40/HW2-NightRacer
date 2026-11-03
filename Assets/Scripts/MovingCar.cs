using UnityEngine;

// A car driving in the same direction as the player, but slower.
public class MovingCar : Interactable
{
    [SerializeField] private float minOwnSpeed = 6f;
    [SerializeField] private float maxOwnSpeed = 12f;

    private float ownSpeed;
    public float OwnSpeed => ownSpeed;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        ownSpeed = Random.Range(minOwnSpeed, maxOwnSpeed);
    }

    // The world scrolls back at GameSpeed, the car drives forward at ownSpeed,
    // so it approaches the player at the difference.
    protected override void Tick(float deltaTime)
    {
        transform.position += Vector3.back * ((GameSpeed.Current - ownSpeed) * deltaTime);
    }

    protected override void OnPlayerContact(PlayerController player)
    {
        Debug.Log($"{name} hit the player (life system comes in task 15)");
        ReturnToPool();
    }
}