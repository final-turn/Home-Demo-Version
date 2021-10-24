using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image consumeGauge;
    [SerializeField] private Image fuelGauge;
    [SerializeField] private TMP_Text fuelPercentage;

    // Update is called once per frame
    void Update()
    {
        consumeGauge.rectTransform.localScale = new Vector3(RenderableState.consumePower, 1, 1);
        fuelGauge.rectTransform.localScale = new Vector3(RenderableState.remainingFuel, 1, 1);
        fuelPercentage.text = "" + Mathf.FloorToInt(RenderableState.remainingFuel * 100f) + "%";
    }
}
