using UnityEngine;

// Shared behaviour for anything that hurts the player on contact.
public abstract class Obstacle : Interactable
{
    protected override void OnPlayerContact(PlayerController player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null) health.TakeHit();
        ReturnToPool();
    }
}
