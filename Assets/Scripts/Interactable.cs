using UnityEngine;

// Base class for everything the player can run into: obstacle cars, coins, fuel cans.
// Handles scrolling with the road, contact detection and returning to the pool.
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private float despawnZ = -15f;

    private bool consumed;   // makes sure one object triggers only once per spawn

    // Called by Unity every time the pool activates this object.
    private void OnEnable()
    {
        consumed = false;
        OnSpawn();
    }

    // Subclasses override these to reset state or add behaviour.
    protected virtual void OnSpawn() { }
    protected virtual void OnDespawn() { }

    // Default movement: scroll toward the camera with the road (treadmill).
    protected virtual void Tick(float deltaTime)
    {
        transform.position += Vector3.back * (GameSpeed.Current * deltaTime);
    }

    // What happens when the player touches this object. Every subclass defines it.
    protected abstract void OnPlayerContact(PlayerController player);

    private void Update()
    {
        Tick(Time.deltaTime);

        if (transform.position.z < despawnZ)
            ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (consumed) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        consumed = true;
        OnPlayerContact(player);
    }

    // Switches the object off and gives it back to the pool (no Destroy).
    public void ReturnToPool()
    {
        if (!gameObject.activeSelf) return;   // already returned

        OnDespawn();
        PoolManager.Instance.Release(gameObject);
    }
}