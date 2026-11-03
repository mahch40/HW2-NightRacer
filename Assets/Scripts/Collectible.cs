using System.Collections;
using UnityEngine;

// Shared behaviour for pickups (coins, fuel cans): they spin, and when collected
// they shrink and rise before going back to the pool.
public abstract class Collectible : Interactable
{
    [SerializeField] private float spinSpeed = 120f;
    [SerializeField] private float collectDuration = 0.25f;
    [SerializeField] private float collectRise = 1.5f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        transform.localScale = originalScale;   // undo the shrink from the last pickup
    }

    protected override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);   // keep scrolling with the road
        transform.Rotate(0f, spinSpeed * deltaTime, 0f, Space.World);
    }

    // Interactable demands this; Collectible answers it once for all pickups.
    protected override void OnPlayerContact(PlayerController player)
    {
        OnCollected(player);
        StartCoroutine(CollectEffect());
    }

    // Coin / FuelCan define what the pickup actually gives.
    protected abstract void OnCollected(PlayerController player);

    private IEnumerator CollectEffect()
    {
        float elapsed = 0f;
        while (elapsed < collectDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / collectDuration);
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            transform.position += Vector3.up * (collectRise / collectDuration) * Time.deltaTime;
            yield return null;
        }
        ReturnToPool();
    }
}