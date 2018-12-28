using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSettingData", menuName = "LevelSetting/NewSetup")]
public class LevelSettings : ScriptableObject
{
    [Tooltip("Ovvero secondi prima di schiantarsi...")]
    public float StartingAltitude = 720;
    [Range(1,8)]
    public int TotalPuzzles = 3;
    [Range(1,8)][Tooltip("Attenzione: se maggiore di TotalPuzzles, sarà impossibile vincere!")]
    public int PuzzlesNeededToWin = 2;
    [Range(1, 8)]
    [Tooltip("Attenzione: se maggiore di TotalPuzzles, sarà impossibile perdere!")]
    public int PuzzlesNeededToloose = 3;

    public PuzzleALARM_Data GetAlarmData()
    {
        return Instantiate(Alarm_Data);
    }

    public List<ScriptableObject> GetPuzzleDatas()
    {
        List<ScriptableObject> dataInstances = new List<ScriptableObject>();

        foreach (ScriptableObject data in PuzzleDatas)
        {
            dataInstances.Add(Instantiate(data));
        }

        return dataInstances;
    }

    //public T GetPuzzleData<T>()
    //{
    //    foreach (IPuzzleData data in PuzzleDatas)
    //    {
    //        if (data.GetType() == typeof(T))
    //            return Instantiate<T>(data);
    //    }
 
    //    return default(T);
    //}

    public PuzzleALARM_Data Alarm_Data;
    public List<ScriptableObject> PuzzleDatas = new List<ScriptableObject>();
    public List<GameObject> FillingObjects = new List<GameObject>();
}
