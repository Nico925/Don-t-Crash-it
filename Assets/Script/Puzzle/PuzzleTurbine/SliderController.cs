using UnityEngine;

public class SliderController : MonoBehaviour {

    public Transform LineBegin;
    public Transform LineEnd;
    Vector3 maxLenght { get { return LineEnd.localPosition - LineBegin.localPosition; } }

    public Material PositiveColor;
    public Material NeutralColor;
    public Material NegativeColor;

    LineRenderer lineRenderer;

    private void OnEnable()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.SetPosition(0, LineBegin.localPosition);
        lineRenderer.SetPosition(1, LineEnd.localPosition);
    }

    public void SetFillAmount(float _percentage, bool adaptColor = true)
    {
        Vector3 newLineHead;

        newLineHead = LineBegin.localPosition + maxLenght * _percentage / 100;

        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newLineHead);

        if(adaptColor)
            AdaptMaterial(_percentage);
    }

    void AdaptMaterial(float _percentage)
    {
        if (_percentage == 50)
            lineRenderer.material = PositiveColor;
        else if (_percentage <= 0 || _percentage >= 100)
            lineRenderer.material = NegativeColor;
        else
            lineRenderer.material = NeutralColor;
    }
}
