using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour, ISelectable
{
    public MenuPauseController PauseMenu;

    public void OnSelection()
    {
        PauseMenu.ToggleTutorial();
    }

    public void OnStateChange(SelectionState _state)
    {
    }
}
