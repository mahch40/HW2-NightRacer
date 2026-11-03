using System.Collections;
using UnityEngine;

// One pooled sparkle burst for pickups. Colored by the item, returns itself to the pool.
public class PickupSparkle : MonoBehaviour
{
    [SerializeField] private ParticleSystem sparkle;
    [SerializeField] private float lifetime = 1f;

    public void Play(Color color)
    {
        ParticleSystem.MainModule main = sparkle.main;
        main.startColor = color;
        sparkle.Clear();
        sparkle.Play();
        StartCoroutine(ReturnAfterDelay());
    }

    private IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(lifetime);
        PoolManager.Instance.Release(gameObject);
    }
}