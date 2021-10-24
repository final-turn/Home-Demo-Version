using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayBackgroundSong("title");
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopBackgroundSong();
    }

    public void GoToGameScene()
    {
        SceneManager.LoadScene("Game");
    }
}
