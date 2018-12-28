using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class timer : MonoBehaviour {

    float currentTime;
    public Text timerText;
	
	void Start()
    {
       currentTime = 0;
    }
	
	void Update ()
    {
        currentTime += Time.deltaTime;
        float minutes = (int)(Time.time / 60f);
        float seconds = (int)(Time.time % 60f);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
