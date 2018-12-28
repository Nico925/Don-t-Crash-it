using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class MenuPauseController : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject TutorialPanel;
    public GameObject VictoryPanel;
    public GameObject DefeatPanel;
    EventSystem currentES;
    AudioMng audioMng;
    public bool CanPause = true;

    FadeController fadeCtrl;

    private void Start()
    {
        currentES = FindObjectOfType<EventSystem>();
        fadeCtrl = GetComponentInChildren<FadeController>();
        audioMng = GameManager.I_GM.AudioManager;
    }

    private void Update()
    {
        if(CanPause)
            if (Input.GetButtonDown("Cancel"))
                TogglePauseMenu();

        if (TutorialPanel.activeSelf && Input.GetMouseButtonDown(1))
            ToggleTutorial();
    }

    public void PlayClickSound()
    {
        audioMng.PlaySound(AudioType.MenuInput);
    }

    public void GoMainMenu(int _sceneIndex = 0)
    {
        //Audio down
        audioMng.FadeAll(0);
        audioMng.Clear();

        fadeCtrl.FadeIn(() => { SceneManager.LoadScene(_sceneIndex); });
    }

    public void TogglePauseMenu()
    {
        if (!PausePanel.activeSelf)
            ToggleMenu(PausePanel);
        else
            ToggleMenu(null);
    }

    public void ToggleVictoryMenu()
    {
        ToggleMenu(VictoryPanel);
    }

    public void ToggleDefeatMenu()
    {
        ToggleMenu(DefeatPanel);
    }

    public void ToggleTutorial()
    {
        if (!TutorialPanel.activeSelf)
            ToggleMenu(TutorialPanel);
        else
            ToggleMenu(null);
    }

    public void RestartLevel()
    {
        //Audio down
        audioMng.FadeAll(0);
        audioMng.Clear();

        fadeCtrl.FadeIn(()=> { SceneManager.LoadScene(1); });
    }

    public void Quit()
    {
        fadeCtrl.FadeIn(()=> { Application.Quit(); });
    }

    void ToggleMenu(GameObject _menuObj)
    {
        //Toggle off all the menues
        if (PausePanel.activeSelf)
            PausePanel.SetActive(false);
        if (VictoryPanel.activeSelf)
            VictoryPanel.SetActive(false);
        if (DefeatPanel.activeSelf)
            DefeatPanel.SetActive(false);
        if (TutorialPanel.activeSelf)
            TutorialPanel.SetActive(false);

        //Toggle on only the desired one
        if(_menuObj)
            _menuObj.SetActive(true);

        fadeCtrl.FadeOut();
    }
}
