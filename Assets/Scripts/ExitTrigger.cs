using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private bool chaseStarted = false; // To ensure chase is triggered only once
    public TheEntity entity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !chaseStarted)
        {
            chaseStarted = true;
            entity.ForceChasePlayer(); // Force the entity to chase the player
        }
    }
}
