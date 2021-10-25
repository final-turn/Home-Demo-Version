using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSphereOfInfluence : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "PLAYER")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.ApproachStartingPoint();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "PLAYER")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.LeaveStartingPoint();
        }
    }
}
