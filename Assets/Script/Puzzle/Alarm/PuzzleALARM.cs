using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SelectableBehaviour), typeof(PuzzleGraphic))]
public class PuzzleALARM : MonoBehaviour, IPuzzle, ISelectable
{
    SelectableBehaviour selectable;
    PuzzleGraphic graphicCtrl;

    PuzzleALARM_Data data;
    PuzzleALARM_Data.PossibileSetup chosenSetup;

    public float LightBlinkInterval = .5f;
    public List<PuzzleALARM_Light> Lights = new List<PuzzleALARM_Light>();

    [Header("GroupA")]
    public SelectableButton[] ButtonsA = new SelectableButton[5];
    [Header("GroupB")]
    public SelectableButton[] ButtonsB = new SelectableButton[5];
    [Header("GroupC")]
    public SelectableButton[] ButtonsC = new SelectableButton[5];
    [Header("GroupD")]
    public SelectableButton[] ButtonsD = new SelectableButton[5];
    [Header("GroupE")]
    public SelectableButton[] ButtonsE = new SelectableButton[5];
    [Header("GroupF")]
    public SelectableButton[] ButtonsF = new SelectableButton[5];
    [Header("GroupG")]
    public SelectableButton[] ButtonsG = new SelectableButton[5];
    [Header("GroupH")]
    public SelectableButton[] ButtonsH = new SelectableButton[5];

    List<SelectableButton> allButtons = new List<SelectableButton>();
    public bool IsAlarmActive { get; private set; }
    int solutionIndex { get;
        set; }

    #region IPuzzle
    PuzzleState _solutionState;
    public PuzzleState SolutionState
    {
        get
        {
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

        //Choosing a Setup between the possibilities
        data = _data as PuzzleALARM_Data;

        allButtons.Clear();
        allButtons.AddRange(ButtonsA);
        allButtons.AddRange(ButtonsB);
        allButtons.AddRange(ButtonsC);
        allButtons.AddRange(ButtonsD);
        allButtons.AddRange(ButtonsE);
        allButtons.AddRange(ButtonsF);
        allButtons.AddRange(ButtonsG);
        allButtons.AddRange(ButtonsH);
    }

    public void Init()
    {
        SolutionState = PuzzleState.Unsolved;

        int _setupIndex = Random.Range(0, data.Setups.Count);
        chosenSetup = data.Setups[_setupIndex];

        solutionIndex = 0;

        //Initializing lights
        foreach (PuzzleALARM_Light light in Lights)
        {
            light.Init(chosenSetup.LightPattern);
        }

        //Initializing buttons
        //Care: it works 'cause buttons are manually ordered
        for (int i = 0; i < allButtons.Count; i++)
        {
            allButtons[i].Init(this, new PuzzleALARM_inputData() { value = (InputValue)i });
        }
    }
    public void DoWin()
    {
        SolutionState = PuzzleState.Solved;
        IsAlarmActive = false;
        selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleSolved(this);

        foreach (PuzzleALARM_Light light in Lights)
            light.TurnOff();

        graphicCtrl.Paint(SolutionState);
    }
    public void DoLoose()
    {
        throw new System.NotImplementedException();
    }

    public void OnButtonSelect(SelectableButton _button)
    {
        if (!IsAlarmActive)
        {
            selectable.Select();
            return;
        }

        switch (solutionIndex)
        {
            case 0:
                if (chosenSetup.Seq1_First == (_button.InputData as PuzzleALARM_inputData).value)
                    solutionIndex++;
                break;
            case 1:
                if (chosenSetup.Seq1_Second == (_button.InputData as PuzzleALARM_inputData).value)
                    solutionIndex++;
                break;
            case 2:
                if (chosenSetup.Seq2_First == (_button.InputData as PuzzleALARM_inputData).value)
                    solutionIndex++;
                break;
            case 3:
                if (chosenSetup.Seq2_Second == (_button.InputData as PuzzleALARM_inputData).value)
                    solutionIndex++;
                break;
            case 4:
                if (chosenSetup.Button_Last == (_button.InputData as PuzzleALARM_inputData).value)
                    solutionIndex++;
                CheckIfSolved();
                break;
            default:
                CheckIfSolved();
                break;
        }

        if(SolutionState != PuzzleState.Solved)
            selectable.Select();
    }
    public void OnSwitchSelect(SelectableSwitch _switch)
    {
        throw new System.NotImplementedException();
    }
    public void OnMonitorSelect(SelectableMonitor _monitor)
    {
        throw new System.NotImplementedException();
    }
    public void OnUpdateSelectable(IPuzzleInput _input)
    {
        throw new System.NotImplementedException();
    }

    public bool CheckIfSolved()
    {
        if (solutionIndex >= 5)
        {
            DoWin();
            return true;
        }
        else
            return false;        
    }
    #endregion

    #region ISelectable
    public void OnSelection()
    {
        Debugger.DebugLogger.Clean();
        Debugger.DebugLogger.LogText(chosenSetup.Seq1_First.ToString() + " " +
            chosenSetup.Seq1_Second.ToString() + " " +
            chosenSetup.Seq2_First.ToString() + " " +
            chosenSetup.Seq2_Second.ToString() + " " +
            chosenSetup.Button_Last.ToString());
    }
    public void OnStateChange(SelectionState _state) { }
    #endregion

    float currentLBInterval = 0;
    private void Update()
    {
        currentLBInterval += Time.deltaTime;
        if (currentLBInterval >= LightBlinkInterval)
        {
            if (IsAlarmActive)
            {
                foreach (PuzzleALARM_Light light in Lights)
                    light.Pulse();

                GameManager.I_GM.AudioManager.PlaySound(AudioType.Alarm);
            }


            currentLBInterval = 0;
        }
    }

    public void Toggle(bool On = true)
    {
        //if ((On && IsAlarmActive) || (!On && !IsAlarmActive))
        //    return;

        if (!(On ^ IsAlarmActive))
            return;

        if (On)
        {
            IsAlarmActive = true;
            Init();
        }
        else
        {
            IsAlarmActive = false;
            solutionIndex = 0;
            foreach (PuzzleALARM_Light light in Lights)
                light.TurnOff();
        }
    }

    public enum InputValue
    {
        A_Button, A1, A2, A3, A4,
        B_Button, B1, B2, B3, B4,
        C_Button, C1, C2, C3, C4,
        D_Button, D1, D2, D3, D4,
        E_Button, E1, E2, E3, E4,
        F_Button, F1, F2, F3, F4,
        G_Button, G1, G2, G3, G4,
        H_Button, H1, H2, H3, H4
    }

    public enum LightsValue
    {
        OFF,
        RED,
        YELLOW
    }

    public class PuzzleALARM_inputData : IPuzzleInputData
    {
        public InputValue value;
    }
}                        
                         