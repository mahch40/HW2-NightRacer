using System.Collections.Generic;
using UnityEngine;

// A pool of pre-made copies of one prefab.
// Get() activates one, Release() switches it off and returns it.
public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Stack<T> inactive = new Stack<T>();

    public ObjectPool(T prefab, int prewarmCount, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;

        // All creation happens here, once, at startup.
        for (int i = 0; i < prewarmCount; i++)
        {
            T item = Object.Instantiate(prefab, parent);
            item.gameObject.SetActive(false);
            inactive.Push(item);
        }
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        if (inactive.Count == 0)
        {
            Debug.LogWarning($"Pool for '{prefab.name}' is empty. Increase its prewarm count.");
            return null;
        }

        T item = inactive.Pop();
        item.transform.SetPositionAndRotation(position, rotation);
        item.gameObject.SetActive(true);
        return item;
    }

    public void Release(T item)
    {
        // Guard against releasing the same object twice.
        if (!item.gameObject.activeSelf) return;

        item.gameObject.SetActive(false);
        item.transform.SetParent(parent);
        inactive.Push(item);
    }
}