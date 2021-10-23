using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCollider : MonoBehaviour
{
    public string type;
    public bool anchorable = false;
    public float gravityStrength = 0;
    public float anchorTime = 1;
    public float knockbackStrength = 0;

    private PlayerController visitingPlayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (type == "gravityField")
        {
            if (other.name == "PLAYER")
            {
                visitingPlayer = other.GetComponent<PlayerController>();
                visitingPlayer.AddGravitySource(this);

                if(anchorable)
                {
                    StartCoroutine(CheckAnchorStatus());
                }
            }
        }

        if (type == "planet")
        {
            if (other.name == "PLAYER")
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.PushShip((player.transform.position - transform.position).normalized * knockbackStrength);
            }
        }
    }

    private IEnumerator CheckAnchorStatus()
    {
        yield return new WaitForSeconds(anchorTime);

        if(visitingPlayer)
        {
            Debug.Log("Anchored!");
            visitingPlayer.ResetForces();
            visitingPlayer.anchoredPlanet = GetInstanceID();
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

                if(visitingPlayer.anchoredPlanet == GetInstanceID())
                {
                    visitingPlayer.anchoredPlanet = -1;
                }

                visitingPlayer = null;
            }
        }
    }
}
