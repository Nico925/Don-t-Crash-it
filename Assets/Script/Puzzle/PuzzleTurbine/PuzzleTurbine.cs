using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SelectableBehaviour), typeof(PuzzleGraphic))]
public class PuzzleTurbine : MonoBehaviour, IPuzzle, ISelectable
{
    #region Interactables controller
    [Header("Labled Buttons")]
    public List<SelectableButton> LabledButtons = new List<SelectableButton>(3);
    [Header("Reset Button")]
    public SelectableButton resetButton = new SelectableButton();
    #endregion

    SelectableBehaviour selectable;
    PuzzleGraphic graphicCtrl;

    PuzzleTurbineData data;
    PuzzleCombination combination;
    int buttonHits;
    List<SliderController> Sliders = new List<SliderController>();
  
    #region IPuzzle
    PuzzleState _solutionState = PuzzleState.Unsolved;
    public PuzzleState SolutionState {
        get { return _solutionState; }
        set {
            if (SolutionState == value)
                return;

            _solutionState = value;
            OnSolutionStateChange(SolutionState);
        }
    }

    public void Setup(IPuzzleData _data) {
        data = _data as PuzzleTurbineData;

        selectable = GetComponent<SelectableBehaviour>();

        graphicCtrl = GetComponent<PuzzleGraphic>();
        graphicCtrl.Init(graphicCtrl.Data);
    }

    public bool CheckIfSolved() {
        for (int i = 0; i < 4; i++) {
            if (combination.CurrentEValues[i] == 50)
                continue;

            DoLoose();
            return false;
        }

        DoWin();
        return true;
    }

    public void DoWin()
    {
        SolutionState = PuzzleState.Solved;
        selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleSolved(this);

        graphicCtrl.Paint(SolutionState);
    }

    public void DoLoose() {
        SolutionState = PuzzleState.Broken;
        selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleBreakdown(this);

        graphicCtrl.Paint(SolutionState);
    }

    public void OnButtonSelect(SelectableButton _button) {

        if (SolutionState != PuzzleState.Unsolved)
            return;

        // Labled Button;
        foreach (var button in LabledButtons) {
            if (button == _button) {
                buttonHits++;
                TurbineButtonData data = button.InputData as TurbineButtonData;
                SetEValues(data.E1Modifier, data.E2Modifier, data.E3Modifier, data.E4Modifier);
            }
        }

        // Reset Button
        if (_button == resetButton) {
            CheckIfSolved();
        }
        if(SolutionState == PuzzleState.Unsolved)
            selectable.Select();
    }
    public void OnSwitchSelect(SelectableSwitch _switch) { }
    public void OnMonitorSelect(SelectableMonitor _monitor) { }
    public void OnUpdateSelectable(IPuzzleInput _input) { }
    #endregion

    #region Selectable Behaviours
    public void Init() {
        SolutionState = PuzzleState.Unsolved;
        graphicCtrl.Paint(SolutionState);

        GenerateNewPuzzleCombination();
        buttonHits = 0;

        InitGenricalElement();
    }

    void OnSolutionStateChange(PuzzleState _solutionState) {
        graphicCtrl.Paint(_solutionState);
    }

    public void OnStateChange(SelectionState _newState)
    {
    }

    public void OnSelection()
    {
        Debugger.DebugLogger.Clean();
        Debugger.DebugLogger.LogText("------------//"+gameObject.name + "//-----------");
        Debugger.DebugLogger.LogText(combination.Solution[0].Label + "_" + combination.Solution[1].Label);
    }
    #endregion

    #region Setup and Init specific
    private void GenerateNewPuzzleCombination() {
        PuzzleCombination newComb = new PuzzleCombination();
        TurbineButtonData possibleButton;
        List<TurbineButtonData> usedButtons = new List<TurbineButtonData>();
        //Numero di pulsanti richiesti;
        for (int i = 0; i < LabledButtons.Count; i++) {
            if (i < 2)
                while (newComb.Solution.Count == i) {
                    possibleButton = GetUnchosenButton(usedButtons);
                    if (IsSolvable(newComb, possibleButton)) {
                        newComb.Solution.Add(possibleButton);
                        usedButtons.Add(possibleButton);
                        break;
                    }
                } else {
                possibleButton = GetUnchosenButton(usedButtons);
                usedButtons.Add(possibleButton);
                newComb.Fillers.Add(possibleButton);
            }
        }

        newComb.ResetEValues();
        combination = newComb;
    }

