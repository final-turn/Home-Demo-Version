using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPlanet : MonoBehaviour
{
    [SerializeField] private string type;

    private bool isDocked = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "PLAYER")
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (type == "gravityField")
            {
                player.ApproachStartingPoint();
            }

            if (type == "platform")
            {
                if (!isDocked)
                {
                    player.DockShip();
                    isDocked = true;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (type == "platform" && other.name == "PLAYER")
        {
            isDocked = false;
        }
    }
}
