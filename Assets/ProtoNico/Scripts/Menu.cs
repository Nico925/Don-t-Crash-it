using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	public MenuOptions menuOptionPrefab;
	public GameObject container;

    int index = 0; 
	public Color normalColor,lockedColor;
    


	public MenuVoice[] voices;


	
	void Start () {
		
		VewMenuInGame();
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.UpArrow)){
            if (index == 0)
            {
                SelectionOptions(voices.Length - 1);
            }
            else
            {
                SelectionOptions(index - 1);
            }
        }
		if(Input.GetKeyDown(KeyCode.DownArrow)){
            if (index == voices.Length - 1)
            {
                SelectionOptions(0);
            }
            else
            {
                SelectionOptions(index + 1);
            }
        }


		if(Input.GetKeyDown(KeyCode.Return)){
			ActionOptions(index);
		}
	}







    void VewMenuInGame()
    {
        for (int i = 0; i < voices.Length; i++)
        {
            MenuOptions options = Instantiate(menuOptionPrefab, container.transform);
            options.menuName.text = voices[i].text;
            if (voices[i].isLocked)
            {
                options.arrow.color = lockedColor;
                options.menuName.color = lockedColor;
            }
            else
            {
                options.arrow.color = normalColor;
                options.menuName.color = normalColor;
            }
                voices[i].option = options;
            
            if (i == index)
            {
              voices[i].option.arrow.gameObject.SetActive(true);
            }
            else
            {
              voices[i].option.arrow.gameObject.SetActive(false);
            }
            
        }
    }



  

     public void SelectionOptions(int _newindex)
     {
		voices[index].option.arrow.gameObject.SetActive(false);
		voices[_newindex].option.arrow.gameObject.SetActive(true);
		index = _newindex;
	 }

	public void ActionOptions(int _index){
		if(voices[_index].isLocked){
			print("Questa funzione è bloccata!");
		}
		else{
			voices[_index].Azione.Invoke();
		}
	}

	public void NextScene(int _sceneindex){
		SceneManager.LoadScene(_sceneindex);
	}

	public void ExitGame(){
		Application.Quit();
	}
}


[System.Serializable]
public class MenuVoice{
	public string text;
	public bool isLocked;
	public MenuOptions option; 
	public UnityEvent Azione;
}
