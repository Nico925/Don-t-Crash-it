using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Basic class that implement the Selection behaviours
/// She class the API of ISelectable to all the ISelectable she has (each ISelectable on the same GO by default)
/// Use UpdateSelectables to force new ISelectable in or restore default.
/// It also work as a Tree Struture with other SelectableBehaviour and allow only one in the tree to be Selected at once.
/// </summary>
[RequireComponent (typeof(Collider))]
public class SelectableBehaviour : MonoBehaviour
{
    //State property
    SelectionState _state;
    public SelectionState State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;

            OnStateChange(_state);
        }
    }
    //Tree Structure fieds
    SelectableBehaviour _parent;
    SelectableBehaviour Parent
    {
        get
        {
            if (!_parent)
                _parent = GetParent();

            return _parent;
        }
        set { _parent = value; }
    }
    List<SelectableBehaviour> Children = new List<SelectableBehaviour>();
    List<SelectableBehaviour> Siblings
    {
        get
        {
            if (Parent)
                return GetSiblings();
            else
                return new List<SelectableBehaviour>();
        }
    }
    //Selectables to notify
    List<ISelectable> selectables = new List<ISelectable>();

    Collider selectionCollider;

    #region API
    #region Input hook up
    public bool HasMouseOver { get; protected set; }
    private void OnMouseUpAsButton()
    {
        Select();
    }

    private void OnMouseEnter()
    {
        HasMouseOver = true;

        if (State == SelectionState.Neutral)
            State = SelectionState.Highlighted;
    }

    private void OnMouseExit()
    {
        HasMouseOver = false;

        if (State == SelectionState.Highlighted)
            State = SelectionState.Neutral;
    }
    #endregion
    /// <summary>
    /// Change the state of this Selectable to selected.
    /// It also call the OnSelection API of all the selectables she has.
    /// </summary>
    public void Select()
    {
        if (State == SelectionState.Selected)
            return;

        State = SelectionState.Selected;
        foreach (ISelectable selectable in selectables)
        {
            selectable.OnSelection();
        }
    }
    
    /// <summary>
    /// Initialize the class
    /// </summary>
    /// <param name="_parent"></param>
    /// <param name="_initialState"></param>
    public void Init(SelectableBehaviour _parent = null, SelectionState _initialState = SelectionState.Neutral)
    {
        selectionCollider = GetComponent<Collider>();
        if (!selectionCollider)
            Debug.LogWarning("No Collider found! Selectable may not work as intended");

        if (Parent && Parent.Children.Contains(this))
            Parent.Children.Remove(this);

        if (_parent)
        {
            Parent = _parent;
            Parent.Children.Add(this);
        }

        State = _initialState;

        UpdateSelectables();
    }
    
    /// <summary>
    /// Get references to all the ISelectable on this GO
    /// </summary>
    public void UpdateSelectables()
    {
        selectables = new List<ISelectable>();

        foreach (ISelectable selectable in GetComponents<ISelectable>())
        {
            if (!selectables.Contains(selectable))
                selectables.Add(selectable);
        }
    }

    /// <summary>
    /// Get references to all the ISelectable on this GO
    /// and add _forcedAdd also
    /// </summary>
    public void UpdateSelectables(ISelectable _forcedAdd)
    {
        UpdateSelectables();

        if (!selectables.Contains(_forcedAdd))
            selectables.Add(_forcedAdd);
    }

    #region Composite Pattern
    /// <summary>
    /// Return all the parent's children except this one
    /// </summary>
    /// <param name="_child"></param>
    /// <returns></returns>
    public List<SelectableBehaviour> GetSiblings()
    {
        if (Parent == null)
            return new List<SelectableBehaviour>();

        //TODO: lista da salvare e aggiornare ad ogni cambio di "children" per ottimizzare gli spazi
        List<SelectableBehaviour> siblings = new List<SelectableBehaviour>();
        foreach (SelectableBehaviour child in Parent.Children)
        {
            if (child != this)
                siblings.Add(child);
        }

        return siblings;
    }

    /// <summary>
    /// Return all the children
    /// </summary>
    /// <returns></returns>
    public List<SelectableBehaviour> GetChildren()
    {
        return Children;
    }

    /// <summary>
    /// Return the Root
    /// </summary>
    /// <returns></returns>
    public SelectableBehaviour GetRoot()
    {
        SelectableBehaviour root = this;
        while (root.Parent)
        {
            root = root.Parent;
        }

        return root;
    }

    /// <summary>
    /// Return the parent of this SelectableBehaviour (climbing the transform hierarchy)
    /// </summary>
    /// <returns></returns>
    public SelectableBehaviour GetParent()
    {
        for (Transform p = transform.parent; p != null; p = p.parent)
        {
            SelectableBehaviour firstParentFound = p.GetComponent<SelectableBehaviour>();
            if (firstParentFound != null)
            {
                return firstParentFound;
            }
        }

        return null;
    }
    #endregion
    #endregion

    void OnStateChange(SelectionState _newState)
    {
        switch (_newState)
        {
            case SelectionState.Neutral:
                ToggleCollider(true);
                foreach(SelectableBehaviour child in Children)
                    child.State = SelectionState.Passive;
                break;

            case SelectionState.Highlighted:
                break;

            case SelectionState.Selected:
                ToggleCollider(false);
                foreach (SelectableBehaviour child in Children)
                    child.State = SelectionState.Neutral;

                GetRoot().EnstablishNewSelection(this);
                break;

            case SelectionState.Passive:
                ToggleCollider(false);
                if(Children.FirstOrDefault(c => c.State == SelectionState.Selected) == null)
                    foreach (SelectableBehaviour child in Children)
                        child.State = SelectionState.Passive;
                break;
        }

        if(selectables.Count > 0)
            foreach (ISelectable selectable in selectables)
            {
                selectable.OnStateChange(_newState);
            }
    }
    
    void ToggleCollider(bool _active)
    {
        if (GetRoot() != this)
        {
            selectionCollider.enabled = _active;
        }
    }
    // Behaviour than only the Root follows
    #region Root Behaviour
    SelectableBehaviour currentSelected { get; set; }

    /// <summary>
    /// It switch the state of the currentSelected before updating it.
    /// </summary>
    /// <param name="_newSelected"></param>
    void EnstablishNewSelection(SelectableBehaviour _newSelected)
    {
        if (_newSelected == null)
            GetRoot().State = SelectionState.Neutral;

        if (_newSelected.State != SelectionState.Selected)
            return;

        if (Parent)
        {
            GetRoot().EnstablishNewSelection(_newSelected);
            return;
        }

        if (currentSelected)
        {
            if (_newSelected.Siblings.Contains(currentSelected) || _newSelected.Children.Contains(currentSelected))
                currentSelected.State = SelectionState.Neutral;
            else
                currentSelected.State = SelectionState.Passive;
        }

        currentSelected = _newSelected;
    }
    #endregion
}

public enum SelectionState
{
    Neutral,
    Highlighted,
    Selected,
    Passive
}