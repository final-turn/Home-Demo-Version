using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalPlanet : MonoBehaviour
{
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private GameObject gravityfield;
    [SerializeField] private GameObject userInterface;
    private PlayerController player;

    private float timeElapsed = 0;

    bool inEnding = false;
    Vector3 initialPosition;
    bool boolSwitchSceneFired = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "PLAYER")
        {
            player = other.GetComponent<PlayerController>();

            player.EndGame(transform.position);

            AudioManager.Instance.PlayBackgroundSong("end");
            gravityfield.SetActive(false);
            userInterface.SetActive(false);
            inEnding = true;
            initialPosition = player.transform.position;
        }
    }

    private void Update()
    {
        if(inEnding)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed < 42f)
            {
                player.transform.position = Vector3.Lerp(initialPosition, transform.position, timeElapsed / 58f);
            }
            else
            {
                if(!boolSwitchSceneFired)
                {
                    boolSwitchSceneFired = true;
                    blackScreen.SetActive(true);
                    StartCoroutine(SwitchScene());
                }
            }
        }
    }

    private IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Ending");
    }
}
