using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelectableBehaviour), typeof(PuzzleGraphic))]
public class PuzzleAutopilot : MonoBehaviour, IPuzzle, ISelectable
{
    SelectableBehaviour selectable;
    PuzzleGraphic graphicCtrl;

    PuzzleAutopilotData data;
    public AutopilotIO Interactables;
    //Indice relativo alla soluzione scelta in data.Fase#
    //I due indici dell'array sono le due possibili fasi.
    int[] solutionCombinantion = new int[2];

    int _currentSolIndex;
    int currentSolutionIndex
    {
        get { return _currentSolIndex; }
        set
        {
            _currentSolIndex = value;
            OnCurrentSolutionIndexUpdate();
        }
    }
    bool isFase1Solved;
    bool isFase2Solved;
    List<InputValue> combToCompare {
        get
        {
            if (!isFase1Solved)
                return data.Fase1[solutionCombinantion[0]].Solution;
            else if (!isFase2Solved)
                return data.Fase2[solutionCombinantion[1]].Solution;
            else
            {
                Debug.LogError("Qualcuno sta cercando di accedere a questa lista e qualcosa non va!");
                return new List<InputValue>();
            }
        }
    }

    #region IPuzzle
    PuzzleState _solutionState = PuzzleState.Unsolved;
    public PuzzleState SolutionState {
        get {
            return _solutionState;
        }

        set
        {
            _solutionState = value;
        }
    }

    public void Setup(IPuzzleData _data)
    {
        selectable = GetComponent<SelectableBehaviour>();
        graphicCtrl = GetComponent<PuzzleGraphic>();

        data = _data as PuzzleAutopilotData;
        GenerateInitialValues();
    }

    public void DoWin()
    {
        selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleSolved(this);

        graphicCtrl.Paint(_solutionState);
    }

    public void DoLoose()
    {
        selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleBreakdown(this);

        graphicCtrl.Paint(_solutionState);
    }

    public bool CheckIfSolved()
    {
        if (currentSolutionIndex >= combToCompare.Count)
        {
            if (!isFase1Solved)
            {
                isFase1Solved = true;
                currentSolutionIndex = 0;
                Interactables.MonitorFase2.ToggleOnOff();
                return false;
            }
            else if (!isFase2Solved)
            {
                isFase2Solved = true;
                Interactables.MonitorFaseOK.ToggleOnOff();
                return true;
            }
            else
                return true;
        }
        else
            return false;
    }

    public void OnButtonSelect(SelectableButton _button)
    {
        InputValue value = (_button.InputData as PuzzleAutopilotInputData).Actualvalue;
        selectable.Select();
        CompareInputWithSolution(value);
    }
    public void OnSwitchSelect(SelectableSwitch _switch)
    {
        selectable.Select();
        InputValue value = (_switch.InputData as PuzzleAutopilotInputData).Actualvalue;
        switch (value)
        {
            case InputValue.LevaSx_Sx:
                value = InputValue.LevaSx_Dx;
                break;
            case InputValue.LevaSx_Dx:
                value = InputValue.LevaSx_Sx;
                break;
            case InputValue.LevaDx_Sx:
                value = InputValue.LevaDx_Dx;
                break;
            case InputValue.LevaDx_Dx:
                value = InputValue.LevaDx_Sx;
                break;
            default:
                Debug.LogError("Valore inconsistente per uno switch!!");
                break;
        }
        (_switch.InputData as PuzzleAutopilotInputData).Actualvalue = value;
        CompareInputWithSolution(value);
    }
    public void OnMonitorSelect(SelectableMonitor _monitor) { }
    public void OnUpdateSelectable(IPuzzleInput _input) { }
    #endregion

    #region Selectable Behaviours
    public void Init()
    {
        SolutionState = PuzzleState.Unsolved;
        graphicCtrl.Init(graphicCtrl.Data);

        //Inizializza gli elementi di Input
        InitSwitches();
        InitButtons();
        InitOutputMonitors();

        //Condizioni iniziali di partita
        currentSolutionIndex = 0;
        isFase1Solved = false;
        isFase2Solved = false;
    }

    public void OnSelection()
    {
        Debugger.DebugLogger.Clean();
        Debugger.DebugLogger.LogText("------------//" + gameObject.name + "//-----------");
        string fase1Sol = "Fase1Sol: ";
        foreach (InputValue iVal in data.Fase1[solutionCombinantion[0]].Solution)
        {
            fase1Sol += iVal.ToString() + ",";
        }
        Debugger.DebugLogger.LogText(fase1Sol);
        string fase2Sol = "Fase2Sol: ";
        foreach (InputValue iVal in data.Fase2[solutionCombinantion[1]].Solution)
        {
            fase2Sol += iVal.ToString() + ",";
        }
        Debugger.DebugLogger.LogText(fase2Sol);
    }

