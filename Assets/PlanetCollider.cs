using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCollider : MonoBehaviour
{
    public string type;
    public bool anchorable = false;
    public float gravityStrength = 0;
    [SerializeField] private float knockbackStrength;

    private PlayerController visitingPlayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (type == "gravityField")
        {
            if (other.name == "PLAYER")
            {
                visitingPlayer = other.GetComponent<PlayerController>();
                visitingPlayer.AddGravitySource(this);

                if (anchorable)
                {
                    visitingPlayer.AttemptAnchor(GetInstanceID());
                }
            }
        }

        if (type == "planet")
        {
            if (other.name == "PLAYER")
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.OnPlanetCollision(player.transform.position - transform.position, knockbackStrength);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (type == "gravityField")
        {
            if (other.name == "PLAYER")
            {
                visitingPlayer = other.GetComponent<PlayerController>();
                visitingPlayer.RemoveGravitySource(this);
                visitingPlayer = null;
            }
        }
    }
}
