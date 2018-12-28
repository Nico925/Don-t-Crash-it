using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGPSData", menuName = "PuzzleData/GPS")]
public class PuzzleGPSData : ScriptableObject, IPuzzleData {
    public GameObject Prefab;

    public bool RandomRotation = false;
    [Header("(Longitude, Latitude)")]
    public List<PossibleCoordinate> PossibleCoordinates = new List<PossibleCoordinate>();
    /// <summary>
    /// [Deprecated] No longer needed
    /// </summary>
    /// <returns></returns>
    //public GridData Grid;


    public GameObject GetIPuzzleGO()
    {
        return Prefab;
    }

    [System.Serializable]
    public struct PossibleCoordinate
    {
        public Vector2Int Coordinate;
        public Texture MonitorImage;
    }

    [System.Serializable]
    public class GridData
    {
        public Vector2Int MinMaxLongitude;
        public Vector2Int MinMaxLatitude;
        public Vector2Int GridDimension { get { return new Vector2Int(MinMaxLongitude.y - MinMaxLongitude.x, MinMaxLatitude.y - MinMaxLatitude.x); } }
        public Vector2 GridTileDimension { get { return new Vector2(1f / GridDimension.x, 1f / GridDimension.y); } }
        public int CellPerEdge = 6;
        public Vector2 Scale { get { return new Vector2((float)CellPerEdge / GridDimension.x, (float)CellPerEdge / GridDimension.y); } }
    }
}

public class PuzzleGPSNumericData : IPuzzleInputData
{
    public int ActualValue;
}

public class PuzzleGPSMonitorData: IPuzzleInputData
{
    public int coordinateValue;
}