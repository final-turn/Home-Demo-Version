using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectAnchoredUI : MonoBehaviour
{
    public Text label;
    public GameObject targetObject;
    [SerializeField] private Vector3 offset = Vector3.zero;

    private RectTransform myRectTransform;

    private void Start()
    {
        myRectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        myRectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(targetObject.transform.position) + offset;
    }
}
