using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float forceScale;

    private Rigidbody2D m_Rigidbody;
    private Vector2 mousePosition;

    public Vector3 directionVector;
    public float pullPower;
    public bool isDown;


    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnPress(InputValue value)
    {
        if(value.isPressed)
        {
            isDown = true;
            StartCoroutine(WhilePressDown(Input.mousePosition));
        }
        else
        {
            isDown = false;
            ComputeMoveVector();
        }
    }

    public void OnPosition(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
    }

    private void ComputeMoveVector()
    {
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.angularVelocity = 0;
        m_Rigidbody.AddForce(transform.up * pullPower * forceScale);
    }

    private IEnumerator WhilePressDown(Vector2 initialPosition)
    {
        while (isDown)
        {
            directionVector = initialPosition - mousePosition;
            pullPower = (Mathf.Clamp(directionVector.magnitude, 50f, 400f) - 50f) / 350f;
            if(pullPower > 0)
            {
                float factor = directionVector.x < 0f ? 1f : -1f;
                Quaternion rotation = Quaternion.Euler(0, 0, factor * Vector3.Angle(directionVector.normalized, Vector3.up));
                transform.SetPositionAndRotation(transform.position, rotation);
            }
            yield return new WaitForSeconds(0);
        }
    }
}