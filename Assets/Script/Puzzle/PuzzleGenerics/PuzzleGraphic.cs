using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Gestore della grafica del puzzle. Cambia il materiale a tutti i renderder in parentela, esclusi specifici.
/// </summary>
public class PuzzleGraphic : MonoBehaviour, ISelectable
{
    public GameObject CameraFocusPoint;
    CameraController camCtrl;

    public PuzzleGraphicData Data;
    List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    bool inhibitColorChange;

    private void Start()
    {
        if (Data != null)
            Init(Data);
    }

    public float LightBlinkInterval = .5f;
    float currentLBInterval = 0;
    bool isPulsing = true;
    private void Update()
    {
        if (!isPulsing)
            return;

        currentLBInterval += Time.deltaTime;
        if (currentLBInterval >= LightBlinkInterval)
        {
            Pulse();
            currentLBInterval = 0;
        }
    }

    #region API
    #region ISelectable
    public void OnSelection()
    {
        CameraFocusCall();
    }

    public void OnStateChange(SelectionState _newState)
    {
        Paint(_newState);
    }
    #endregion

    public void Init(PuzzleGraphicData _data)
    {
        Data = _data;

        camCtrl = Camera.main.GetComponent<CameraController>();

        List<MeshRenderer> renderers = GetComponentsInChildren<MeshRenderer>().ToList();
        renderers = renderers.Where(r => r.GetComponent<TextMesh>() == null).ToList();
        List<MeshRenderer> rendSToNotPaint = new List<MeshRenderer>();
        foreach (GameObject gO in Data.DoNotPaintItems)
        {
            if (gO == null)
                continue;

            rendSToNotPaint = gO.GetComponentsInChildren<MeshRenderer>().ToList();
            if(rendSToNotPaint != null && rendSToNotPaint.Count > 0)
                foreach (MeshRenderer gORend in rendSToNotPaint)
                {
                    if (renderers.Contains(gORend))
                        renderers.Remove(gORend);
                }
        }

        meshRenderers = renderers;

        if(Data.ParticlesGroup != null)
            Data.ParticlesGroup.SetActive(false);

        Paint(Data.NeutralMat);
        PaintLights(Data.EmissiveNeutral);
    }

    public void Paint(SelectionState _state)
    {
        if (Data == null || inhibitColorChange)
            return;

        switch (_state)
        {
            case SelectionState.Passive:
                Paint(Data.NeutralMat);
                break;
            case SelectionState.Neutral:
                Paint(Data.NeutralMat);
                break;
            case SelectionState.Highlighted:
                Paint(Data.HighlightedMat);
                break;
            case SelectionState.Selected:
                Paint(Data.NeutralMat);
                break;
            default:
                break;
        }
    }

    public void Paint(PuzzleState _state)
    {
        if (Data == null)
            return;

        switch (_state)
        {
            case PuzzleState.Unsolved:
                isPulsing = true;
                inhibitColorChange = false;
                if(Data.Lights != null)
                    PaintLights(Data.EmissiveNeutral);
                if (Data.ParticlesGroup != null)
                    Data.ParticlesGroup.SetActive(false);
                break;
            case PuzzleState.Broken:
                isPulsing = false;
                inhibitColorChange = true;
                if (Data.Lights != null)
                    PaintLights(Data.EmissiveNegative);
                if (Data.ParticlesGroup != null)
                    Data.ParticlesGroup.SetActive(true);
                break;
            case PuzzleState.Solved:
                isPulsing = false;
                inhibitColorChange = true;
                if (Data.ParticlesGroup != null)
                    Data.ParticlesGroup.SetActive(false);
                if(Data.Lights != null)
                    PaintLights(Data.EmissivePositive);
                break;
            default:
                break;
        }
    }

    public void Paint(Material _mat)
    {
        foreach (Renderer renderer in meshRenderers)
        {
            Material[] newMaterials = renderer.materials;
            for (int i = 0; i < newMaterials.Length; i++)
            {
                if (renderer == Data.Lights && i == 1)
                    continue;
                newMaterials[i] = _mat;
            }
            renderer.materials = newMaterials;
        }
    }
    public void PaintLights(Material _mat)
    {
        foreach (Renderer renderer in meshRenderers)
        {
            Material[] newMaterials = renderer.materials;
            for (int i = 0; i < newMaterials.Length; i++)
            {
                if (renderer == Data.Lights && i == 1)
                    newMaterials[i] = _mat;
            }
            renderer.materials = newMaterials;
        }
    }

    #endregion
    bool blinkOn = false;
    void Pulse()
    {
        blinkOn = !blinkOn;

        if (blinkOn)
            PaintLights(Data.EmissiveNeutral);
        else
            PaintLights(Data.OffMaterial);
    }

    void CameraFocusCall()
    {
        if (CameraFocusPoint != null)
            camCtrl.FocusAt(CameraFocusPoint.transform);
    }
}

[System.Serializable]
public class PuzzleGraphicData
{
    [Header("Materials"), Space]
    public Material NeutralMat;
    public Material HighlightedMat;

    [Tooltip("Lista di tutti i renderer figli che verranno ignorati da tutte le variazioni di colore del puzzle")]
    public List<GameObject> DoNotPaintItems = new List<GameObject>();

    [Header("Puzzle Lights")]
    public MeshRenderer Lights;
    public Material OffMaterial;
    public Material EmissiveNeutral;
    public Material EmissivePositive;
    public Material EmissiveNegative;

    [Header("Puzzle Particles")]
    public GameObject ParticlesGroup;
}
