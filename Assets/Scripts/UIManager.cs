using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Menus")]
    [SerializeField] private GameObject infoWindow;
    [SerializeField] private PlayerController player;
    [SerializeField] private Button save;
    public GameObject pauseMenu;
    [Header("Fuel Gauge Properties")]
    [SerializeField] private Image consumeGauge;
    [SerializeField] private Image fuelGauge;
    [SerializeField] private TMP_Text fuelPercentage;
    [SerializeField] private TMP_Text title;
    [Header("Game Elements")]
    [SerializeField] private GameObjectAnchoredUI planetUI;

    private void Start()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Update()
    {
        consumeGauge.rectTransform.localScale = new Vector3(RenderableState.consumePower, 1, 1);
        fuelGauge.rectTransform.localScale = new Vector3(RenderableState.remainingFuel, 1, 1);
        int remainingFuel = Mathf.FloorToInt(RenderableState.remainingFuel * 100f);
        fuelPercentage.text = "" + remainingFuel + "%";
        title.text = "Fuel Gauge" + (remainingFuel == 69 ? " (Nice.)" : "");
    }

    public void ToggleInfoWindow()
    {
        AudioManager.Instance.ClickSoundEffect();
        infoWindow.SetActive(!infoWindow.activeSelf);
    }

    public void SetActivePlanet(string name, GameObject targetPlanet)
    {
        if (string.IsNullOrEmpty(name))
        {
            planetUI.gameObject.SetActive(false);
        }
        else
        {
            planetUI.label.text = name;
            planetUI.targetObject = targetPlanet;
            planetUI.gameObject.SetActive(true);
        }
    }

    public void GoToTitleScreen()
    {
        AudioManager.Instance.ClickSoundEffect();
        SceneManager.LoadScene("TitleScreen");
    }

    public void TogglePause()
    {
        AudioManager.Instance.ClickSoundEffect();
        save.interactable = (player.anchoredPlanetID != -1);
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    public void ClickSoundEffect()
    {
        AudioManager.Instance.ClickSoundEffect();
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
