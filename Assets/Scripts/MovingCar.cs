using UnityEngine;

// A car driving in the same direction as the player, but slower.
public class MovingCar : Obstacle
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

    protected override void Tick(float deltaTime)
    {
        transform.position += Vector3.back * ((GameSpeed.Current - ownSpeed) * deltaTime);
    }
}