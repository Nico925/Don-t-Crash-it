using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BlinkScreen : MonoBehaviour {

	Text text;
	private float blinkRate = 0.5f;
    private float _blinkRate;
	public bool isShowingCaret = false;
    
   void  Start () {
        _blinkRate = blinkRate;
       text= GameObject.Find("Result").GetComponent<Text>();
    }
	
	
	void Update () {

        GetBlink();
        
	}

    
	public void ShowBlinking(){

		if(isShowingCaret){
			
			text.text = text.text.Remove(text.text.Length -1);
            
		}
		else{
		
			text.text = text.text.Insert(text.text.Length,"|");
           
		}

		isShowingCaret = !isShowingCaret;
	}

    
   
   
    public void GetBlink()
    {
        blinkRate -= Time.deltaTime;
        if (blinkRate <= 0f)
        {
            ShowBlinking();       
            blinkRate = _blinkRate;
        }
    }


    public void NotBlink()
    {
       

    }

    public void YesBlink()
    {
        
    }

    
}
