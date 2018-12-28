using UnityEngine;

public class Error_Panel : MonoBehaviour {

    public Material LightOff;
    public Material LightOn;

    public MeshRenderer Lights;

    public void TurnLights(int _amount)
    {
        Material[] newMat = Lights.materials;
        for (int i = 0; i < newMat.Length; i++)
        {
            if(i != 1)
                newMat[i] = LightOff;
        }

        for (int i = 0; i <= _amount; i++)
        {
            if (i == 1)
                newMat[2] = LightOn;

            if (i == 2)
                newMat[0] = LightOn;

            if (i == 3)
                newMat[3] = LightOn;
        }

        Lights.materials = newMat;
    }
}
