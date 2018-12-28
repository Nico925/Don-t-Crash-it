using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SelectableBehaviour), typeof(PuzzleGraphic))]
public class PuzzleCables : MonoBehaviour, IPuzzle, ISelectable
{
    [Header("Cable colours")]
    public Material RedCab_Mat;
    public Material YellowCab_Mat;
    public Material GreenCab_Mat;
    public Material BlueCab_Mat;
    public Material WhiteCab_Mat;
    public Material BrownCab_Mat;

    [Header("Light colours")]
    public Material LightOff_Mat;
    Material[] lightOff_MatArr;
    public Material LightOn_Mat;
    Material[] lightOn_MatArr;
    [Header("Components")]
    public PuzzleComponents Components;

    List<SelectableSwitch> switches = new List<SelectableSwitch>();

    int currentLightIndex;
    int currentLightOnAmount = 1;

    float currentLightInterval;

    SelectableBehaviour selectable;
    PuzzleGraphic graphicCtrl;

    PuzzleCablesData data;
    PuzzleCablesData.Setup chosenSetup;

    public PuzzleState SolutionState { get; set; }

    private void Update()
    {
        if (!data)
            return;

        if (SolutionState != PuzzleState.Unsolved)
            return;

        currentLightInterval += Time.deltaTime;
        if(currentLightInterval >= data.LightInterval)
        {
            currentLightInterval = 0;
            UpdateLights();
        }
    }

    public bool CheckIfSolved()
    {
        CabSwitchData data;
        List<CableType> connectedCabs = new List<CableType>(chosenSetup.Sol_ConnectedCabs);
        List<CableType> detachedCabs = new List<CableType>(chosenSetup.Sol_DetachedCables);
        foreach (SelectableSwitch switchCab in switches)
        {
            data = switchCab.InputData as CabSwitchData;

            if(switchCab.selectStatus == true)
            {
                //Check right connection
                if (!connectedCabs.Remove(data.CabType))
                {
                    return false;
                }
            }
            else
            {
                //Check right detachment
                if (!detachedCabs.Remove(data.CabType))
                {
                    return false;
                }
            }
        }

        if (connectedCabs.Count + detachedCabs.Count != 0)
            return false;
        else
            return true;
    }

    public void DoLoose()
    {
        if (SolutionState == PuzzleState.Broken)
            return;

        SolutionState = PuzzleState.Broken;
        selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleBreakdown(this);
        TurnLightsOff();
        graphicCtrl.Paint(SolutionState);
    }

    public void DoWin()
    {
        SolutionState = PuzzleState.Solved;
        selectable.GetRoot().GetComponent<LevelManager>().NotifyPuzzleSolved(this);
        TurnLightsOff();
        graphicCtrl.Paint(SolutionState);
    }

    public void OnButtonSelect(SelectableButton _button)
    {
        if (CheckIfSolved())
        {
            DoWin();
        }
        else
        {
            DoLoose();
        }
    }

    public void OnSelection()
    {
        Debugger.DebugLogger.Clean();
        string toConnectCables = "";
        string toDetachCables = "";

        foreach (var item in chosenSetup.Sol_ConnectedCabs)
        {
            toConnectCables += item.ToString() + " / ";
        }
        foreach (var item in chosenSetup.Sol_DetachedCables)
        {
            toDetachCables += item.ToString() + " / ";
        }

        Debugger.DebugLogger.LogText("Connect: " + toConnectCables);
        Debugger.DebugLogger.LogText("Disconnect: " + toDetachCables);
    }

    public void OnStateChange(SelectionState _state)
    {
    }

    public void OnSwitchSelect(SelectableSwitch _switch)
    {
        CabSwitchData data = _switch.InputData as CabSwitchData;

        //Check for light electrocution
        int lightToCheck = currentLightOnAmount;
        for (int i = currentLightIndex; i < Components.Lights.Count; i++)
        {
            lightToCheck--;
            if (i == data.CabID)
            {
                DoLoose();
                break;
            }

            if (lightToCheck == 0)
                break;
        }
        if(lightToCheck > 0)
            for (int i = 0; i < currentLightIndex; i++)
            {
                lightToCheck--;
                if (i == data.CabID)
                {
                    DoLoose();
                    break;
                }

                if (lightToCheck == 0)
                    break;
            }

        currentLightOnAmount++;
        if (currentLightOnAmount == Components.Lights.Count)
            DoLoose();
        else
        {
            currentLightInterval = 0;
            UpdateLights();
        }

        if (SolutionState == PuzzleState.Unsolved)
            selectable.Select();
    }

    public void OnMonitorSelect(SelectableMonitor _monitor)
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

        data = _data as PuzzleCablesData;

        GameObject fakeCable2 = new GameObject("FakeCable2");
        fakeCable2.transform.parent = transform;
        Components.Cables.Insert(1, fakeCable2);

        GameObject fakeCable6 = new GameObject("FakeCable2");
        fakeCable6.transform.parent = transform;
        Components.Cables.Insert(5, fakeCable6);

