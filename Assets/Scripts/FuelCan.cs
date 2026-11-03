using UnityEngine;

public class FuelCan : Collectible
{
    [SerializeField] private float fuelAmount = 30f;

    protected override void OnCollected(PlayerController player)
    {
        PlayerFuel fuel = player.GetComponent<PlayerFuel>();
        if (fuel != null) fuel.Refill(fuelAmount);
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFuel();
    }
    
}