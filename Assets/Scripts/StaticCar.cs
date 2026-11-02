using UnityEngine;

// A parked car in one lane. It never drives, so the default Interactable.Tick
// (scroll with the road) is all it needs.
public class StaticCar : Interactable
{
    protected override void OnPlayerContact(PlayerController player)
    {
        Debug.Log($"{name} hit the player (life system comes in task 15)");
        ReturnToPool();
    }
}