using UnityEngine;

// Raises the difficulty with the distance travelled: faster world, denser obstacles.
public class Difficulty : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Spawner spawner;

    [Header("Speed")]
    [SerializeField] private float startSpeed = 20f;
    [SerializeField] private float maxSpeed = 40f;

    [Header("Density")]
    [SerializeField] private float startRowSpacing = 18f;
    [SerializeField] private float minRowSpacing = 14f;
    [SerializeField, Range(0f, 1f)] private float startObstacleChance = 0.7f;
    [SerializeField, Range(0f, 1f)] private float maxObstacleChance = 0.9f;

    [Header("Ramp")]
    [SerializeField] private float distanceToMaxDifficulty = 6000f;

    public float Distance { get; private set; }   // total road travelled
    public float Level { get; private set; }      // 0 = easiest, 1 = hardest

    private void Start()
    {
        Distance = 0f;
        Level = 0f;
        Apply();
    }

    private void Update()
    {
        // After a crash or running out of fuel the world stays stopped.
        if (playerHealth != null && playerHealth.IsDead) return;

        Distance += GameSpeed.Current * Time.deltaTime;
        Level = Mathf.Clamp01(Distance / distanceToMaxDifficulty);
        Apply();
    }

    private void Apply()
    {
        GameSpeed.Current = Mathf.Lerp(startSpeed, maxSpeed, Level);

        if (spawner != null)
        {
            spawner.RowSpacing = Mathf.Lerp(startRowSpacing, minRowSpacing, Level);
            spawner.ObstacleChance = Mathf.Lerp(startObstacleChance, maxObstacleChance, Level);
        }
    }
}