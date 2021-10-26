using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingScene : MonoBehaviour
{
    [SerializeField] private GameObject theEnd;
    [SerializeField] private Image blackScreen;
    [SerializeField] private Image thankYouMessage;

    void Start()
    {
        StartCoroutine(WereFinallyLanding());
    }

    private IEnumerator WereFinallyLanding()
    {
        var tempColor = Color.white;
        float duration = 0;
        float coroutineStep = 0.0125f;

        while (duration < 1)
        {
            yield return new WaitForSeconds(coroutineStep);
            tempColor = blackScreen.color;
            tempColor.a = 1f - duration;
            blackScreen.color = tempColor;
            duration += coroutineStep;
        }

        blackScreen.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);

        float startTime = Time.time;
        Vector3 startPosition = theEnd.transform.GetComponent<RectTransform>().localPosition;
        while (Time.time - startTime < 20)
        {
            yield return new WaitForSeconds(coroutineStep);
            theEnd.transform.GetComponent<RectTransform>().localPosition = Vector3.Lerp(startPosition, new Vector3(0, 1930f, 0), (Time.time - startTime)/20);
        }

        duration = 0;
        while (duration < 1)
        {
            yield return new WaitForSeconds(coroutineStep);
            tempColor = thankYouMessage.color;
            tempColor.a = duration;
            thankYouMessage.color = tempColor;
            duration += coroutineStep;
        }
        tempColor = thankYouMessage.color;
        tempColor.a = 1f;
        thankYouMessage.color = tempColor;
    }

    public void GoToTwitch ()
    {
        Application.OpenURL("https://www.twitch.tv/godthekid");
    }
}