using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimButtons : MonoBehaviour {
	private Animator anim;
   
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
        
	}

    public void PushDownButton()
    {
        anim.SetBool("PushDown", true);
        anim.SetBool("PushIdle", false);
        anim.enabled = true;

    }    


    public void PushIdleButton()
    {
        anim.SetBool("PushIdle", true);
        anim.SetBool("PushDown", false);
        anim.enabled = true;
    }
}
