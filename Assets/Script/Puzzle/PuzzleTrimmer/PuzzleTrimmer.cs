using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelectableBehaviour), typeof(PuzzleGraphic))]
public class PuzzleTrimmer : MonoBehaviour, IPuzzle,ISelectable {

    SelectableBehaviour _selectable;
    PuzzleTrimmerData _data;
    PuzzleGraphic _graphicCtrl;
    int _solutionProgression;
    PuzzleLockCodeData.PossibleSetup _chosenSetup;

    public PuzzleState SolutionState
    {
        get; set;
    }

    public bool CheckIfSolved()
    {
        throw new System.NotImplementedException();
    }

    public void DoLoose()
    {
        SolutionState = PuzzleState.Broken;
        _selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleBreakdown(this);

        _graphicCtrl.Paint(SolutionState);
    }

    public void DoWin()
    {
        SolutionState = PuzzleState.Solved;
        _selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleSolved(this);

        _graphicCtrl.Paint(SolutionState);
    }

    public void Init()
    {
        SolutionState = PuzzleState.Unsolved;
        _graphicCtrl.Paint(SolutionState);
        _solutionProgression = 0;
    }

    public void OnButtonSelect(SelectableButton _button)
    {
        throw new System.NotImplementedException();
    }

    public void OnMonitorSelect(SelectableMonitor _monitor)
    {
        throw new System.NotImplementedException();
    }

    public void OnSelection()
    {
        
    }

    public void OnStateChange(SelectionState _state)
    {

    }

    public void OnSwitchSelect(SelectableSwitch _switch)
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdateSelectable(IPuzzleInput _input)
    {
        throw new System.NotImplementedException();
    }

    public void Setup(IPuzzleData data)
    {
        _selectable = GetComponent<SelectableBehaviour>();
         _graphicCtrl = GetComponent<PuzzleGraphic>();

        data = _data as PuzzleTrimmerData;



    }

   
}