    TurbineButtonData GetUnchosenButton(List<TurbineButtonData> alreadyChosen) {
        List<TurbineButtonData> possibles = data.ButtonsValues.ToList();
        foreach (var item in alreadyChosen)
            possibles.Remove(item);

        int chosenIndex = Random.Range(0, possibles.Count);

        return possibles[chosenIndex];
    }

    bool IsSolvable(PuzzleCombination _combination, TurbineButtonData newButton) {
        int[] currentEs = _combination.InitialEValues;

        currentEs[0] -= newButton.E1Modifier;
        if (currentEs[0] < 0 || currentEs[0] > 100)
            return false;
        currentEs[1] -= newButton.E2Modifier;
        if (currentEs[1] < 0 || currentEs[1] > 100)
            return false;
        currentEs[2] -= newButton.E3Modifier;
        if (currentEs[2] < 0 || currentEs[2] > 100)
            return false;
        currentEs[3] -= newButton.E4Modifier;
        if (currentEs[3] < 0 || currentEs[3] > 100)
            return false;

        return true;
    }

    private void InitGenricalElement() {
        List<TurbineButtonData> buttonPool = new List<TurbineButtonData>();
        foreach (var item in combination.Solution)
            buttonPool.Add(item);
        foreach (var item in combination.Fillers)
            buttonPool.Add(item);
        buttonPool.Shuffle();

        /// Inject Labled Buttons
        for (int i = 0; i < LabledButtons.Count; i++) {
            LabledButtons[i].Init(this, buttonPool[i]);
            LabledButtons[i].SetAdditionalData(buttonPool[i].Label);
        }

        /// Inject Reset Button
        resetButton.Init(this, null);

        foreach (SliderController slider in GetComponentsInChildren<SliderController>()) {
            Sliders.Add(slider);
        }
        UpdateSliderValues();
    }
    #endregion

    public void SetEValues(int E1, int E2, int E3, int E4) {
        combination.CurrentEValues[0] += E1;
        combination.CurrentEValues[1] += E2;
        combination.CurrentEValues[2] += E3;
        combination.CurrentEValues[3] += E4;

        CheckBreackDown();
        for (int i = 0; i < combination.CurrentEValues.Length; i++) {
            if (combination.CurrentEValues[i] < 0)
                combination.CurrentEValues[i] = 0;
            if (combination.CurrentEValues[i] > 100)
                combination.CurrentEValues[i] = 100;
        }

        UpdateSliderValues();
    }

    void CheckBreackDown() {
        for (int i = 0; i < 4; i++) {
            if (combination.CurrentEValues[i] < 0 || combination.CurrentEValues[i] > 100)
            {
                DoLoose();
                return;
            }
        }
        if (buttonHits >= 3)
            DoLoose();
    }

    void UpdateSliderValues() {
        for (int i = 0; i < Sliders.Count; i++) {
            Sliders[i].SetFillAmount(combination.CurrentEValues[i]);
        }
    }

    /// <summary>
    /// Values setup of the puzzle.
    /// </summary>
    class PuzzleCombination {
        public int[] InitialEValues { get { return GetEs(); } }
        public int[] CurrentEValues { get; private set; }
        public List<TurbineButtonData> Solution = new List<TurbineButtonData>();
        public List<TurbineButtonData> Fillers = new List<TurbineButtonData>();
        //Assuming solution is correct
        int[] GetEs() {
            int[] eS = new int[] { 50, 50, 50, 50 };

            foreach (var sol in Solution) {
                eS[0] -= sol.E1Modifier;
                eS[1] -= sol.E2Modifier;
                eS[2] -= sol.E3Modifier;
                eS[3] -= sol.E4Modifier;
            }

            return eS;
        }

        public void ResetEValues() {
            CurrentEValues = InitialEValues;
        }
    }
}