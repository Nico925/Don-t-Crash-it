using UnityEngine;

[RequireComponent(typeof(SelectableBehaviour), typeof(PuzzleGraphic))]
public class PuzzleGPS : MonoBehaviour, IPuzzle, ISelectable
{
    SelectableBehaviour selectable;
    PuzzleGraphic graphicCtrl;

    PuzzleGPSData data;
    public GPS_IO Interactables;

    PuzzleGPSData.PossibleCoordinate solutionCoordinates;
    float solutionOrientation;

    SelectableMonitor currentSelectedMonitor;
    
   
    #region IPuzzle
    PuzzleState _solutionState = PuzzleState.Unsolved;
    public PuzzleState SolutionState
    {
        get
        {
            return _solutionState;
        }

        set
        {
            _solutionState = value;
            OnSolutionStateChange(_solutionState);
        }
    }

    public void Setup(IPuzzleData _data)
    {
        data = _data as PuzzleGPSData;
        selectable = GetComponent<SelectableBehaviour>();

        graphicCtrl = GetComponent<PuzzleGraphic>();

        GenerateRandomCombination();
    }

    public bool CheckIfSolved()
    {
        int latitude = (Interactables.Latitude.InputData as PuzzleGPSMonitorData).coordinateValue;
        int longitude = (Interactables.Longitude.InputData as PuzzleGPSMonitorData).coordinateValue;

        if (solutionCoordinates.Coordinate.x == longitude && solutionCoordinates.Coordinate.y == latitude)
        {
            DoWin();
            return true;
        }
        else
        {
            DoLoose();
            return false;
        }
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

    public void OnButtonSelect(SelectableButton _button)
    {
        if(SolutionState != PuzzleState.Unsolved)
        {
            selectable.Select();
            return;
        }

        PuzzleGPSNumericData data = _button.InputData as PuzzleGPSNumericData;
        PuzzleGPSMonitorData coordinateData = currentSelectedMonitor.InputData as PuzzleGPSMonitorData;

        selectable.Select();
        if (data.ActualValue < 10)
        {
            if(coordinateData.coordinateValue < 10)
            {
                coordinateData.coordinateValue *= 10;
                coordinateData.coordinateValue += data.ActualValue;
                currentSelectedMonitor.TypeOn(coordinateData.coordinateValue.ToString());
            }
        }
        else if(data.ActualValue == 10)
        {
            coordinateData.coordinateValue /= 10;
            currentSelectedMonitor.TypeOn(coordinateData.coordinateValue.ToString());
        }
        else if(data.ActualValue == 11)
        {
            CheckIfSolved();
        }
    }
    public void OnSwitchSelect(SelectableSwitch _switch) { }
    public void OnMonitorSelect(SelectableMonitor _monitor) {
        if(_monitor == Interactables.Latitude)
        {
            currentSelectedMonitor = Interactables.Latitude;
        }
        else if (_monitor == Interactables.Longitude)
        {
            currentSelectedMonitor = Interactables.Longitude;
        }

        selectable.Select();
    }

    public void OnUpdateSelectable(IPuzzleInput _input)
    {
        //CurrentSelectedMonitor deve lampeggiare.
    }
    #endregion

    #region Selectable Behaviours
    public void Init()
    {
        SolutionState = PuzzleState.Unsolved;
        graphicCtrl.Init(graphicCtrl.Data);
        graphicCtrl.Paint(SolutionState);
        InitOutputMonitor();
        InitNumerics();
        InitSelectableMonitors();
    }

    public void OnStateChange(SelectionState _newState)
    {

    }

    public void OnSelection()
    {
        Debugger.DebugLogger.Clean();
        Debugger.DebugLogger.LogText("------------//" + gameObject.name + "//-----------");
        Debugger.DebugLogger.LogText(gameObject.name + gameObject.GetInstanceID());
        Debugger.DebugLogger.LogText(solutionCoordinates.Coordinate.ToString());
    }
    #endregion

    void OnSolutionStateChange(PuzzleState _solutionState)
    {
        graphicCtrl.Paint(_solutionState);
    }

    void InitNumerics()
    {
        for (int i = 0; i < Interactables.NumericalButtons.Length; i++)
        {
            Interactables.NumericalButtons[i].Init(this, new PuzzleGPSNumericData() { ActualValue = i });
        }
    }

    void InitSelectableMonitors()
    {
        Interactables.Latitude.Init(this, new PuzzleGPSMonitorData());
        Interactables.Longitude.Init(this, new PuzzleGPSMonitorData());

        Interactables.Latitude.TypeOn("");
        Interactables.Longitude.TypeOn("");

        currentSelectedMonitor = Interactables.Latitude;
    }

    void InitOutputMonitor()
    {
        Interactables.OutputMonitor.Init(data);
        Interactables.OutputMonitor.DisplayAndRotate(solutionCoordinates, solutionOrientation);
    }

    void GenerateRandomCombination()
    {
        int randCoordIndex = Random.Range(0, data.PossibleCoordinates.Count);
        solutionCoordinates = data.PossibleCoordinates[randCoordIndex];

        if (data.RandomRotation)
        {
            int randOrient = Random.Range(0, 4);
            solutionOrientation = randOrient * 90;
        }
        else
            solutionOrientation = 0;
    }

    [System.Serializable]
    public class GPS_IO
    {
        public SelectableButton[] NumericalButtons = new SelectableButton[12];
        public SelectableMonitor Latitude;
        public SelectableMonitor Longitude;
        public PuzzleGPSOutputMonitor OutputMonitor;
    }
}
