using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Vector3 directionVector;
    public float pullPower;
    [HideInInspector] public bool isDown;
    [HideInInspector] public bool awayFromCenter;
    [HideInInspector] public int anchoredPlanet = -1;

    [Header("Swag")]
    [SerializeField] private float forceScale;
    [SerializeField] private float gravityScale;
    [SerializeField] private float anchorSpeed;

    private Rigidbody2D m_Rigidbody;
    private Vector2 mousePosition;

    private Dictionary<int, PlanetCollider> gravitySourceMap;

    private void Start()
    {
        gravitySourceMap = new Dictionary<int, PlanetCollider>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        RenderableState.Initialize();
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
        ResetForces();
        anchoredPlanet = -1;
        m_Rigidbody.AddForce(transform.up * pullPower * forceScale, ForceMode2D.Impulse);
        RenderableState.remainingFuel = 1 - pullPower;
        RenderableState.consumePower = 0;
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

            RenderableState.consumePower = pullPower;
            yield return new WaitForSeconds(0);
        }
    }

    public void AddGravitySource(PlanetCollider source)
    {
        gravitySourceMap.Add(source.GetInstanceID(), source);
    }

    public void RemoveGravitySource(PlanetCollider source)
    {
        gravitySourceMap.Remove(source.GetInstanceID());
    }


    private void FixedUpdate()
    {
        if (transform.position != Vector3.zero)
        {
            if(anchoredPlanet != -1)
            {
                AnchoredMovement();
            }
            else
            {
                FreeMovement();
            }
        }

    }

    private void AnchoredMovement()
    {
        PlanetCollider planet = gravitySourceMap[anchoredPlanet];
        transform.RotateAround(planet.transform.position, Vector3.back, anchorSpeed * Time.fixedDeltaTime);
    }

    private void FreeMovement()
    {
        Vector3 direction = Vector3.zero - transform.position;
        if (direction.magnitude > 1)
        {
            m_Rigidbody.AddForce(direction.normalized * gravityScale, ForceMode2D.Force);

            Vector2 displacement = Vector3.zero;

            foreach (PlanetCollider collider in gravitySourceMap.Values)
            {
                m_Rigidbody.AddForce((collider.transform.position - transform.position).normalized * collider.gravityStrength);
            }

            transform.position = transform.position + new Vector3(displacement.x, displacement.y, 0);
        }
        else if (!awayFromCenter && m_Rigidbody.velocity.magnitude < 3)
        {
            transform.position = Vector3.zero;
            m_Rigidbody.velocity = Vector3.zero;
        }
    }

    public void ResetForces()
    {
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.angularVelocity = 0;
    }

    public void PushShip(Vector3 direction)
    {
        m_Rigidbody.AddForce(direction, ForceMode2D.Impulse);
        anchoredPlanet = -1;
    }
}