    public void OnStateChange(SelectionState _newState)
    {

    }
    #endregion

    void InitSwitches()
    {
        InputValue randDx = (InputValue)Random.Range(-2, 0);
        InputValue randSx = (InputValue)Random.Range(-4, -2);

        Interactables.LevaDx.Init(this, new PuzzleAutopilotInputData() { Actualvalue = randDx });
        Interactables.LevaDx.selectStatus = (int)randDx % 2 != 0 ? true : false;

        Interactables.LevaSx.Init(this, new PuzzleAutopilotInputData() { Actualvalue = randSx });
        Interactables.LevaSx.selectStatus = (int)randSx % 2 != 0 ? true : false;

        OnCurrentSolutionIndexUpdate();
    }

    void InitButtons() {

        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0:
                    Interactables.ButtonA.Init(this, new PuzzleAutopilotInputData() { Actualvalue = InputValue.BottoneA });
                    break;
                case 1:
                    Interactables.ButtonB.Init(this, new PuzzleAutopilotInputData() { Actualvalue = InputValue.BottoneB });
                    break;
                case 2:
                    Interactables.ButtonF.Init(this, new PuzzleAutopilotInputData() { Actualvalue = InputValue.BottoneF });
                    break;
                case 3:
                    Interactables.ButtonG.Init(this, new PuzzleAutopilotInputData() { Actualvalue = InputValue.BottoneG });
                    break;
                case 4:
                    Interactables.ButtonK.Init(this, new PuzzleAutopilotInputData() { Actualvalue = InputValue.BottoneK });
                    break;
                case 5:
                    Interactables.ButtonL.Init(this, new PuzzleAutopilotInputData() { Actualvalue = InputValue.BottoneL });
                    break;
                default:
                    break;
            }
        }
    }

    void InitOutputMonitors()
    {
        OutputValue output = data.Fase1[solutionCombinantion[0]].MonitorOutput;
        Interactables.MonitorFase1.SetMaterial((int)output);

        OutputValue output2 = data.Fase2[solutionCombinantion[1]].MonitorOutput;
        Interactables.MonitorFase2.SetMaterial((int)output2);

        Interactables.MonitorFase2.ToggleOnOff(false);
        Interactables.MonitorFaseOK.ToggleOnOff(false);
    }

    //genera a caso una combinazione iniziale del puzzle (e la soluzione)
    void GenerateInitialValues() {
        int fase1index = Random.Range(0, data.Fase1.Count);
        solutionCombinantion[0] = fase1index;

        int fase2index = Random.Range(0, data.Fase2.Count);
        solutionCombinantion[1] = fase2index;
    }
    bool CompareInputWithSolution(InputValue _input)
    {
        if(combToCompare[currentSolutionIndex] == _input)
        {
            currentSolutionIndex++;
            return true;
        }
        else
        {
            currentSolutionIndex = 0;
            DoLoose();
            return false;
        }
    }

    void OnCurrentSolutionIndexUpdate()
    {
        //Check if fase is solved
        if (CheckIfSolved())
        {
            DoWin();
            return;
        }

        //Compare eventual already done lever status
        InputValue currentInput = combToCompare[currentSolutionIndex];
        InputValue leverValue;
        if ((int)currentInput < -2)
        {
            leverValue = (Interactables.LevaSx.InputData as PuzzleAutopilotInputData).Actualvalue;
            if(leverValue == currentInput)
            {
                currentSolutionIndex++;
                return;
            }
        }
        else if(currentInput < 0)
        {
            leverValue = (Interactables.LevaDx.InputData as PuzzleAutopilotInputData).Actualvalue;
            if (leverValue == currentInput)
            {
                currentSolutionIndex++;
                return;
            }
        }
    }


    [System.Serializable]
    public struct AutopilotIO
    {
        public SelectableButton ButtonA;
        public SelectableButton ButtonB;
        public SelectableButton ButtonF;
        public SelectableButton ButtonG;
        public SelectableButton ButtonK;
        public SelectableButton ButtonL;

        public PuzzleAutopilotOutputMonitor MonitorFase1;
        public PuzzleAutopilotOutputMonitor MonitorFase2;
        public PuzzleAutopilotOutputMonitor MonitorFaseOK;

        public SelectableSwitch LevaSx;
        public SelectableSwitch LevaDx;
    }

    public enum InputValue{
        LevaSx_Sx = -4,
        LevaSx_Dx = -3,
        LevaDx_Sx = -2,
        LevaDx_Dx = -1,
        BottoneA = 0,
        BottoneB,
        BottoneF,
        BottoneG,
        BottoneK,
        BottoneL
    }
    public enum OutputValue
    {
        C,D,E,H,I,J
    }
}

