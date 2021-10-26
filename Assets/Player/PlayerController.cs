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
    [SerializeField] private PlayerSphereOfInfluence sphereOfInfluence;
    [SerializeField] private float forceScale;
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
    private float anchorStartTime;
    private bool isDown;
    private bool inGame = true;
    private float anchorFactor;
    [HideInInspector] public int anchoredPlanetID = -1;
    [HideInInspector] public bool instantAnchor = false;

    private int anchorTargetID = -1;
    private bool anchorCountdownRunning = false;

    private void Start()
    {
        fuel = 1.0f;
        RenderableState.Initialize();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        if (SaveManager.continueGame)
        {
            transform.position = SaveManager.startPosition;
            instantAnchor = true;
        }
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

    private void ComputeMoveVector()
    {
        if (fuel > 0)
        {
            m_Rigidbody.AddForce(playerSprite.transform.up * fuel * pullPower * forceScale, ForceMode2D.Impulse);
            fuel *= 1 - pullPower;
            pullPower = 0;
            AudioManager.Instance.PlaySFX(launchSFX, 0.5f);
        }

        // Debug.Log(anchorTargetID);
        if (anchorTargetID != -1)
        {
            AttemptAnchor(anchorTargetID);
        }

        RenderableState.remainingFuel = fuel;
        RenderableState.consumePower = 0;
    }

    private void FixedUpdate()
    {
        if (transform.position != Vector3.zero)
        {
            if (anchoredPlanetID != -1 && sphereOfInfluence.GetPlanetPosition(anchoredPlanetID) != Vector3.zero)
            {
                transform.RotateAround(
                    sphereOfInfluence.GetPlanetPosition(anchoredPlanetID),
                    Vector3.back,
                    anchorFactor * anchorSpeed * Time.fixedDeltaTime
                    );
            }
            else
            {
                m_Rigidbody.AddForce(sphereOfInfluence.GetFreefallForce(), ForceMode2D.Force);
            }
        }

    }

    public void ApproachStartingPoint()
    {
        ResetForces();
        m_Rigidbody.velocity = Vector3.zero - transform.position;
    }

    public void DockShip()
    {
        AudioManager.Instance.PlaySFX(anchorSFX, 0.5f);
        transform.position = Vector3.zero;
        StartCoroutine(RefuelShip());
        ResetForces();
    }

    public void OnPlanetCollision(Vector3 collisionVector, float knockbackStrength)
    {
        AudioManager.Instance.PlaySFX(crashSFX, 0.5f);
        m_Rigidbody.AddForce(collisionVector.normalized * knockbackStrength, ForceMode2D.Impulse);
        anchorStartTime = Time.time;
        if(anchorTargetID != -1)
        {
            AttemptAnchor(anchorTargetID);
        }
    }

    public void AttemptAnchor(int targetPlanetID)
    {
        anchoredPlanetID = -1;
        anchorStartTime = Time.time;
        anchorTargetID = targetPlanetID;


        if (instantAnchor)
        {
            AnchorShip(targetPlanetID);
            instantAnchor = false;
            return;
        }

        if (!anchorCountdownRunning)
        {
            StartCoroutine(AnchorCountdown(targetPlanetID));
        }
    }

    public void CancelAnchor()
    {
        anchorTargetID = -1;
    }

    private IEnumerator AnchorCountdown(int targetPlanetID)
    {
        anchorCountdownRunning = true;
        anchorProgressRoot.SetActive(true);

        while (anchorTargetID == targetPlanetID && Time.time - anchorStartTime < 1f)
        {
            anchorProgressImage.fillAmount = Time.time - anchorStartTime;
            yield return new WaitForSeconds(0.025f);
        }

        if (anchorTargetID != -1 && anchorTargetID == targetPlanetID && Time.time - anchorStartTime >= 1f)
        {
            AnchorShip(targetPlanetID);
        }

        anchorProgressRoot.SetActive(false);
        anchorCountdownRunning = false;
    }

    public void AnchorShip(int instanceID)
    {
        anchoredPlanetID = instanceID;

        PlanetCollider planet = sphereOfInfluence.GetPlanet(anchoredPlanetID);
        Vector2 direction = (planet.transform.position - transform.position).normalized;
        Vector2 perp = Vector2.Perpendicular(direction);
        anchorFactor = Vector3.Dot(perp, m_Rigidbody.velocity.normalized) > 0 ? 1f : -1f;
        UIManager.Instance.SetActivePlanet(planet.name, planet.gameObject);
        AudioManager.Instance.PlaySFX(anchorSFX, 0.5f);
        ResetForces();

        StartCoroutine(RefuelShip());
    }

    private IEnumerator RefuelShip()
    {
        while (fuel < 1f && (anchoredPlanetID != -1 || transform.position == Vector3.zero))
        {
            yield return new WaitForSeconds(0.025f);
            fuel = Mathf.Min(fuel + 0.025f, 1.0f);
            RenderableState.remainingFuel = fuel;
        }
    }

    public void ResetForces()
    {
        // Debug.Log("Reset");
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.angularVelocity = 0;
    }

    public void OnRestart()
    {
        SaveManager.continueGame = false;
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