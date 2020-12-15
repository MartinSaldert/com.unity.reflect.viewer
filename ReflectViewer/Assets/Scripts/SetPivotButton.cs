using SharpFlux;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Reflect.Viewer.UI;
using UnityEngine;
using UnityEngine.Reflect;
using UnityEngine.Reflect.Viewer.Pipeline;

public class SetPivotButton : MonoBehaviour
{
    private bool isResettingPivot;
    ISpatialPicker<Tuple<GameObject, RaycastHit>> m_ObjectPicker;
    private bool hasDone = false;
    private ToolType lastTool;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (UIStateManager.current.stateData.toolState.activeTool != lastTool)
        {
            hasDone = false;
        }
        lastTool = UIStateManager.current.stateData.toolState.activeTool;
        if (UIStateManager.current.stateData.toolState.activeTool == ToolType.SetPivot)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (hasDone)
                {
                    Vector3 pos = FindObjectOfType<SoveliaUISelectionController>().PressScreen(Input.mousePosition);
                    if (pos.y < -10000000)
                    {
                        pos = FindObjectOfType<SoveliaFreeFlyCamera>().GetTarget();
                    }
                    FindObjectOfType<SoveliaFreeFlyCamera>().SetTarget(pos);
                    //var toolState = UIStateManager.current.stateData.toolState;
                    //toolState.activeTool = ToolType.None;
                    //UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SetToolState, toolState));
                    FindObjectOfType<RightSideBarController>().OnOrbitButtonClicked();
                }
                hasDone = true;
            }
        }
    }    
}
