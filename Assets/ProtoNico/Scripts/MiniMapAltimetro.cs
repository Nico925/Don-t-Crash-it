using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MiniMapAltimetro : MonoBehaviour {

    public GameObject mapAltimeter;

	void Start () {
        NotActive();
	}
	
	void Update () {
        if (Input.GetKey(KeyCode.A))
        {
            ActiveMap();
        }
        else
        {
            NotActive();
        }
    }


    void ActiveMap()
    {
        if(!mapAltimeter.activeSelf)
            mapAltimeter.SetActive(true);
    }

    void NotActive()
    {
        if (mapAltimeter.activeSelf)
            mapAltimeter.SetActive(false);
    }

}
