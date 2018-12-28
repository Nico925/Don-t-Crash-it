using DG.Tweening;
using UnityEngine;

public class PuzzleAutopilotOutputMonitor : MonoBehaviour {

    public Material[] materialsCDEHIJ = new Material[6];

    public MeshRenderer Icon;
    Material originalMat;

    public void ToggleOnOff(bool isOn = true)
    {
        if(!originalMat)
            originalMat = new Material(Icon.material);

        if (!isOn)
            Icon.material.DOBlendableColor(originalMat.color * new Color(1, 1, 1, 0), 0);
        else
            Icon.material.DOBlendableColor(originalMat.color, 0);
    }

    public void SetMaterial(int index) {
        originalMat = new Material(materialsCDEHIJ[index]);
        Icon.material = originalMat;
    }
}
