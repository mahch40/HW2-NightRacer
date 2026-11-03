using System.Collections;
using UnityEngine;

// Shared behaviour for pickups (coins, fuel cans): they spin, and when collected
// they stop scrolling and play a short shrink-and-rise effect in place.
public abstract class Collectible : Interactable
{
    [SerializeField] private float spinSpeed = 120f;
    [SerializeField] private float collectSpinMultiplier = 4f;
    [SerializeField] private float collectDuration = 0.3f;
    [SerializeField] private float collectRise = 1.5f;

    [Header("Pickup sparkle")]
    [SerializeField] private GameObject sparklePrefab;
    [SerializeField] private Color sparkleColor = Color.white;

    private Vector3 originalScale;
    private bool collecting;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        collecting = false;
        transform.localScale = originalScale;   // undo the shrink from the last pickup
    }

    protected override void Tick(float deltaTime)
    {
        if (collecting)
        {
            transform.Rotate(0f, spinSpeed * collectSpinMultiplier * deltaTime, 0f, Space.World);
            return;
        }

        base.Tick(deltaTime);   // keep scrolling with the road
        transform.Rotate(0f, spinSpeed * deltaTime, 0f, Space.World);
    }

    protected override void OnPlayerContact(PlayerController player)
    {
        collecting = true;
        OnCollected(player);

        if (sparklePrefab != null && PoolManager.Instance != null)
        {
            PickupSparkle sparkle = PoolManager.Instance.Get<PickupSparkle>(sparklePrefab, transform.position, Quaternion.identity);
            if (sparkle != null) sparkle.Play(sparkleColor);
        }

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