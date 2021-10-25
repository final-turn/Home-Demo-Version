using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Vector3 mouseDirection;
    [SerializeField] private GameObject playerSprite;
    [SerializeField] private GameObject anchorProgressRoot;
    [SerializeField] private Image anchorProgressImage;
    [Header("Swag")]
    [SerializeField] private float forceScale;
    [SerializeField] private float gravityScale;
    [SerializeField] private float anchorSpeed;
    [SerializeField] private float anchorTime;
    [Header("Sound Effects")]
    [SerializeField] private AudioClip anchorSFX;
    [SerializeField] private AudioClip launchSFX;
    [SerializeField] private AudioClip crashSFX;

    private Rigidbody2D m_Rigidbody;
    private Vector2 mousePosition;
    private float pullPower;
    private float fuel;
    private bool isDown;
    private bool inGame = true;
    private float anchorFactor;
    private bool awayFromCenter;
    [HideInInspector] public int anchoredPlanet = -1;
    private float anchorDuration = 0;

    private Dictionary<int, PlanetCollider> gravitySourceMap;

    private void Start()
    {
        gravitySourceMap = new Dictionary<int, PlanetCollider>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        RenderableState.Initialize();
        fuel = 1.0f;
    }

    public void PointerDown(Vector2 downPosition)
    {
        isDown = true;
        StartCoroutine(WhilePressDown(downPosition));
    }

    public void PointerUp()
    {
        isDown = false;
        if (pullPower > 0)
        {
            ComputeMoveVector();
        }
    }

    public void OnPosition(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
    }

    private void ComputeMoveVector()
    {
        if (fuel > 0)
        {
            m_Rigidbody.AddForce(playerSprite.transform.up * fuel * pullPower * forceScale, ForceMode2D.Impulse);
            fuel *= 1 - pullPower;
            pullPower = 0;
            AudioManager.Instance.PlaySFX(launchSFX, 0.5f);
        }

        if (anchoredPlanet != -1)
        {
            anchorDuration = 0f;
            StartCoroutine(AnchorCountdown(anchoredPlanet));
            anchoredPlanet = -1;
        }

        RenderableState.remainingFuel = fuel;
        RenderableState.consumePower = 0;
    }

    private IEnumerator WhilePressDown(Vector2 initialPosition)
    {
        while (isDown && inGame)
        {
            mouseDirection = initialPosition - mousePosition;
            pullPower = (Mathf.Clamp(mouseDirection.magnitude, 50f, 400f) - 50f) / 350f;

            if (pullPower > 0)
            {
                float factor = mouseDirection.x < 0f ? 1f : -1f;
                Quaternion rotation = Quaternion.Euler(0, 0, factor * Vector3.Angle(mouseDirection.normalized, Vector3.up));
                playerSprite.transform.rotation = rotation;
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

        if (anchoredPlanet == source.GetInstanceID())
        {
            anchoredPlanet = -1;
        }
    }


    private void FixedUpdate()
    {
        if (transform.position != Vector3.zero)
        {
            if (anchoredPlanet != -1)
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
        transform.RotateAround(planet.transform.position, Vector3.back, anchorFactor * anchorSpeed * Time.fixedDeltaTime);
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

    public void ApproachStartingPoint()
    {
        awayFromCenter = false;
        ResetForces();
    }

    public void LeaveStartingPoint()
    {
        awayFromCenter = true;
    }

    public void DockShip()
    {
        AudioManager.Instance.PlaySFX(anchorSFX, 0.5f);
        transform.position = Vector3.zero;
        StartCoroutine(RefuelShip());
        ResetForces();
    }

    public void AttemptAnchor(int anchorPlanet)
    {
        StartCoroutine(AnchorCountdown(anchorPlanet));
    }

    public void OnPlanetCollision(Vector3 collisionVector, float knockbackStrength)
    {
        AudioManager.Instance.PlaySFX(crashSFX, 0.5f);
        m_Rigidbody.AddForce(collisionVector.normalized * knockbackStrength, ForceMode2D.Impulse);
        anchorDuration = 0f;
        if (anchoredPlanet != -1)
        {
            StartCoroutine(AnchorCountdown(anchoredPlanet));
            anchoredPlanet = -1;
        }
    }

    public void AnchorShip(int instanceID)
    {
        if (anchoredPlanet == -1)
        {
            anchoredPlanet = instanceID;
        }
        PlanetCollider planet = gravitySourceMap[anchoredPlanet];
        Vector2 direction = (planet.transform.position - transform.position).normalized;
        Vector2 perp = Vector2.Perpendicular(direction);
        anchorFactor = Vector3.Dot(perp, m_Rigidbody.velocity.normalized) > 0 ? 1f : -1f;

        UIManager.Instance.SetActivePlanet(planet.name, planet.gameObject);

        AudioManager.Instance.PlaySFX(anchorSFX, 0.5f);
        StartCoroutine(RefuelShip());
        ResetForces();
    }

    private IEnumerator RefuelShip()
    {
        while (fuel < 1f && (anchoredPlanet != -1 || transform.position == Vector3.zero))
        {
            yield return new WaitForSeconds(0.025f);
            fuel = Mathf.Min(fuel + 0.025f, 1.0f);
            RenderableState.remainingFuel = fuel;
        }
    }

    private IEnumerator AnchorCountdown(int targetPlanet)
    {
        anchorDuration = 0f;
        anchorProgressRoot.SetActive(true);

        while (gravitySourceMap.ContainsKey(targetPlanet) && anchorDuration < anchorTime)
        {
            anchorDuration += 0.025f;
            anchorProgressImage.fillAmount = anchorDuration;

            yield return new WaitForSeconds(0.025f);
        }

        if (gravitySourceMap.ContainsKey(targetPlanet) && anchorDuration >= 1f)
        {
            AnchorShip(targetPlanet);
        }
        anchorProgressRoot.SetActive(false);
    }

    public void OnRestart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void OnPause()
    {
        UIManager.Instance.pauseMenu.SetActive(!UIManager.Instance.pauseMenu.activeSelf);
    }

    public void EndGame(Vector3 home)
    {
        m_Rigidbody.bodyType = RigidbodyType2D.Static;
        GetComponent<PlayerInput>().actions.Disable();
        inGame = false;
    }
}