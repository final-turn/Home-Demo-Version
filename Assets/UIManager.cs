using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image consumeGauge;
    [SerializeField] private Image fuelGauge;

    // Update is called once per frame
    void Update()
    {
        consumeGauge.rectTransform.localScale = new Vector3(1, RenderableState.consumePower, 1);
        fuelGauge.rectTransform.localScale = new Vector3(1, RenderableState.remainingFuel, 1);
    }
}
