using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsButtons : MonoBehaviour {

    Animator anima;
    public bool PushIdle;
    public bool PushDown;

	// Use this for initialization
	void Start () {
		anima=GetComponent<Animator>();
        anima.enabled = false;
    }
	


	// Update is called once per frame
	void Update () {
        
	}

    public void GetAnimationsButtons()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PushDown = true;
            PushIdle = false;
            anima.enabled = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            PushIdle = true;
            PushDown = false;
            anima.enabled = true;
        }
        
        if (PushIdle == true)
        {
            anima.SetBool("PushIdle", true);
            anima.SetBool("PushDown", false);
           
        }

        if (PushDown == true)
        {
            anima.SetBool("PushDown", true);
            anima.SetBool("PushIdle", false);
            
        }

    }

}
