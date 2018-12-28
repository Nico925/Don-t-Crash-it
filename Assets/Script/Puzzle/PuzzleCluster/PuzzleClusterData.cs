using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClusterData", menuName = "PuzzleData/Cluster")]
public class PuzzleClusterData : ScriptableObject, IPuzzleData
{
    public GameObject Prefab;
    public Material LightOffMat;
    public Material ScreenOffMat;

    public float SequenceDuration = 5;
    public float HeatBarBaseFilling = 1;
    public float HeatBarErrorFilling = 1;

    public int SequencesAmount = 6;

    public List<Sequence> Sequences = new List<Sequence>();

    public GameObject GetIPuzzleGO()
    {
        return Prefab;
    }

    [System.Serializable]
    public class Sequence
    {
        [Header("Monitors Images")]
        public Material A1_Mat;
        public Material A2_Mat;
        public Material A3_Mat;
        public Material B4_Mat;
        public Material B5_Mat;
        public Material B6_Mat;
        [Header("Solutions")]
        public List<PuzzleCluster.ClusterButton> RedSolution = new List<PuzzleCluster.ClusterButton>();
        public List<PuzzleCluster.ClusterButton> YellowSolution = new List<PuzzleCluster.ClusterButton>();
        public List<PuzzleCluster.ClusterButton> GreenSolution = new List<PuzzleCluster.ClusterButton>();
        public List<PuzzleCluster.ClusterButton> BlueSolution = new List<PuzzleCluster.ClusterButton>();
    }
}
