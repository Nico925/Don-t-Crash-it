using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAutopilotData", menuName = "PuzzleData/Autopilot")]
public class PuzzleAutopilotData : ScriptableObject, IPuzzleData {
    public GameObject Prefab;

    public List<PartialSolution> Fase1;
    public List<PartialSolution> Fase2;

    public GameObject GetIPuzzleGO()
    {
        return Prefab;
    }

    [System.Serializable]
    public class PartialSolution {
        public PuzzleAutopilot.OutputValue MonitorOutput;
        public List<PuzzleAutopilot.InputValue> Solution = new List<PuzzleAutopilot.InputValue>();
    }
}

public class PuzzleAutopilotInputData : IPuzzleInputData
{
    public PuzzleAutopilot.InputValue Actualvalue;
}