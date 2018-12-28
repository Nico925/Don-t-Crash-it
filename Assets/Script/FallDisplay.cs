using UnityEngine;

public class FallDisplay : MonoBehaviour {

    public Transform PuntoBase;
    public Transform PuntoInizio;
    public Transform PuntoFine;

    Vector3 basePt { get { return /*PuntoBase.localRotation * */ PuntoBase.localPosition; } }
    Vector3 startPt { get { return /*PuntoInizio.localRotation **/ PuntoInizio.localPosition; } }
    Vector3 endPt { get { return /*PuntoFine.localRotation **/ PuntoFine.localPosition; } }

    public GameObject Aereo;
    public LineRenderer renderer;

    private void Awake()
    {
        renderer.positionCount = 3;

        renderer.SetPosition(0, basePt);
        renderer.SetPosition(1, startPt);
        renderer.SetPosition(2, endPt);
    }

    public void UpdateLine(float _percentage)
    {
        Vector3 newLineHead;

        newLineHead = startPt + (endPt - startPt) * ( 1 - _percentage);

        Aereo.transform.localPosition = newLineHead;

        renderer.SetPosition(2, newLineHead);
    }

}
