using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SelectableBehaviour))]
public class LevelManager : MonoBehaviour, ISelectable
{
    SelectableBehaviour selectable;

    CameraController camCtrl;

    public LevelSettings Setting;
    public PlaneKinematicController Plane;

    int puzzleNeededToLoose { get { return Setting.PuzzlesNeededToloose; } }
    int PuzzleNeededToWin { get { return Setting.PuzzlesNeededToWin; } }
    public Altimetro Altimetro;
    public Error_Panel ErrorPanel;
    public FallDisplay FallDisplay;
    public SelectableBehaviour AlarmPuzzle;
    PuzzleALARM_Data Alarm_Data; 
    public List<SelectableBehaviour> OtherSelectable = new List<SelectableBehaviour>();

    List<ScriptableObject> PuzzleDatas = new List<ScriptableObject>();
    List<IPuzzle> puzzles = new List<IPuzzle>();
    public List<Transform> PuzzlePositions = new List<Transform>();

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        Setting = GameManager.I_GM.ChosenSetting;
        PuzzleDatas = Setting.GetPuzzleDatas();
        Alarm_Data = Setting.GetAlarmData();
        
        selectable = GetComponent<SelectableBehaviour>();
        selectable.Init(null, SelectionState.Selected);

        if (AlarmPuzzle)
        {
            AlarmPuzzle.Init(selectable);
            AlarmPuzzle.GetComponent<PuzzleALARM>().Setup(Alarm_Data);
            AlarmPuzzle.GetComponent<PuzzleALARM>().Init();
        }

        if (Altimetro)
        {
            Altimetro.GetComponent<SelectableBehaviour>().Init(selectable);
            Altimetro.Init(this);
        }

        camCtrl = Camera.main.GetComponent<CameraController>();
        camCtrl.isMoveFreeCam = false;
        camCtrl.Init();

        foreach (var item in OtherSelectable)
        {
            item.Init(selectable);
        }
        //Sceglie i puzzle tra quelli possibili trai i dati ricevuti
        CreateNewPuzzleSet();

        Plane.StartFall(Setting.StartingAltitude);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            SelectableBehaviour selected = selectable.GetChildren().FirstOrDefault(s => s.State == SelectionState.Selected);
            if (selected != null)
                selectable.Select();
            else if (selectable.State != SelectionState.Selected)
                selectable.Select();

