using UnityEngine;
using UnityEngine.UI;

public class ShowNumber_Blink : MonoBehaviour
{
    
    Text resultText;
    double result = 0.0;
     double multiplier = 1;
  
    
    

  

    // Update is called once per frame
    void Start()
    {
        resultText = GameObject.Find("Result").GetComponent<Text>();
      
    }

    public void WriteTextField()
    {
        resultText.text = "" + result;
    }

   
    public void AddDigit(int d)
    {
        if (multiplier == 1)
        {
            result *= 10;
            result += d;
        }
        else
        {
            result += d * multiplier;
            multiplier /=10;
        }
      
        WriteTextField();
    }

}

