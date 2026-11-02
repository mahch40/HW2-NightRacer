using UnityEngine;

// TEMPORARY: only for testing the obstacles. Deleted when the real Spawner is built (task 14).
public class TestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private float interval = 2f;
    [SerializeField] private float spawnZ = 100f;

    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = interval;

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        int lane = Random.Range(0, RoadConfig.LaneCount);
        Vector3 position = new Vector3(RoadConfig.LaneX(lane), 0f, spawnZ);

        PoolManager.Instance.Get(prefab, position, Quaternion.identity);
    }
}