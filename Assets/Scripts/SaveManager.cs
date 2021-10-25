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

    private void Start()
    {
        Instance = this;
        Load();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void Save()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Vector3 saveData = player.transform.position;
        saveData.z = player.anchoredPlanet;
        SetLocalData(player.transform.position.ToString());
#endif
    }

    public void Load()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string saveData = GetLocalData();
        if (!string.IsNullOrEmpty(saveData))
        {
            Vector3 data = StringToVector3(saveData);
            player.transform.position = new Vector3(data.x, data.y, 0);
            // player.AnchorShip(Mathf.FloorToInt(data.z));
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
