using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAlarmData", menuName = "PuzzleData/Alarm")]
public class PuzzleALARM_Data : ScriptableObject, IPuzzleData
{
    public GameObject GetIPuzzleGO()
    {
        return null;
    }

    public List<PossibileSetup> Setups = new List<PossibileSetup>();

    [System.Serializable]
    public class PossibileSetup
    {
        public List<PuzzleALARM.LightsValue> LightPattern = new List<PuzzleALARM.LightsValue>();

        [Header("Sequence 1")]
        public PuzzleALARM.InputValue Seq1_First;
        public PuzzleALARM.InputValue Seq1_Second;

        [Header("Sequence 2")]
        public PuzzleALARM.InputValue Seq2_First;
        public PuzzleALARM.InputValue Seq2_Second;
        public PuzzleALARM.InputValue Button_Last;
    }
}