            camCtrl.isMoveFreeCam = false;
        }

        if(selectable.State != SelectionState.Passive && Input.GetMouseButton(1))
            camCtrl.isMoveFreeCam = true;
    }

    bool hasAlarmedOnce;
    bool hasAlarmedAltitude;
    public void ActivateAlarm(bool forceIt = false)
    {
        PuzzleALARM _alarm = AlarmPuzzle.GetComponent<PuzzleALARM>();

        if (forceIt)
            _alarm.Toggle();
        else
        {
            ActivateAlarm();
        }

        hasAlarmedOnce = true;
    }

    public void ActivateAlarm()
    {
        PuzzleALARM _alarm = AlarmPuzzle.GetComponent<PuzzleALARM>();
        if (hasAlarmedOnce)
        {
            int actualChance = Random.Range(0, 100);
            if (actualChance >= 50)
                _alarm.Toggle();
        }
        else
            _alarm.Toggle();

        hasAlarmedOnce = true;
    }

    public void AccelerateAltimeter()
    {
        Altimetro.Accelerate();
        Plane.UpdateFallTime(Altimetro.CurrentAltitude / Altimetro.DropSpeed);
    }

    public void DecelerateAltimeter(bool goPositive = false)
    {
        Altimetro.Decelerate(goPositive);
        Plane.UpdateFallTime(Altimetro.CurrentAltitude / Altimetro.DropSpeed);
    }

    public void NotifyAltitudeUpdate(float _maxAltitude, float _currentAltitude)
    {
        NotifyAltitudeUpdate(_currentAltitude / _maxAltitude);
    }

    public void NotifyAltitudeUpdate(float percentage)
    {
        //Crescita dello shake in relazione all'altitudine
        camCtrl.ShakeForce = camCtrl.ShakeMaxForce * (1- percentage);
        camCtrl.ShakeFrequence = camCtrl.ShakeMaxFrequence * percentage;

        FallDisplay.UpdateLine(percentage);

        if (percentage >= 0.5f)
            return;

        if(percentage <= 0)
        {
            selectable.State = SelectionState.Passive;
            GameLost();
        }

        if (hasAlarmedAltitude)
            return;
        else
        {
            if (!hasAlarmedOnce)
                ActivateAlarm(true);
            hasAlarmedAltitude = true;
        }
    }

    public void NotifyPuzzleSolved(IPuzzle puzzle)
    {
        //Parziale comportamento comunque da refactorizzare
        camCtrl.AwarnessLook(()=>
        {
            selectable.Select();
        });

        GameManager.I_GM.AudioManager.PlaySound(AudioType.PuzzleSolved);
        puzzle.SolutionState = PuzzleState.Solved;

        if(puzzle.GetType() == typeof(PuzzleALARM))
        {
            List<IPuzzle> brokenPuzzles = new List<IPuzzle>();
            foreach (IPuzzle puz in puzzles)
            {
                if (puz.SolutionState == PuzzleState.Broken)
                    brokenPuzzles.Add(puz);
            }

            if(brokenPuzzles.Count > 0)
            {
                brokenPuzzles.Shuffle();
                IPuzzle randPuzz = brokenPuzzles[Random.Range(0, brokenPuzzles.Count)];
                randPuzz.SolutionState = PuzzleState.Unsolved;
                randPuzz.Init();
                DecelerateAltimeter();
            }
            else
            {
                DecelerateAltimeter(true);
            }
        }

        UpdateOverallSolution();
    }

    public void NotifyPuzzleBreakdown(IPuzzle _puzzle)
    {
        //camCtrl.AwarnessLook(null);
        camCtrl.Shake(()=> { selectable.Select(); });
        //selectable.Select();
        _puzzle.SolutionState = PuzzleState.Broken;

        //Altimeter
        AccelerateAltimeter();
        //Alarm
        if (!hasAlarmedOnce)
        {
            ActivateAlarm(true);
        }
        else
        {
            int activationChance = Random.Range(0, 100);
            if (activationChance < 50)
                ActivateAlarm(true);
        }

        UpdateOverallSolution();
    }

    public void OnSelection()
    {
        camCtrl.isMoveFreeCam = false;
        camCtrl.FocusReset();
    }

    public void OnStateChange(SelectionState _newState) { }

    public void GameLost()
    {
        GameManager.I_GM.AudioManager.Clear();
        FindObjectOfType<FadeController>().FadeIn(()=> { SceneManager.LoadScene(2); });
    }

    public void GameWon()
    {
        GameManager.I_GM.AudioManager.Clear();
        FindObjectOfType<FadeController>().FadeIn(() => { SceneManager.LoadScene(3); });
    }

    /// <summary>
    /// Sceglie, posiziona e inizializza i puzzle
    /// </summary>
    void CreateNewPuzzleSet()
    {
        int randIndex;
        List<Transform> positionLeft = PuzzlePositions;
        List<IPuzzleData> _puzzleDatas = new List<IPuzzleData>();

        foreach (IPuzzleData item in PuzzleDatas)
        {
            //Creo il doppio delle copie di ogni puzzle data
            _puzzleDatas.Add(item);
            _puzzleDatas.Add(item);
        }

        for (int i = 0; i < Setting.TotalPuzzles; i++)
        {
            randIndex = Random.Range(0, _puzzleDatas.Count);
            IPuzzleData randData = _puzzleDatas[randIndex];

            int randPos = Random.Range(0, positionLeft.Count);
            Transform position = positionLeft[randPos];
            positionLeft.RemoveAt(randPos);

            IPuzzle randPuzzle = Instantiate(randData.GetIPuzzleGO(), position).GetComponent<IPuzzle>();
            randPuzzle.Setup(randData);
            randPuzzle.Init();
            puzzles.Add(randPuzzle);
            (randPuzzle as MonoBehaviour).GetComponent<SelectableBehaviour>().Init(selectable);
            _puzzleDatas.RemoveAt(randIndex);
        }

        foreach (Transform pos in positionLeft)
        {
            Instantiate(Setting.FillingObjects[Random.Range(0, Setting.FillingObjects.Count)], pos);
        }
    }

    void UpdateOverallSolution()
    {
        int currentSolvedPuzzles = 0;
        int currentBrokenPuzzles = 0;

        foreach (IPuzzle puzzle in puzzles)
        {
            if (puzzle.SolutionState == PuzzleState.Solved)
                currentSolvedPuzzles++;
            if (puzzle.SolutionState == PuzzleState.Broken)
                currentBrokenPuzzles++;
        }

        ErrorPanel.TurnLights(currentBrokenPuzzles);

        //Momentanea Soluzione di sconfitta
        if (currentBrokenPuzzles >= puzzleNeededToLoose)
        {
            GameLost();
        }

        //Momentanea Soluzione di vittoria
        if (currentSolvedPuzzles >= PuzzleNeededToWin)
        {
            GameWon();
        }
    }
}