        lightOff_MatArr = new Material[] { Components.Lights[0].materials[0], LightOff_Mat };
        lightOn_MatArr = new Material[] { Components.Lights[0].materials[0], LightOn_Mat };
    }

    public void Init()
    {
        //Choose a new setup in data
        int setupIndex = Random.Range(0, data.Setups.Count);
        chosenSetup = data.Setups[setupIndex];

        SolutionState = PuzzleState.Unsolved;
        graphicCtrl.Paint(SolutionState);

        switches.Clear();

        //Setup button
        Components.StartButton.Init(this, new CabButtonData() { BtnType = chosenSetup.StartButton });
        //Setup cables
        InitCables();
        //Setup lights
        InitLights();

        graphicCtrl.Init(graphicCtrl.Data);
    }

    void InitCables()
    {
        int toConnectCabs = chosenSetup.ConnectedCables.Count;
        int toDetachCabs = chosenSetup.DetachedCables.Count;
        int noCab = Components.Cables.Count -2/*Per i cavi virtuali dei posti 1 e 5*/ - toConnectCabs - toDetachCabs;

        List<int> alreadySetCabs = new List<int>() { 1, 5 };
        while (toConnectCabs + toDetachCabs + noCab > 0)
        {
            //choose an avaiable cable
            int cabIndex = Random.Range(0, Components.Cables.Count);
            GameObject cab = Components.Cables[cabIndex];
            if (alreadySetCabs.Contains(cabIndex))
            {
                if (alreadySetCabs.Count >= Components.Cables.Count)
                {
                    Debug.LogError("Impossible Setup");
                    return;
                }
                continue;
            }
            else
                alreadySetCabs.Add(cabIndex);

            if (toConnectCabs > 0)
            {
                cab.SetActive(true);

                toConnectCabs--;
                CableType cabColor = chosenSetup.ConnectedCables[toConnectCabs];
                MeshRenderer cabRenderer = cab.GetComponentInChildren<MeshRenderer>();
                cabRenderer.material = GetMaterialByType(cabColor);

                SelectableSwitch cabSwitch = cab.GetComponentInChildren<SelectableSwitch>();
                cabSwitch.Init(this, new CabSwitchData() { CabType = cabColor, CabID = cabIndex });
                cabSwitch.selectStatus = true;
                switches.Add(cabSwitch);
            }
            else if (toDetachCabs > 0)
            {
                cab.SetActive(true);

                toDetachCabs--;
                CableType cabColor = chosenSetup.DetachedCables[toDetachCabs];
                MeshRenderer cabRenderer = cab.GetComponentInChildren<MeshRenderer>();
                cabRenderer.material = GetMaterialByType(cabColor);

                SelectableSwitch cabSwitch = cab.GetComponentInChildren<SelectableSwitch>();
                cabSwitch.Init(this, new CabSwitchData() { CabType = cabColor, CabID = cabIndex});
                cabSwitch.selectStatus = false;
                switches.Add(cabSwitch);
            }
            else
            {
                noCab--;
                cab.SetActive(false);
            }
        }
    }

    void InitLights()
    {
        currentLightIndex = Random.Range(0, Components.Lights.Count);
        currentLightOnAmount = 1;
    }
 
    void UpdateLights()
    {
        currentLightIndex++;
        if(currentLightIndex >= Components.Lights.Count)
            currentLightIndex = 0;

        int toLightOn = currentLightOnAmount;
        TurnLightsOff();

        for (int i = currentLightIndex; i < Components.Lights.Count; i++)
        {
            if (toLightOn <= 0)
                break;

            toLightOn--;
            Components.Lights[i].materials = lightOn_MatArr;
        }

        if(toLightOn > 0)
            for (int i = 0; i < currentLightIndex; i++)
            {
                if (toLightOn <= 0)
                    break;

                toLightOn--;
                Components.Lights[i].materials = lightOn_MatArr;
            }
    }

    void TurnLightsOff()
    {
        for (int i = 0; i < Components.Lights.Count; i++)
        {
            Components.Lights[i].materials = lightOff_MatArr;
        }
    }

    Material GetMaterialByType(CableType _type)
    {
        Material toReturn = null;
        switch (_type)
        {
            case CableType.Red1:
                toReturn = new Material(RedCab_Mat);
                break;
            case CableType.Yellow2:
                toReturn = new Material(YellowCab_Mat);
                break;
            case CableType.Green3:
                toReturn = new Material(GreenCab_Mat);
                break;
            case CableType.Blue4:
                toReturn = new Material(BlueCab_Mat);
                break;
            case CableType.White5:
                toReturn = new Material(WhiteCab_Mat);
                break;
            case CableType.Brown6:
                toReturn = new Material(BrownCab_Mat);
                break;
            default:
                break;
        }

        return toReturn;
    }

    public enum CableType
    {
        Red1,
        Yellow2,
        Green3,
        Blue4,
        White5,
        Brown6
    }

    public enum ButtonType
    {
        Red, Blue, Green
    }

    [System.Serializable]
    public class PuzzleComponents
    {
        public SelectableButton StartButton;
        public List<GameObject> Cables = new List<GameObject>();
        public List<MeshRenderer> Lights = new List<MeshRenderer>();
    }

    class CabSwitchData: IPuzzleInputData
    {
        public CableType CabType;
        public int CabID;
    }

    class CabButtonData: IPuzzleInputData
    {
        public ButtonType BtnType;
    }
}
