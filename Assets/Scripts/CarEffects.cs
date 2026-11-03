using UnityEngine;

// Exhaust flame and smoke that grow with speed, and sparks when the car is hit.
public class CarEffects : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;

    [Header("Exhaust")]
    [SerializeField] private ParticleSystem exhaustFlame;
    [SerializeField] private ParticleSystem exhaustSmoke;
    [SerializeField] private float minSpeed = 20f;
    [SerializeField] private float maxSpeed = 55f;
    [SerializeField] private Vector2 flameRate = new Vector2(25f, 70f);
    [SerializeField] private Vector2 smokeRate = new Vector2(15f, 45f);
    [SerializeField] private Vector2 flameSpeed = new Vector2(3f, 7f);
    [SerializeField] private Vector2 smokeSpeed = new Vector2(2f, 8f);

    [Header("Hit")]
    [SerializeField] private ParticleSystem hitSparks;

    private void Awake()
    {
        if (health == null) health = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (health != null) health.OnHit += OnHit;
    }

    private void OnDisable()
    {
        if (health != null) health.OnHit -= OnHit;
    }

    private void OnHit()
    {
        if (hitSparks != null) hitSparks.Play();
    }

    private void Update()
    {
        bool dead = health != null && health.IsDead;
        float level = Mathf.InverseLerp(minSpeed, maxSpeed, GameSpeed.Current);

        UpdateExhaust(exhaustFlame, flameRate, flameSpeed, level, dead);
        UpdateExhaust(exhaustSmoke, smokeRate, smokeSpeed, level, dead);
    }

    private static void UpdateExhaust(ParticleSystem ps, Vector2 rate, Vector2 speed, float level, bool stop)
    {
        if (ps == null) return;

        if (stop)
        {
            if (ps.isEmitting) ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            return;
        }

        if (!ps.isPlaying) ps.Play();

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = Mathf.Lerp(rate.x, rate.y, level);

        ParticleSystem.MainModule main = ps.main;
        main.startSpeed = Mathf.Lerp(speed.x, speed.y, level);
    }
}