using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Vector3 directionVector;
    [HideInInspector] public float pullPower;
    [HideInInspector] public bool isDown;

    [SerializeField] private float forceScale;
    [SerializeField] private float gravityScale;

    private Rigidbody2D m_Rigidbody;
    private Vector2 mousePosition;

    private Dictionary<int, GravityCollider> gravitySourceMap;

    private void Start()
    {
        gravitySourceMap = new Dictionary<int, GravityCollider>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnPress(InputValue value)
    {
        if (value.isPressed)
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
        m_Rigidbody.AddForce(transform.up * pullPower * forceScale, ForceMode2D.Impulse);
    }

    private IEnumerator WhilePressDown(Vector2 initialPosition)
    {
        while (isDown)
        {
            directionVector = initialPosition - mousePosition;
            pullPower = (Mathf.Clamp(directionVector.magnitude, 50f, 400f) - 50f) / 350f;
            if (pullPower > 0)
            {
                float factor = directionVector.x < 0f ? 1f : -1f;
                Quaternion rotation = Quaternion.Euler(0, 0, factor * Vector3.Angle(directionVector.normalized, Vector3.up));
                transform.SetPositionAndRotation(transform.position, rotation);
            }
            yield return new WaitForSeconds(0);
        }
    }

    public void AddGravitySource(GravityCollider source)
    {
        gravitySourceMap.Add(source.GetInstanceID(), source);
    }

    public void RemoveGravitySource(GravityCollider source)
    {
        gravitySourceMap.Remove(source.GetInstanceID());
    }


    private void FixedUpdate()
    {
        if (transform.position != Vector3.zero)
        {
            m_Rigidbody.AddForce((Vector3.zero - transform.position).normalized * gravityScale, ForceMode2D.Force);

            Vector2 displacement = Vector3.zero;

            foreach (GravityCollider collider in gravitySourceMap.Values)
            {
                m_Rigidbody.AddForce((collider.transform.position - transform.position).normalized * collider.gravityStrength);
            }

            transform.position = transform.position + new Vector3(displacement.x, displacement.y, 0);
        }

    }
}