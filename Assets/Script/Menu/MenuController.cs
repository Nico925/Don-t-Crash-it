using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public GameObject MainMenuPanel;
    public GameObject LevelSelectionPanel;
    public GameObject CreditsPanel;
    public GameObject IntroPanel;
    public GameObject TutorialPanel;
    EventSystem currentES;
    AudioMng audioMng;
    FadeController fadeCtrl;


    bool isGameStarted;

    private void Start()
    {
        currentES = FindObjectOfType<EventSystem>();
        fadeCtrl = GetComponentInChildren<FadeController>();
        audioMng = GameManager.I_GM.AudioManager;
        isGameStarted = false;
    }

    private void Update()
    {
        if (isGameStarted)
            return;

        if (Input.GetButtonDown("Cancel") || Input.GetMouseButtonDown(1))
            GoToMainMenu();
    }

    #region API
    public void PlayClickSound()
    {
        audioMng.PlaySound(AudioType.MenuInput);
    }

    public void GoToMainMenu()
    {
        ToggleMenu(MainMenuPanel);
    }

    public void GoToSelectionMenu()
    {
        ToggleMenu(LevelSelectionPanel);
    }

    public void GoToIntro()
    {
        isGameStarted = true;
        fadeCtrl.FadeIn(() => { ToggleMenu(IntroPanel); });
    }

    public void GoToTutorial()
    {
        if ((int)GameManager.I_GM.ChosenDifficoulty > 1)
            GoGamePlay(1);
        else
        {
            ToggleMenu(TutorialPanel);
        }
    }

    public void GoToCredits()
    {
        fadeCtrl.FadeIn(() => { ToggleMenu(CreditsPanel); });
    }

    public void QuitGame()
    {
        fadeCtrl.FadeIn(() => { Application.Quit(); });
    }

    public void GoGamePlay(int _sceneIndex)
    {
        //Audio down
        audioMng.FadeAll(0);
        audioMng.Clear();
        fadeCtrl.FadeIn(() => { SceneManager.LoadScene(_sceneIndex); });
    }

    public void SetDifficoulty(int difficoulty)
    {
        GameManager.I_GM.SetDifficultyLevel((DifficoultyLevel)difficoulty);
    }
    #endregion

    void ToggleMenu(GameObject _menuObj)
    {
        //Toggle off all the menues
        if (MainMenuPanel.activeSelf)
            MainMenuPanel.SetActive(false);
        if (LevelSelectionPanel.activeSelf)
            LevelSelectionPanel.SetActive(false);
        if (CreditsPanel.activeSelf)
            CreditsPanel.SetActive(false);
        if (IntroPanel.activeSelf)
            IntroPanel.SetActive(false);
        if (TutorialPanel.activeSelf)
            TutorialPanel.SetActive(false);

        //Toggle on only the desired one
        _menuObj.SetActive(true);

        fadeCtrl.FadeOut();
    }

    void ResetEventSystmSelection(GameObject _selection)
    {
        currentES.SetSelectedGameObject(_selection);
    }
}
