using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Spawns rows of obstacles and pickups at the far end of the road, from pools.
// Fairness rules:
//  - One "safe" lane never gets an obstacle, and it moves at most one lane per row.
//  - Only one taxi (moving car) at a time. While it is on the road it owns one lane,
//    and parked cars only use one other fixed lane, so the safe lane stays free.
public class Spawner : MonoBehaviour
{
    [Header("Prefabs (must be registered in PoolManager)")]
    [SerializeField] private GameObject staticCarPrefab;
    [SerializeField] private GameObject movingCarPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject fuelCanPrefab;

    [Header("Placement")]
    [SerializeField] private float spawnZ = 100f;
    [SerializeField] private float rowSpacing = 18f;

    [Header("Chances (per row)")]
    [SerializeField, Range(0f, 1f)] private float obstacleChance = 0.7f;
    [SerializeField, Range(0f, 1f)] private float movingCarChance = 0.35f;
    [SerializeField, Range(0f, 1f)] private float doubleObstacleChance = 0.3f;
    [SerializeField, Range(0f, 1f)] private float coinChance = 0.5f;
    [SerializeField, Range(0f, 1f)] private float fuelChance = 0.06f;

    public float RowSpacing
    {
        get => rowSpacing;
        set => rowSpacing = Mathf.Max(6f, value);
    }

    public float ObstacleChance
    {
        get => obstacleChance;
        set => obstacleChance = Mathf.Clamp01(value);
    }

    private float distanceSinceRow;
    private readonly bool[] laneUsed = new bool[RoadConfig.LaneCount];

    private int safeLane = 1;          // never gets an obstacle
    private MovingCar activeTaxi;      // the taxi currently on the road, if any
    private int taxiLane = -1;
    private int blockedLane = -1;      // the only lane parked cars may use while a taxi is out
    private float protectedUntil;   // Time.time until which parked cars stay in the blocked lane
    [SerializeField] private int minColumnLength = 2;
    [SerializeField] private int maxColumnLength = 3;
    private int columnLimit;   // how many parked cars this taxi's window may place
    private int columnCount;

    private void Update()
    {
        distanceSinceRow += GameSpeed.Current * Time.deltaTime;

        while (distanceSinceRow >= rowSpacing)
        {
            distanceSinceRow -= rowSpacing;
            SpawnRow(spawnZ - distanceSinceRow);
        }
    }

    private void SpawnRow(float z)
    {
        Array.Clear(laneUsed, 0, laneUsed.Length);

        bool taxiOnRoad = activeTaxi != null && activeTaxi.gameObject.activeInHierarchy;
        if (!taxiOnRoad) activeTaxi = null;

        bool protecting = taxiOnRoad && Time.time < protectedUntil;

        if (taxiOnRoad)
        {
            laneUsed[taxiLane] = true;   // keep pickups and cars out of the taxi's lane
        }

        if (!protecting)
        {
            // The safe lane drifts by at most one lane per row, so it is always reachable.
            safeLane = Mathf.Clamp(safeLane + Random.Range(-1, 2), 0, RoadConfig.LaneCount - 1);
        }

        // Obstacles
        if (Random.value < obstacleChance)
        {
            if (protecting)
            {
                // Parked cars use only the one fixed lane, and only up to the column limit.
                if (columnCount < columnLimit)
                {
                    Spawn(staticCarPrefab, blockedLane, z);
                    laneUsed[blockedLane] = true;
                    columnCount++;
                }
            }
            else if (!taxiOnRoad && Random.value < movingCarChance)
            {
                int lane = PickLane(true);
                if (lane >= 0)
                {
                    GameObject taxi = Spawn(movingCarPrefab, lane, z);
                    laneUsed[lane] = true;

                    if (taxi != null)
                    {
                        activeTaxi = taxi.GetComponent<MovingCar>();
                        taxiLane = lane;
                        blockedLane = OtherLane(lane, safeLane);
                        // How long a later parked car could still overtake this taxi in front of the player.
                        float own = activeTaxi.OwnSpeed;
                        float speed = GameSpeed.Current;
                        float relative = Mathf.Max(0.1f, speed - own);
                        float overtakeTime = own * (spawnZ + 10f) / (relative * speed);
                        protectedUntil = Time.time + overtakeTime * 1.25f + 0.5f;   // small safety margin
                        columnCount = 0;
                        columnLimit = Random.Range(minColumnLength, maxColumnLength + 1);
                    }
                }
            }
            else
            {
                int count = Random.value < doubleObstacleChance ? 2 : 1;
                count = Mathf.Min(count, RoadConfig.LaneCount - 1);   // never block every lane

                for (int i = 0; i < count; i++)
                {
                    int lane = PickLane(true);
                    if (lane < 0) break;
                    Spawn(staticCarPrefab, lane, z);
                    laneUsed[lane] = true;
                }
            }
        }

        // Pickups go in any lane that is still unused.
        if (Random.value < coinChance)
        {
            int lane = PickLane(false);
            if (lane >= 0)
            {
                Spawn(coinPrefab, lane, z);
                laneUsed[lane] = true;
            }
        }

        if (Random.value < fuelChance)
        {
            int lane = PickLane(false);
            if (lane >= 0)
            {
                Spawn(fuelCanPrefab, lane, z);
                laneUsed[lane] = true;
            }
        }
    }

    // Random unused lane (optionally never the safe lane), or -1 if none.
    private int PickLane(bool avoidSafe)
    {
        int start = Random.Range(0, RoadConfig.LaneCount);
        for (int i = 0; i < RoadConfig.LaneCount; i++)
        {
            int lane = (start + i) % RoadConfig.LaneCount;
            if (laneUsed[lane]) continue;
            if (avoidSafe && lane == safeLane) continue;
            return lane;
        }
        return -1;
    }

    // The lane that is neither a nor b (works for exactly 3 lanes).
    private int OtherLane(int a, int b)
    {
        for (int lane = 0; lane < RoadConfig.LaneCount; lane++)
            if (lane != a && lane != b) return lane;
        return -1;
    }

    private GameObject Spawn(GameObject prefab, int lane, float z)
    {
        if (prefab == null) return null;
        return PoolManager.Instance.Get(prefab, new Vector3(RoadConfig.LaneX(lane), 0f, z), Quaternion.identity);
    }
}