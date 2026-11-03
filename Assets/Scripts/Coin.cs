using UnityEngine;

public class Coin : Collectible
{
    [SerializeField] private int scoreValue = 10;

    protected override void OnCollected(PlayerController player)
    {
        Debug.Log($"Coin collected: +{scoreValue} score (GameManager comes in task 18)");
    }
}