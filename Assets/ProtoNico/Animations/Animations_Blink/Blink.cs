using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour {

    Text text;
    string textTrattino = "|";
    string blankText = "";
    string staticText = "";
   
    bool isBlinking = true;


	// Use this for initialization
	void Start () {
        text = GameObject.Find("Result").GetComponent<Text>();
       

    }

	
	
    public IEnumerator BlinkText()
    {
        while (isBlinking)
        {
            text.text = blankText;
            yield return new WaitForSeconds(0.5f);

            text.text = textTrattino;
            yield return new WaitForSeconds(0.5f);
        }
        
    }


    public IEnumerator StopBlinking()
    {
        yield return new WaitForSeconds(5f);
        isBlinking = false;
        text.text = staticText;


    }

    public void Active()
    {

        StartCoroutine(BlinkText());
        isBlinking = true;
    }

    public void Disactive()
    {
        StartCoroutine(StopBlinking());
        isBlinking = false;

    }

}
