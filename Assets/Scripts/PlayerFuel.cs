using System;
using UnityEngine;

// The fuel tank: drains over time, refilled by fuel cans. Empty tank = game over.
public class PlayerFuel : MonoBehaviour
{
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float drainPerSecond = 3f;
    [SerializeField, Range(0f, 1f)] private float lowFuelThreshold = 0.25f;

    public event Action<float> OnFuelChanged;   // normalized 0..1
    public event Action OnFuelEmpty;

    public float Fuel { get; private set; }
    public float MaxFuel => maxFuel;
    public float Normalized => Fuel / maxFuel;
    public bool IsLow => Normalized <= lowFuelThreshold;

    private PlayerHealth health;
    private bool empty;

    private void Awake()
    {
        Fuel = maxFuel;
        health = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (empty) return;
        if (health != null && health.IsDead) return;   // no draining after a crash

        Fuel = Mathf.Max(0f, Fuel - drainPerSecond * Time.deltaTime);
        OnFuelChanged?.Invoke(Normalized);

        if (Fuel <= 0f)
        {
            empty = true;
            Debug.Log("Out of fuel");
            OnFuelEmpty?.Invoke();
            if (health != null) health.Kill();
        }
    }

    public void Refill(float amount)
    {
        if (empty) return;
        Fuel = Mathf.Min(maxFuel, Fuel + amount);
        OnFuelChanged?.Invoke(Normalized);
    }
}