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
                Debug.Log("Enter");
                PlayerController player = other.GetComponent<PlayerController>();
                player.awayFromCenter = false;
                player.ResetForces();
            }
        }

        if (type == "platform")
        {
            if (other.name == "PLAYER")
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.transform.position = Vector3.zero;
                player.ResetForces();
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
                player.awayFromCenter = true;
            }
        }
    }
}
