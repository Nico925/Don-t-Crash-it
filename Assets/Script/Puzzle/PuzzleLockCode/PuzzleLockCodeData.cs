using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLockCodeData", menuName = "PuzzleData/LockCode")]
public class PuzzleLockCodeData : ScriptableObject, IPuzzleData
{
    public GameObject Prefab;
    public List<PossibleSetup> Setups = new List<PossibleSetup>();

    public GameObject GetIPuzzleGO()
    {
        return Prefab;
    }

    [System.Serializable]
    public class PossibleSetup
    {
        public Texture ScreenImage;
        public PuzzleLockCode.KeyOrder Seq_1st;
        public PuzzleLockCode.KeyOrder Seq_2nd;
        public PuzzleLockCode.KeyOrder Seq_3rd;
    }
}
