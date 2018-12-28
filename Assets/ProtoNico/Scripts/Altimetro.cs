using UnityEngine;
using UnityEngine.UI;


public class Altimetro : MonoBehaviour
{
    public float[] Multipliers = new float[6] { .7f, 1f, 1.3f, 1.6f, 2f, 2.33f };
    public int AudioDelay = 2;
    int currentMultiplayerIndex = 1;
    public GameObject AltitudeArrow;
    public GameObject AltitudeDecimalArrow;
    public float DropSpeed { get { return 1 * Multipliers[currentMultiplayerIndex]; } }
    float MaxAltitude = 1000;
    public float CurrentAltitude { get; private set; }
    
    private int SecondsToMove;
    LevelManager gameController;

    public void Init(LevelManager _lvlMng)
    {
        gameController = _lvlMng;

        MaxAltitude = _lvlMng.Setting.StartingAltitude;

        CurrentAltitude = MaxAltitude;
        if (AltitudeArrow == null)
            AltitudeArrow = this.gameObject;
    }

    private void Update()
    {
        //if (!gameController)
        //    return;

        UpdateAltitude();
        RotateArrows();
       // GetMoveArrowSeconds();
    }

    int altitudeMarkPre;
    int altitudeMarkPost;
    void UpdateAltitude()
    {
        gameController.NotifyAltitudeUpdate(MaxAltitude, CurrentAltitude);

        if (CurrentAltitude <= 0)
        {
            CurrentAltitude = 0;
            return;
        }

        altitudeMarkPre = (int)CurrentAltitude % AudioDelay;

        CurrentAltitude -= Time.deltaTime * DropSpeed;

        altitudeMarkPost = (int)CurrentAltitude % AudioDelay;

        if (altitudeMarkPost == 0 && altitudeMarkPost != altitudeMarkPre)
            GameManager.I_GM.AudioManager.PlaySound(AudioType.Altimeter);
        
    }
    
    public void Accelerate()
    {
        currentMultiplayerIndex += currentMultiplayerIndex >= Multipliers.Length -1? 0 : 1;
    }

    public void Decelerate(bool goPositive = false)
    {
        if(goPositive)
            currentMultiplayerIndex -= currentMultiplayerIndex <= 0 ? 0 : 1;
        else
            currentMultiplayerIndex -= currentMultiplayerIndex <= 1 ? 0 : 1;
    }
    
    void RotateArrows()
    {
        float currentAngle = (360 * CurrentAltitude) / MaxAltitude;
        if (CurrentAltitude == 0)
            currentAngle = 0;

        AltitudeArrow.transform.localRotation = Quaternion.AngleAxis(currentAngle, -Vector3.right);
        AltitudeDecimalArrow.transform.localRotation = Quaternion.AngleAxis(currentAngle * 10, -Vector3.right);
    }

    //muove la seconda freccia più sottile al secondo 
    void GetMoveArrowSeconds()
    {
        SecondsToMove = (int)(Time.time % 60);
        gameObject.transform.localRotation = Quaternion.AngleAxis(SecondsToMove, Vector3.back);
    }
}

