using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlaneKinematicController : MonoBehaviour {

    public List<Transform> FallTrajectory = new List<Transform>();
    float timeToCrash;
    float pathLenght;
    float fallRatio { get { return pathLenght/ timeToCrash; } }
    int currentPathIndex;

    private void Start()
    {
        foreach (var item in FallTrajectory)
        {
            Renderer rend = item.GetComponent<Renderer>();
            if (rend)
                Destroy(rend);
        }
    }

    public void StartFall(float _timeToFall)
    {
        if(FallTrajectory.Count <= 1)
        {
            Debug.LogWarning("Impossibile calcolare percorso di caduta!/n/rNon ci sono abbastanza punti nel percorso");
            return;
        }

        timeToCrash = _timeToFall;
        currentPathIndex = 0;
        pathLenght = Vector3.Distance(transform.position, FallTrajectory[currentPathIndex].position);
        for (int i = currentPathIndex; i < FallTrajectory.Count -1; i++)
        {
            pathLenght += Vector3.Distance(FallTrajectory[i].position, FallTrajectory[i+1].position);
        }

        GameManager.I_GM.AudioManager.PlayAmbient(currentPathIndex);
        StartCoroutine(Fall());
    }

    public void UpdateFallTime(float _newFallTime)
    {
        timeToCrash = _newFallTime;
        pathLenght = Vector3.Distance(transform.position, FallTrajectory[currentPathIndex].position);
        for (int i = currentPathIndex; i < FallTrajectory.Count - 1; i++)
        {
            pathLenght += Vector3.Distance(FallTrajectory[i].position, FallTrajectory[i + 1].position);
        }
    }

    IEnumerator Fall()
    {
        while (currentPathIndex < FallTrajectory.Count)
        {
            transform.position += (FallTrajectory[currentPathIndex].position - transform.position).normalized * fallRatio * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(FallTrajectory[currentPathIndex].position - transform.position), Time.deltaTime / fallRatio * 10);
            if(Vector3.Distance(transform.position, FallTrajectory[currentPathIndex].position) <= 1)
            {

                currentPathIndex++;
                GameManager.I_GM.AudioManager.PlayAmbient(currentPathIndex);
            }

            yield return null;
        }
    }
}
