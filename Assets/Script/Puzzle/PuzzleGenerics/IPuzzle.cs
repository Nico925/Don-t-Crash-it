using UnityEngine;

/// <summary>
/// Behaviour that all puzzles share
/// </summary>
public interface IPuzzle {

    PuzzleState SolutionState { get; set; }

    void Setup(IPuzzleData data);
    void Init();

    void DoWin();
    void DoLoose();
    bool CheckIfSolved();

    void OnButtonSelect(SelectableButton _button);
    void OnSwitchSelect(SelectableSwitch _switch);
    void OnMonitorSelect(SelectableMonitor _monitor);
    void OnUpdateSelectable(IPuzzleInput _input);
}

/// <summary>
/// Interface used to flag all the data needed by an IPuzzle
/// Necessary to impose a Data injection on Setup and avid generic behaviours
/// </summary>
public interface IPuzzleData {
    GameObject GetIPuzzleGO();
}

public enum PuzzleState
{
    Unsolved,
    Broken,
    Solved
}

