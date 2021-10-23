using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCollider : MonoBehaviour
{
    public float gravityStrength = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "PLAYER")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.AddGravitySource(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "PLAYER")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.RemoveGravitySource(this);
        }
    }
}
