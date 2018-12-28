using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTrimmerData", menuName = "PuzzleData/Trimmer")]
public class PuzzleTrimmerData : ScriptableObject, IPuzzleData {

    public GameObject prefab;
    public List<PossibleSetup> Setups = new List<PossibleSetup>();

    public GameObject GetIPuzzleGO()
    {
        return prefab;
    }

    [System.Serializable]
    public class PossibleSetup
    {
      
     
    }

}
