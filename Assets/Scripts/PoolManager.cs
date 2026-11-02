using System.Collections.Generic;
using UnityEngine;

// Owns one ObjectPool per prefab. Other scripts only call Get() and Release().
public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int prewarmCount = 10;
    }

    public static PoolManager Instance { get; private set; }

    [SerializeField] private List<PoolEntry> entries = new List<PoolEntry>();

    // prefab -> its pool
    private readonly Dictionary<GameObject, ObjectPool<Transform>> pools =
        new Dictionary<GameObject, ObjectPool<Transform>>();

    // active instance -> the pool it must go back to
    private readonly Dictionary<GameObject, ObjectPool<Transform>> owners =
        new Dictionary<GameObject, ObjectPool<Transform>>();

    private void Awake()
    {
        Instance = this;

        foreach (PoolEntry entry in entries)
        {
            if (entry.prefab == null) continue;

            Transform holder = new GameObject("Pool_" + entry.prefab.name).transform;
            holder.SetParent(transform);
            pools[entry.prefab] = new ObjectPool<Transform>(entry.prefab.transform, entry.prewarmCount, holder);
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!pools.TryGetValue(prefab, out ObjectPool<Transform> pool))
        {
            Debug.LogError($"No pool registered for prefab '{prefab.name}'. Add it to the PoolManager list.");
            return null;
        }

        Transform item = pool.Get(position, rotation);
        if (item == null) return null; // pool empty, already warned

        owners[item.gameObject] = pool;
        return item.gameObject;
    }

    // Same as Get, but returns a component on the object (e.g. an Interactable).
    public T Get<T>(GameObject prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        GameObject item = Get(prefab, position, rotation);
        return item != null ? item.GetComponent<T>() : null;
    }

    public void Release(GameObject instance)
    {
        if (owners.TryGetValue(instance, out ObjectPool<Transform> pool))
        {
            owners.Remove(instance);
            pool.Release(instance.transform);
        }
        else
        {
            Debug.LogWarning($"'{instance.name}' was not taken from a pool, so it can't be released.");
        }
    }
}