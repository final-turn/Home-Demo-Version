using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSliderInit : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        bgmSlider.value = AudioManager.Instance.MusicSource.volume;
        sfxSlider.value = AudioManager.Instance.SFXSource.volume;
    }
}
