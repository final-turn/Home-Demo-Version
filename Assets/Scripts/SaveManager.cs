using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private PlayerController player;

    [DllImport("__Internal")]
    private static extern void SetLocalData(string saveData);

    [DllImport("__Internal")]
    private static extern string GetLocalData();

    public static bool continueGame;
    public static Vector3 startPosition;

    private void Start()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void Save()
    {
        if(player.anchoredPlanetID == -1)
        {
            return;
        }
        AudioManager.Instance.ClickSoundEffect();
#if UNITY_WEBGL && !UNITY_EDITOR
        string saveDataString = player.transform.position.ToString();
        SetLocalData(saveDataString);
#endif
    }

    public void Load()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string saveData = GetLocalData();
        if (!string.IsNullOrEmpty(saveData))
        {
            startPosition = StringToVector3(saveData);
        }
#endif
    }

    public static Vector3 StringToVector3(string sVector)
    {
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }
        else
        {
            return Vector3.zero;
        }

        string[] sArray = sVector.Split(',');

        return new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));
    }
}
