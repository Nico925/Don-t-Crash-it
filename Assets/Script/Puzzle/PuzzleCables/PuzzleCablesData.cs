using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCablesData", menuName = "PuzzleData/Cables")]
public class PuzzleCablesData : ScriptableObject, IPuzzleData
{
    public GameObject Prefab;
    public float LightInterval = 1f;
    public List<Setup> Setups = new List<Setup>();

    public GameObject GetIPuzzleGO()
    {
        return Prefab;
    }

    [System.Serializable]
    public class Setup
    {
        [Header("Start Condition")]
        public PuzzleCables.ButtonType StartButton;
        [Tooltip("Sono i cavi che partono collegati al bottone centrale")]
        public List<PuzzleCables.CableType> ConnectedCables = new List<PuzzleCables.CableType>();
        [Tooltip("Sono i cavi che partono NON collegati al bottone centrale")]
        public List<PuzzleCables.CableType> DetachedCables = new List<PuzzleCables.CableType>();

        [Header("Solution")]
        public List<PuzzleCables.CableType> Sol_ConnectedCabs = new List<PuzzleCables.CableType>();
        public List<PuzzleCables.CableType> Sol_DetachedCables = new List<PuzzleCables.CableType>();
    }
}
