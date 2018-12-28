using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPressureData", menuName = "PuzzleData/Pressure")]
public class PuzzlePressureData : ScriptableObject, IPuzzleData
{
    public GameObject Prefab;

    public int MaxMisstakes = 3;
    [Range(0,100)]
    public float FillingPerStrike = 1;
    public MonitorValues Monitor = new MonitorValues();
    public List<Setup> Setups = new List<Setup>();

    public GameObject GetIPuzzleGO()
    {
        return Prefab;
    }

    [System.Serializable]
    public class MonitorValues
    {
        public float MainImgTime = 5;
        public float AnimN2Time = 2;
        public float InteractTime = 3;
    }

    [System.Serializable]
    public struct Setup
    {
        public Material ImgToDispaly;
        public PuzzlePressure.ButtonType ButtonToPress;
    }
}
