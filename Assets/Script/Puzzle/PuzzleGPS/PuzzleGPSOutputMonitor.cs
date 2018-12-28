using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGPSOutputMonitor : MonoBehaviour {

    public MeshRenderer MapDisplay;
    public GameObject NText;
    public GameObject EText;
    public GameObject SText;
    public GameObject WText;
    Material mapMaterial;
    //PuzzleGPSData data;

    private void Start()
    {
        InitMapMaterial();
    }

    public void Init(PuzzleGPSData _data)
    {
        //data = _data;

        if (!mapMaterial)
            InitMapMaterial();

        //mapMaterial.SetTextureScale("_MainTex", data.Scale);
    }

    void InitMapMaterial()
    {
        mapMaterial = new Material(MapDisplay.material);
        MapDisplay.material = mapMaterial;
    }

    /// <summary>
    /// It display specific coordinates on the monitor and rotate it with a specific angle
    /// </summary>
    /// <param name="_coordinatesToDisplay"></param>
    public void DisplayAndRotate(PuzzleGPSData.PossibleCoordinate _coordinatesToDisplay, float _angle)
    {
        DisplayCoordinates(_coordinatesToDisplay);
        Rotate(_angle);
    }

    public void DisplayCoordinates(PuzzleGPSData.PossibleCoordinate _coordToShow)
    {
        mapMaterial.mainTexture = _coordToShow.MonitorImage;
    }
    /// <summary>
    /// [Deprecated] use ShowCoordinates instead
    /// Displays a specific coordinate on the monitor
    /// </summary>
    /// <param name="_coordinatesToDisplay"></param>
    //public void DisplayCoordinates(Vector2Int _coordinatesToDisplay)
    //{
    //    Vector2 lowleftCorner = _coordinatesToDisplay - new Vector2(data.MinMaxLongitude.x, data.MinMaxLatitude.y) - Vector2.one * data.CellPerEdge / 2;
    //    mapMaterial.SetTextureOffset("_MainTex", new Vector2(data.GridTileDimension.x * lowleftCorner.x, data.GridTileDimension.y * lowleftCorner.y));
    //}
    /// <summary>
    /// Rotates the monitor by a specific angle
    /// </summary>
    /// <param name="_angle"></param>
    public void Rotate(float _angle)
    {
        MapDisplay.transform.Rotate(Vector3.forward, _angle);
        int quarter = (int)_angle / 90;

        switch (quarter)
        {
            case 0:
                NText.SetActive(true);
                break;
            case 1:
                EText.SetActive(true);
                break;
            case 2:
                SText.SetActive(true);
                break;
            case 3:
                WText.SetActive(true);
                break;
            default:
                break;
        }
    }
}
