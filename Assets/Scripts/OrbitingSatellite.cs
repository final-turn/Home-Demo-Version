using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingSatellite : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float orbitSpeed;


    private void FixedUpdate()
    {
        transform.RotateAround(targetObject.transform.position, Vector3.back, orbitSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.identity;
    }
}
