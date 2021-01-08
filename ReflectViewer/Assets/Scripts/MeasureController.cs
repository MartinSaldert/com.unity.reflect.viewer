using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Reflect.Viewer.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Reflect.Viewer;
using UnityEngine.UI;

public class MeasureController : MonoBehaviour
{
    public LineRenderer lines;
    public float lineThickness = 0.01f;
    public TextMeshProUGUI measureText;
    public GameObject measurePoint;
    private List<GameObject> measurePoints;
    MeasureTool lastFrameTool;
    private List<Vector3> savedPoints;
    [HideInInspector]
    public SpatialSelector m_ObjectSelector;
    // Start is called before the first frame update
    void Start()
    {
        savedPoints = new List<Vector3>();
        measurePoints = new List<GameObject>();
        if (!lines)
        {
            lines = GetComponent<LineRenderer>();
        }
        m_ObjectSelector = new SpatialSelector();
    }

    Vector2 downPoint;

    // Update is called once per frame
    void Update()
    {
        //float scalef = (float)FindObjectOfType<UIStateManager>().stateData.modelScale;
        float scalef = FindObjectOfType<UIStateManager>().m_RootNode.transform.localScale.x;
        lines.startWidth = lineThickness * scalef;
        lines.endWidth = lineThickness * scalef;
        if (UIStateManager.current.stateData.toolState.activeTool == ToolType.SetPivot || UIStateManager.current.stateData.activeDialog != DialogType.MeasureTool)
        {
            return;
        }
        MeasureTool toolState = UIStateManager.current.stateData.toolState.measureTool;
        if (toolState != lastFrameTool)
        {
            ResetMeasurement();
            lastFrameTool = toolState;
            VRInteraction vri = FindObjectOfType<VRInteraction>();
            vri.pressedLastFrame = false;
            return;
        }
        
        if (toolState == MeasureTool.Delete)
        {
            ResetMeasurement();
        }

        lastFrameTool = toolState;

        if (toolState == MeasureTool.Distance )
        {
            int fingerId = -1;
#if !UNITY_EDITOR
     fingerId = 0; 
#endif
            if (Input.GetMouseButtonDown(0))
            {
                downPoint = Input.mousePosition;
            }
            //print(downPoint);
            VRInteraction vri = FindObjectOfType<VRInteraction>();
            if ((Input.GetMouseButtonUp(0) && Vector2.Distance(downPoint, Input.mousePosition) < 40f) || (vri && vri.pressedLastFrame && !vri.isPressing))
            {
                Transform controller = null;
                if (vri)
                {
                    controller = vri.transform;
                }
                Vector3 pos = FindObjectOfType<SoveliaUISelectionController>().PressScreen(Input.mousePosition, controller);
                print(pos);
                if (pos.y > -10000000)
                {
                    savedPoints.Add(pos);
                    GameObject go = Instantiate(measurePoint);
                    Vector3 scale = go.transform.localScale;
                    go.transform.SetParent(FindObjectOfType<UIStateManager>().m_RootNode.transform, true);
                    go.transform.localScale = scale;
                    go.transform.position = pos;
                    measurePoints.Add(go);
                    
                    //measureText.text = Math.Round(CalculateDistance(savedPoints), 2) + "m";
                }
            }
        }
        lines.positionCount = savedPoints.Count;
        int count = 0;
        foreach (GameObject t in measurePoints)
        {
            lines.SetPosition(count, t.transform.position);
            count++;
        }

        measureText.text = Math.Round(CalculateDistance(measurePoints), 2) + "m";
    }

    public float CalculateDistance(List<GameObject> points)
    {
        float distance = 0f;;
        for (int i = 0; i<points.Count; i++)
        {
            if (i == 0)
            {
                continue;
            }
            distance += Vector3.Distance(points[i - 1].transform.localPosition, points[i].transform.localPosition);// / FindObjectOfType<UIStateManager>().m_RootNode.transform.localScale.x;
        }
        return distance;
    }

    public void ResetMeasurement()
    {
        savedPoints.Clear();
        measureText.text = "0m";
        lines.positionCount = 0;
        lines.SetPositions(new List<Vector3>().ToArray());
        foreach (GameObject go in measurePoints)
        {
            Destroy(go);
        }
        measurePoints.Clear();
    }
}
