using System.Collections.Generic;
using UnityEngine;

// Keeps an endless road in front of the player by recycling pooled segments.
// The player stays still; the road scrolls toward the camera (treadmill).
public class RoadManager : MonoBehaviour
{
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private int segmentCount = 8;
    [SerializeField] private float recycleBehindZ = -30f;

    // Ordered from nearest (index 0) to farthest.
    private readonly List<Transform> segments = new List<Transform>();

    private void Start()
    {
        // Start one segment behind the player so the camera never sees the road's end.
        float z = -RoadConfig.SegmentLength;
        for (int i = 0; i < segmentCount; i++)
        {
            SpawnSegment(z);
            z += RoadConfig.SegmentLength;
        }
    }

    private void Update()
    {
        float move = GameSpeed.Current * Time.deltaTime;
        foreach (Transform segment in segments)
            segment.position += Vector3.back * move;

        // Recycle the first segment once it is completely behind the player.
        while (segments.Count > 0 &&
               segments[0].position.z + RoadConfig.SegmentLength < recycleBehindZ)
        {
            Transform first = segments[0];
            segments.RemoveAt(0);

            float nextZ = segments[segments.Count - 1].position.z + RoadConfig.SegmentLength;
            PoolManager.Instance.Release(first.gameObject);
            SpawnSegment(nextZ);
        }
    }

    private void SpawnSegment(float z)
    {
        GameObject segment = PoolManager.Instance.Get(segmentPrefab, new Vector3(0f, 0f, z), Quaternion.identity);
        if (segment != null) segments.Add(segment.transform);
    }
}