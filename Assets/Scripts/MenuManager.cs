using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject creditsMenu;
    [Header("Save")]
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject window;

    private GameObject activeMenu;

    void Start()
    {
        AudioManager.Instance.PlayBackgroundSong("title");
        mainMenu.SetActive(true);
        activeMenu = mainMenu;
        SaveManager.Instance.Load();

        if(SaveManager.startPosition != Vector3.zero)
        {
            continueButton.SetActive(true);
            window.GetComponent<RectTransform>().sizeDelta = new Vector2(window.GetComponent<RectTransform>().sizeDelta.x, 310);
        }
    }

    private void OnDestroy()
    {
        if(AudioManager.Instance)
        {
            AudioManager.Instance.StopBackgroundSong();
        }
    }

    public void OpenMainMenu ()
    {
        AudioManager.Instance.ClickSoundEffect();
        activeMenu.SetActive(false);
        mainMenu.SetActive(true);
        activeMenu = mainMenu;
    }

    public void OpenOptions()
    {
        AudioManager.Instance.ClickSoundEffect();
        activeMenu.SetActive(false);
        optionsMenu.SetActive(true);
        activeMenu = optionsMenu;
    }

    public void OpenCredits()
    {
        AudioManager.Instance.ClickSoundEffect();
        activeMenu.SetActive(false);
        creditsMenu.SetActive(true);
        activeMenu = creditsMenu;
    }

    public void Continue()
    {
        AudioManager.Instance.ClickSoundEffect();
        SaveManager.continueGame = true;
        SceneManager.LoadScene("Game");
    }

    public void GoToGameScene()
    {
        AudioManager.Instance.ClickSoundEffect();
        SaveManager.continueGame = false;
        SceneManager.LoadScene("Game");
    }

    public void SetBackgroundMusicVolume(float value)
    {
        AudioManager.Instance.SetBackgroundMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}
