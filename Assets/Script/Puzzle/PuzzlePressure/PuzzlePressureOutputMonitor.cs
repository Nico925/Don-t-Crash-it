using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePressureOutputMonitor : MonoBehaviour
{
    public MeshRenderer Renderer;
    PuzzlePressure parentPuzzle;

    public Material Anim1;
    public Material Anim2;

    public Material CD1;
    public Material CD2;
    public Material CD3;
    [Range(0, 1)]
    public float CDDisplayTime;
    float CDHideTime { get { return 1 - CDDisplayTime; } }
    [HideInInspector]
    public float MainDisplayTime = 5;
    [HideInInspector]
    public float AnimTime = 2;
    [HideInInspector]
    public float CDFullDisplayTime = 3;

    [HideInInspector]
    public Material ImageToDisplay;

    float currentTimer = 0;
    bool isActive;
    public bool isInteractionTime
    {
        get
        {
            if (currentTimer > MainDisplayTime)
                return true;
            else
                return false;
        }
    }

    private State _currentState = State.Off;
    public State CurrentState
    {
        get { return _currentState; }
        private set
        {
            if (_currentState == value)
                return;

            _currentState = value;

            switch (_currentState)
            {
                case State.Off:
                    Renderer.enabled = false;
                    break;
                case State.MainImg:
                    Renderer.enabled = true;
                    Renderer.material = ImageToDisplay;
                    break;
                case State.Anim1Img:
                    Renderer.enabled = true;
                    Renderer.material = Anim1;
                    break;
                case State.Anim2Img:
                    Renderer.enabled = true;
                    Renderer.material = Anim2;
                    break;
                case State.CD1Img:
                    Renderer.enabled = true;
                    Renderer.material = CD3;
                    break;
                case State.CD2Img:
                    Renderer.enabled = true;
                    Renderer.material = CD2;
                    break;
                case State.CD3Img:
                    Renderer.enabled = true;
                    Renderer.material = CD1;
                    break;
                default:
                    break;
            }
        }
    }

    private void Start()
    {
        CurrentState = State.Off;
    }

    private void Update()
    {
        if (!isActive)
            return;

        currentTimer += Time.deltaTime;
        if (currentTimer > MainDisplayTime + AnimTime + CDFullDisplayTime)
        {
            parentPuzzle.InitOutputMonitor();
            currentTimer = 0;
        }
        else if (currentTimer > MainDisplayTime + AnimTime)
        {
            float partialTimer = currentTimer - MainDisplayTime - AnimTime;
            if (partialTimer >= 3 * CDDisplayTime + 2 * CDHideTime)
            {
                CurrentState = State.Off;
            }
            else if (partialTimer >= 2 * CDDisplayTime + 2 * CDHideTime)
            {
                CurrentState = State.CD3Img;
            }
            else if (partialTimer >= 2 * CDDisplayTime + CDHideTime)
            {
                CurrentState = State.Off;
            }
            else if (partialTimer >= CDDisplayTime + CDHideTime)
            {
                CurrentState = State.CD2Img;
            }
            else if (partialTimer >= CDDisplayTime)
            {
                CurrentState = State.Off;
            }
            else
            {
                CurrentState = State.CD1Img;
            }
        }
        else if (currentTimer > MainDisplayTime)
        {
            float partialTimer = currentTimer - MainDisplayTime;

            if (partialTimer > AnimTime * .75f)
            {
                CurrentState = State.Anim2Img;
            }
            else if (partialTimer > AnimTime * .5f)
            {
                CurrentState = State.Anim1Img;
            }
            else if (partialTimer > AnimTime * .25f)
            {
                CurrentState = State.Anim2Img;
            }
            else
            {
                CurrentState = State.Anim1Img;
            }
        }
        else
        {
            CurrentState = State.MainImg;
        }
    }

    public void Toggle(bool _active)
    {
        if (isActive == _active)
            return;

        isActive = _active;
        if (!isActive)
        {
            currentTimer = 0;
            CurrentState = State.Off;
        }
        else
        {
            parentPuzzle.InitOutputMonitor();
        }
    }

    public void Setup(PuzzlePressure _parent, PuzzlePressureData.MonitorValues _values)
    {
        parentPuzzle = _parent;
        MainDisplayTime = _values.MainImgTime;
        AnimTime = _values.AnimN2Time;
        CDFullDisplayTime = _values.InteractTime;
    }

    public enum State
    {
        Off,
        MainImg,
        Anim1Img,
        Anim2Img,
        CD1Img,
        CD2Img,
        CD3Img,
    }
}
