using UnityEngine;

public class GameManager : MonoBehaviour {

    static GameManager _iGM;
    public static GameManager I_GM
    {
        get {return _iGM;}
        set
        {
            if (_iGM != null && _iGM != value)
                DestroyImmediate(value.gameObject);
            else
            {
                _iGM = value;
                DontDestroyOnLoad(_iGM.gameObject);
            }
        }
    }
    public LevelSettings DefaultDebug;
    [Header("Settings")]
    public LevelSettings Easy;
    public LevelSettings Medium;
    public LevelSettings Hard;

    public DifficoultyLevel ChosenDifficoulty { get; private set; }
    public LevelSettings ChosenSetting { get; private set; }

    public AudioMng AudioManager { get; private set; }

    public void Awake()
    {
        I_GM = this;
        ChosenSetting = Instantiate(DefaultDebug);
        AudioManager = GetComponentInChildren<AudioMng>();
    }

    private void Start()
    {

    }

    public void SetDifficultyLevel(DifficoultyLevel _difficoulty)
    {
        ChosenDifficoulty = _difficoulty;
        switch (_difficoulty)
        {
            case DifficoultyLevel.Default:
                ChosenSetting = Instantiate(DefaultDebug);
                break;
            case DifficoultyLevel.Easy:
                ChosenSetting = Instantiate(Easy);
                break;
            case DifficoultyLevel.Medium:
                ChosenSetting = Instantiate(Medium);
                break;
            case DifficoultyLevel.Hard:
                ChosenSetting = Instantiate(Hard);
                break;
            default:
                break;
        }
    }
}

public enum DifficoultyLevel
{
    Default = 0,
    Easy = 1,
    Medium,
    Hard
}