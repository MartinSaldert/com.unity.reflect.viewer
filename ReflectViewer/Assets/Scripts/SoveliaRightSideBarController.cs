using SharpFlux;
using System.Collections;
using System.Collections.Generic;
using Unity.Reflect.Viewer.UI;
using UnityEngine;

public class SoveliaRightSideBarController : RightSideBarController
{
    [SerializeField]
    public ToolButton m_MeasureButton;

    [SerializeField]
    ToolButton m_SetPivotButton;

    void Awake()
    {
        base.Awake();
        m_SetPivotButton.buttonClicked += OnSetPivotButtonClicked;
        m_MeasureButton.buttonClicked += OnMeasureButtonClicked;
    }

    protected override void OnStateDataChanged(UIStateData data)
    {
        if (m_ToolbarsEnabled != data.toolbarsEnabled)
        {
            m_OrbitButton.button.interactable = data.toolbarsEnabled;
            m_LookAroundButton.button.interactable = data.toolbarsEnabled;
            m_SelectButton.button.interactable = data.toolbarsEnabled;
            m_SunStudyButton.button.interactable = data.toolbarsEnabled;
            m_SetPivotButton.button.interactable = data.toolbarsEnabled;
            m_MeasureButton.button.interactable = data.toolbarsEnabled;
            m_ToolbarsEnabled = data.toolbarsEnabled;
        }

        if (m_CurrentToolState != data.toolState)
        {
            m_OrbitButton.selected = false;

            m_LookAroundButton.selected = false;
            m_SelectButton.selected = false;
            m_SetPivotButton.selected = false;

            if (data.toolState.activeTool == ToolType.SelectTool)
            {
                m_SelectButton.selected = true;
            }
            else if (data.toolState.activeTool == ToolType.OrbitTool)
            {
                if (data.toolState.orbitType == OrbitType.OrbitAtPoint)
                {
                    m_CurrentOrbitButtonType = data.toolState.activeTool;
                    m_OrbitButton.selected = true;
                    m_OrbitButton.SetIcon(m_OrbitImage);
                }
                else if (data.toolState.orbitType == OrbitType.WorldOrbit)
                {
                    m_LookAroundButton.selected = true;
                }
            }
            else if (data.toolState.activeTool == ToolType.ZoomTool)
            {
                m_CurrentOrbitButtonType = data.toolState.activeTool;
                m_OrbitButton.selected = true;
                m_OrbitButton.SetIcon(m_ZoomImage);
            }
            else if (data.toolState.activeTool == ToolType.PanTool)
            {
                m_CurrentOrbitButtonType = data.toolState.activeTool;
                m_OrbitButton.selected = true;
                m_OrbitButton.SetIcon(m_PanImage);
            }
            else if (data.toolState.activeTool == ToolType.SetPivot)
            {
                m_SetPivotButton.selected = true;
            }
            else if (data.toolState.activeTool == ToolType.MeasureTool)
            {
                m_MeasureButton.selected = true;
            }
            else if (data.toolState.activeTool == ToolType.SelectTool)
            {
                m_MeasureButton.selected = false;
                m_SelectButton.selected = true;
            }
            m_CurrentToolState = data.toolState;
        }
    }

    void OnSetPivotButtonClicked()
    {
        //isResettingPivot = true;

        var toolState = UIStateManager.current.stateData.toolState;
        if (m_SetPivotButton.selected)
        {
            toolState.activeTool = ToolType.None;
            m_SetPivotButton.selected = false;
        }
        else
        {
            toolState.activeTool = m_SelectButton.selected ? ToolType.None : ToolType.SetPivot;
            m_SetPivotButton.selected = true;
            UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.OpenSubDialog, DialogType.None));
            UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SetObjectPicker, m_ObjectSelector));
        }
        UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SetToolState, toolState));
    }

    void OnMeasureButtonClicked()
    {
        var toolState = UIStateManager.current.stateData.toolState;

        if (m_MeasureButton.selected)
        {
            print("Disable measure tool");
            toolState.activeTool = ToolType.None;
            m_MeasureButton.selected = false;
            UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.OpenDialog, DialogType.None));
        }
        else
        {
            print("Enable measure tool");
            //HideSelectButton();
            toolState.activeTool = ToolType.MeasureTool;
            m_MeasureButton.selected = true;
            UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.OpenSubDialog, DialogType.None));
            UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.OpenDialog, DialogType.MeasureTool));
            FindObjectOfType<VisibilityController>().m_HideStuffButton.selected = false;
            UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SetObjectPicker, m_ObjectSelector));
        }
        UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SetToolState, toolState));
    }
}
