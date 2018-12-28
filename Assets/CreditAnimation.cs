using UnityEngine;
using UnityEngine.UI;

public class CreditAnimation : MonoBehaviour {

    public float Speed;
    HorizontalLayoutGroup hlg;
    RectOffset hlgPadding;

	// Use this for initialization
	void Start () {
        hlg = GetComponent<HorizontalLayoutGroup>();
	}
	
	// Update is called once per frame
	void Update () {
        hlgPadding = new RectOffset();
        hlgPadding.left = hlg.padding.left;
        hlgPadding.left -= (int)(Time.deltaTime * Speed);
        if (hlgPadding.left < -Screen.width*4)
            hlgPadding.left = 0;

        hlg.padding = hlgPadding;
	}

    public void SetSpeed (int _newSpeed)
    {
        Speed = _newSpeed;
    }
}
