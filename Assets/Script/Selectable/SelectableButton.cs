using UnityEngine;

[RequireComponent(typeof(SelectableBehaviour))]
public class SelectableButton : MonoBehaviour, IPuzzleInput
{
    public TextMesh Text;
    public Renderer IconRenderer;
    public Animator Animator;
    public string AnimationDownName;
    public string AnimationUpName;

    SelectableBehaviour selectable;
    IPuzzle puzzleCtrl;

    #region  IPuzzleInput
    public void Init(IPuzzle _parentPuzzle, IPuzzleInputData _data)
    {
        //Initial data injection
        InputData = _data;

        //setup parent relationship
        puzzleCtrl = _parentPuzzle;

        Animator = GetComponent<Animator>();

        //selectable behaviour setup
        selectable = GetComponent<SelectableBehaviour>();

        selectable.Init((puzzleCtrl as MonoBehaviour).GetComponent<SelectableBehaviour>());
    }

    public void SetAdditionalData(string _label = "", Material _iconMat = null) {
        //Label Set
        if (Text != null)
            Text.text = _label;

        //Icon Set
        if (IconRenderer != null)
            IconRenderer.material = _iconMat;
    }

    public void OnSelection()
    {
        GameManager.I_GM.AudioManager.PlaySound(AudioType.InputClick);

        puzzleCtrl.OnButtonSelect(this);
    }

    public void OnStateChange(SelectionState _newState)
    {
        if (_newState == SelectionState.Highlighted)
            GameManager.I_GM.AudioManager.PlaySound(AudioType.InputHover);
    }

    #region Data injection
    public IPuzzleInputData InputData;

    public void DataInjection(IPuzzleInputData _data) {
        InputData = _data;
    }

    private void OnMouseDown()
    {
        if (Animator)
            Animator.Play(AnimationDownName);
    }

    private void OnMouseUp()
    {
        if (Animator)
            Animator.Play(AnimationUpName);
    }
    #endregion
    #endregion
}
