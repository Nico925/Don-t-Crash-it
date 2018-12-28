using UnityEngine;

[RequireComponent(typeof(SelectableBehaviour))]
public class SelectableMonitor : MonoBehaviour, IPuzzleInput
{
    SelectableBehaviour selectable;
    IPuzzle puzzleCtrl;
    TextMesh textMesh;

    public Animator AnimatorCtrl;
    public string SelectionAnimation;
    public string IdleAnimation;

    #region Data injection
    public IPuzzleInputData InputData;

    public void DataInjection(IPuzzleInputData _data)
    {
        InputData = _data;
    }
    #endregion

    public void Init(IPuzzle _parentPuzzle, IPuzzleInputData _data)
    {
        textMesh = GetComponentInChildren<TextMesh>();
        if (!textMesh)
            Debug.LogWarning("This component needs a TextMesh in order to work properly!");

        //Initial data injection
        InputData = _data;

        AnimatorCtrl = GetComponent<Animator>();

        //setup parent relationship
        puzzleCtrl = _parentPuzzle;

        //selectable behaviour setup
        selectable = GetComponent<SelectableBehaviour>();
        selectable.Init((puzzleCtrl as MonoBehaviour).GetComponent<SelectableBehaviour>());
    }

    void Update ()
    {
        if (puzzleCtrl != null)
            puzzleCtrl.OnUpdateSelectable(this);
	}

    public void OnSelection()
    {
        GameManager.I_GM.AudioManager.PlaySound(AudioType.InputClick);

        if (puzzleCtrl != null)
            puzzleCtrl.OnMonitorSelect(this);
    }

    public void OnStateChange(SelectionState _newState)
    {
        if (AnimatorCtrl)
        {
            if(_newState == SelectionState.Selected)
            {
                AnimatorCtrl.Play(SelectionAnimation);
            }
            else
                AnimatorCtrl.Play(IdleAnimation);
        }

        if (_newState == SelectionState.Highlighted)
            GameManager.I_GM.AudioManager.PlaySound(AudioType.InputHover);
    }

    public void TypeOn(string _thingsToWrite, bool replaceOldText = true)
    {
        if(replaceOldText)
            textMesh.text = _thingsToWrite;
        else
        {
            textMesh.text = textMesh.text + _thingsToWrite;
        }
    }

    public string GetText()
    {
        return textMesh.text;
    }
}
