using UnityEngine;

public class Coin : Collectible
{
    [SerializeField] private int scoreValue = 10;

    protected override void OnCollected(PlayerController player)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCoin();
    }
}