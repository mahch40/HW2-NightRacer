using System.Collections;
using UnityEngine;

// One pooled firework: picks random neon colors for its burst and
// returns itself to the pool after the show is over.
public class Firework : MonoBehaviour
{
    [SerializeField] private ParticleSystem burst;
    [SerializeField] private float lifetime = 5f;

    private static readonly Color[] palette =
    {
        new Color32(0x00, 0xE5, 0xFF, 255),   // cyan
        new Color32(0xFF, 0x2B, 0xD6, 255),   // magenta
        new Color32(0xFF, 0xC4, 0x00, 255),   // gold
        new Color32(0x5C, 0xFF, 0x7A, 255),   // green
        new Color32(0xA5, 0x6B, 0xFF, 255),   // violet
        new Color32(0xFF, 0x5A, 0x36, 255),   // orange-red
    };

    private void OnEnable()
    {
        Color a = palette[Random.Range(0, palette.Length)];
        Color b = palette[Random.Range(0, palette.Length)];

        ParticleSystem.MainModule main = burst.main;
        main.startColor = new ParticleSystem.MinMaxGradient(a, b);

        StartCoroutine(ReturnAfterDelay());
    }

    private IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(lifetime);
        PoolManager.Instance.Release(gameObject);
    }
}