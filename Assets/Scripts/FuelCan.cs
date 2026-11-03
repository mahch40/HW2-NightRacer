using UnityEngine;

public class FuelCan : Collectible
{
    [SerializeField] private float fuelAmount = 30f;

    protected override void OnCollected(PlayerController player)
    {
        Debug.Log($"Fuel can collected: +{fuelAmount} fuel (fuel system comes in task 16)");
    }
}