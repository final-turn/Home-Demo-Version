using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject cameraRoot;
    [SerializeField] private PlayerController targetObject;
    [SerializeField] private Image background;
    [SerializeField] private float scrollScale;
    [SerializeField] private float offetScale;

    private Vector3 diff = Vector3.zero;
    private Vector2 mouseOffset;
    private float smoothElapsed = 0;

    // Update is called once per frame
    void Update()
    {
        diff += targetObject.transform.position - cameraRoot.transform.position;
        Vector2 difference = new Vector3(diff.x, diff.y, 0);
        Vector3 followVector = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, -10f);
        cameraRoot.transform.position = followVector;
        Vector3 offset = targetObject.mouseDirection.normalized * RenderableState.consumePower * offetScale;

        Vector2 mouseScrollOffset = mouseOffset;
        if (offset.magnitude == 0 && mouseOffset.magnitude > 0)
        {
            smoothElapsed = Mathf.Min(1f, smoothElapsed + Time.deltaTime * 2);
            mouseScrollOffset = Vector2.Lerp(mouseOffset, Vector3.zero, smoothElapsed);
            transform.localPosition = mouseScrollOffset;
            if (smoothElapsed >= 1f)
            {
                mouseOffset = Vector3.zero;
                smoothElapsed = 0;
            }
        }
        else
        {
            smoothElapsed = 0;
            mouseOffset = new Vector3(offset.x, offset.y, 0);
            transform.localPosition = mouseOffset;
        }

        background.materialForRendering.SetTextureOffset("_MainTex", (difference + mouseScrollOffset) * scrollScale);
    }
}
