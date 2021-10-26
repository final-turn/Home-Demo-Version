using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlanetCollider : MonoBehaviour
{
    public float baseSize = 64;
    [SerializeField] private float knockbackStrength = 40;
    [SerializeField] private bool anchorable = true;

    [HideInInspector] public float planetSize;

    private void Awake()
    {
        if (name == "Gravity")
        {
            name = transform.parent.name;
            if(!anchorable)
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "PLAYER" && anchorable)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.AttemptAnchor(GetInstanceID());
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "PLAYER")
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.OnPlanetCollision(player.transform.position - transform.position, knockbackStrength);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "PLAYER" && anchorable)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.CancelAnchor();
        }
    }
}