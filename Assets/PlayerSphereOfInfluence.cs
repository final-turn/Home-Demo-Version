using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSphereOfInfluence : MonoBehaviour
{
    public Dictionary<int, PlanetCollider>.ValueCollection Planets { get { return gravitySourceMap.Values; } }

    [SerializeField] private PlayerController player;
    [SerializeField] private float centerGravityFactor;
    [SerializeField] private float distanceOffset;

    private Dictionary<int, PlanetCollider> gravitySourceMap;

    private void Start()
    {
        gravitySourceMap = new Dictionary<int, PlanetCollider>(); ;
    }

    public Vector3 GetPlanetPosition(int planetID)
    {
        if (gravitySourceMap.ContainsKey(planetID))
        {
            return gravitySourceMap[planetID].transform.position;
        }
        return Vector3.zero;
    }

    public PlanetCollider GetPlanet(int planetID)
    {
        if (gravitySourceMap.ContainsKey(planetID))
        {
            return gravitySourceMap[planetID];
        }
        return null;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Gravity")
        {
            PlanetCollider gravity = other.GetComponent<PlanetCollider>();
            PlanetCollider planet = other.transform.parent.GetComponent<PlanetCollider>();
            gravity.baseSize = planet.baseSize;
            gravity.planetSize = planet.transform.localScale.x;

            // Debug.Log($"[{other.name}]({gravity.baseSize}, {gravity.planetSize})");
            gravitySourceMap.Add(gravity.GetInstanceID(), gravity);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Gravity")
        {
            PlanetCollider gravity = other.GetComponent<PlanetCollider>();
            PlanetCollider planet = other.transform.parent.GetComponent<PlanetCollider>();

            gravitySourceMap.Remove(gravity.GetInstanceID());

            if (player.anchoredPlanetID == gravity.GetInstanceID())
            {
                player.anchoredPlanetID = -1;
            }
        }
    }

    public Vector2 GetFreefallForce()
    {
        Vector3 force = (Vector3.zero - transform.position).normalized * centerGravityFactor;

        foreach (PlanetCollider collider in Planets)
        {
            float gravityStrength = (collider.baseSize / 64f) * collider.planetSize + distanceOffset;
            Vector3 sourceVector = collider.transform.position - transform.position;
            force += sourceVector.normalized * (gravityStrength / (sourceVector.magnitude * sourceVector.magnitude));
        }

        return force * Time.fixedDeltaTime * 25;
    }
}
