using UnityEngine;

// Launches pooled fireworks at random spots in the sky ahead of the player.
public class FireworkLauncher : MonoBehaviour
{
    [SerializeField] private GameObject fireworkPrefab;

    [Header("Timing")]
    [SerializeField] private float minInterval = 1.5f;
    [SerializeField] private float maxInterval = 4f;

    [Header("Launch area (rockets rise from the ground at this X/Z)")]
    [SerializeField] private float minX = -60f;
    [SerializeField] private float maxX = 60f;
    [SerializeField] private float minZ = 80f;
    [SerializeField] private float maxZ = 130f;

    private float timer;

    private void Start()
    {
        timer = Random.Range(1f, 2.5f);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = Random.Range(minInterval, maxInterval);

        Vector3 position = new Vector3(Random.Range(minX, maxX), 0f, Random.Range(minZ, maxZ));
        PoolManager.Instance.Get(fireworkPrefab, position, Quaternion.identity);
    }
}