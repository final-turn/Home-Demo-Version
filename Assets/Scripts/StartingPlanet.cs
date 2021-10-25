using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPlanet : MonoBehaviour
{
    public string type;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(type=="gravityField")
        {
            if (other.name == "PLAYER")
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.ApproachStartingPoint();
            }
        }

        if (type == "platform")
        {
            if (other.name == "PLAYER")
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.DockShip();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (type == "gravityField")
        {
            if (other.name == "PLAYER")
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.LeaveStartingPoint();
            }
        }
    }
}
