using UnityEngine;

[CreateAssetMenu(fileName = "NewTurbineData", menuName = "PuzzleData/Turbine")]
public class PuzzleTurbineData : ScriptableObject, IPuzzleData
{
    public GameObject Prefab;
    public TurbineButtonData[] ButtonsValues = new TurbineButtonData[8];

    public GameObject GetIPuzzleGO()
    {
        return Prefab;
    }
}

[System.Serializable]
public class TurbineButtonData : IPuzzleInputData
{
    public string Label;
    public int[] EModifiers { get { return new int[] { E1Modifier, E2Modifier, E3Modifier, E4Modifier }; } }
    public int E1Modifier;
    public int E2Modifier;
    public int E3Modifier;
    public int E4Modifier;
}