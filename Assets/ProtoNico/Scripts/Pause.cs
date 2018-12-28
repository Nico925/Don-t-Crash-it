using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {
	public GameObject pauseMenu,exitMenu;
    

	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)&& !exitMenu.activeInHierarchy)
        { 
			ToggleMenu(pauseMenu);
		}
	}
    
	public void ToggleMenu(GameObject menu){
		menu.SetActive(!menu.activeInHierarchy);
        
	}

	
	public void ReturnToMainMenu(){
		SceneManager.LoadScene(0);
	}

   


}
