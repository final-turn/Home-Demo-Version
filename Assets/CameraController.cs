using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerController targetObject;
    [SerializeField] private float forceScale;

    // Update is called once per frame
    void Update()
    {
        Vector3 followVector = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, -10f);
        transform.position = followVector;
    }
}
