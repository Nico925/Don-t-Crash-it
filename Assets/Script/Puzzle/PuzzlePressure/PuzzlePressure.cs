using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelectableBehaviour), typeof(PuzzleGraphic))]
public class PuzzlePressure : MonoBehaviour, IPuzzle, ISelectable
{
    SelectableBehaviour selectable;
    PuzzleGraphic graphicCtrl;

    PuzzlePressureData data;
    public Pressure_IO Interactables;

    PuzzlePressureData.Setup currentSetup;
    float currentSolutionAmount = 0;
    int currentMisstakes = 0;

    public PuzzleState SolutionState { get; set; }

    public bool CheckIfSolved()
    {
        throw new System.NotImplementedException();
    }

    public void DoLoose()
    {
        Interactables.OutputMonitor.Toggle(false);

        SolutionState = PuzzleState.Broken;
        selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleBreakdown(this);

        graphicCtrl.Paint(SolutionState);
    }

    public void DoWin()
    {
        Interactables.OutputMonitor.Toggle(false);

        SolutionState = PuzzleState.Solved;
        selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleSolved(this);

        graphicCtrl.Paint(SolutionState);
    }

    public void Init()
    {
        SolutionState = PuzzleState.Unsolved;

        graphicCtrl.Init(graphicCtrl.Data);
        graphicCtrl.Paint(SolutionState);

        currentSolutionAmount = 0;
        currentMisstakes = 0;
        Interactables.ErrorText.text = currentMisstakes.ToString();
        Interactables.Slider.SetFillAmount(currentSolutionAmount);
    }

    public void OnButtonSelect(SelectableButton _button)
    {
        if (SolutionState != PuzzleState.Unsolved)
        {
            selectable.Select();
            return;
        }

        if (!Interactables.OutputMonitor.isInteractionTime)
        {
            ApplyError();
        }
        else
        {
            if ((_button.InputData as ButtonData).Type != currentSetup.ButtonToPress)
            {
                ApplyError();
            }
            else
            {
                ApplySucces();
            }
        }

        if(SolutionState == PuzzleState.Unsolved)
            selectable.Select();
    }

    public void OnMonitorSelect(SelectableMonitor _monitor)
    {
        if (SolutionState != PuzzleState.Unsolved)
            return;
    }

    public void OnSelection() { }

    public void OnStateChange(SelectionState _state)
    {
        switch (_state)
        {
            case SelectionState.Neutral:
                if (SolutionState == PuzzleState.Unsolved)
                    Interactables.OutputMonitor.Toggle(false);
                break;
            case SelectionState.Highlighted:
                break;
            case SelectionState.Selected:
                if(SolutionState == PuzzleState.Unsolved)
                    Interactables.OutputMonitor.Toggle(true);
                break;
            case SelectionState.Passive:
                break;
            default:
                break;
        }
    }

    public void OnSwitchSelect(SelectableSwitch _switch)
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdateSelectable(IPuzzleInput _input)
    {
        throw new System.NotImplementedException();
    }

    public void Setup(IPuzzleData _data)
    {
        selectable = GetComponent<SelectableBehaviour>();
        graphicCtrl = GetComponent<PuzzleGraphic>();

        //Choosing setups between the possibilities
        data = _data as PuzzlePressureData;

        //Setup Interactables
        Interactables.OutputMonitor.Setup(this, data.Monitor);
        Interactables.OutputMonitor.Toggle(false);

        Interactables.RedButton.Init(this, new ButtonData() { Type = ButtonType.Red });
        Interactables.BlueButton.Init(this, new ButtonData() { Type = ButtonType.Blue });
        Interactables.GreenButton.Init(this, new ButtonData() { Type = ButtonType.Green });

        Init();
    }

    public void InitOutputMonitor()
    {
        int _setupIndex = Random.Range(0, data.Setups.Count);
        currentSetup = data.Setups[_setupIndex];

        Interactables.OutputMonitor.ImageToDisplay = currentSetup.ImgToDispaly;

        Debugger.DebugLogger.Clean();
        Debugger.DebugLogger.LogText(currentSetup.ButtonToPress.ToString());
    }

    void ApplyError()
    {
        currentMisstakes++;

        Interactables.ErrorText.text = currentMisstakes.ToString();

        if (currentMisstakes >= data.MaxMisstakes)
        {
            currentMisstakes = data.MaxMisstakes;
            Interactables.ErrorText.text = currentMisstakes.ToString();
            DoLoose();
        }
    }

    void ApplySucces()
    {
        currentSolutionAmount += data.FillingPerStrike;
        if (currentSolutionAmount > 100)
            currentSolutionAmount = 100;

        Interactables.Slider.SetFillAmount(currentSolutionAmount, false);

        if (currentSolutionAmount == 100)
            DoWin();
    }

    [System.Serializable]
    public class Pressure_IO
    {
        public SelectableButton RedButton;
        public SelectableButton BlueButton;
        public SelectableButton GreenButton;
        public SliderController Slider;
        public TextMesh ErrorText;
        public PuzzlePressureOutputMonitor OutputMonitor;
    }

    public enum ButtonType
    {
        Red = 0,
        Blue = 1,
        Green = 2
    }

    public class ButtonData : IPuzzleInputData
    {
        public ButtonType Type;
    }
}
