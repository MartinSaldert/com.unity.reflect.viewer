using SharpFlux;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Reflect.Viewer.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;

public class VisibilityController : MonoBehaviour
{
    public Transform root;
    [SerializeField]
    public ToolButton m_ToggleButton;
    [SerializeField]
    public ToolButton m_HideStuffButton;
    [SerializeField]
    ToolButton m_GhostAllButton;
    [SerializeField]
    ToolButton m_ShowAllButton;
    bool m_ToolbarsEnabled;
    ToolState m_CurrentToolState;
    public RenderObjects ghostRenderer;
    public LayerMask defaultLayer;
    public LayerMask ghostLayer;
    public Canvas secondaryMenu;
    // Start is called before the first frame update
    void Start()
    {
        UIStateManager.stateChanged += OnStateDataChanged;
        m_ToggleButton.buttonClicked += OnToggle;
        m_HideStuffButton.buttonClicked += OnHideStuffButtonClicked;
        m_GhostAllButton.buttonClicked += OnGhostAllButtonClicked;
        m_ShowAllButton.buttonClicked += OnShowAllButtonClicked;
    }
    Vector2 downPoint;
    bool showAllThisFrame = false;

    // Update is called once per frame
    void Update()
    {
        if (!secondaryMenu.enabled)
        {
            showAllThisFrame = false;
            return;
        }
        if (m_HideStuffButton.selected)
        {
            int fingerId = -1;
#if !UNITY_EDITOR
     fingerId = 0; 
#endif
            if (Input.GetMouseButtonDown(0))
            {
                downPoint = Input.mousePosition;
            }
            VRInteraction vri = FindObjectOfType<VRInteraction>();
            if ((Input.GetMouseButtonUp(0) && Vector2.Distance(downPoint, Input.mousePosition) < 40f) || (vri && vri.pressedLastFrame && !vri.isPressing))
            {
                
                Transform controller = null;
                if (vri)
                {
                    controller = vri.transform;
                }
                Transform t = FindObjectOfType<SoveliaUISelectionController>().PressScreenTransform(Input.mousePosition, controller);
                if (t && t.GetComponent<Renderer>() && !showAllThisFrame)
                {
                    t.GetComponent<Renderer>().enabled = false;
                }
            }
        }
        showAllThisFrame = false;
    }

    void OnToggle()
    {
        secondaryMenu.enabled = !secondaryMenu.enabled;
        m_ToggleButton.selected = secondaryMenu.enabled;
    }

    void OnStateDataChanged(UIStateData data)
    {
        if (m_ToolbarsEnabled != data.toolbarsEnabled)
        {
            m_HideStuffButton.button.interactable = data.toolbarsEnabled;
            m_GhostAllButton.button.interactable = data.toolbarsEnabled;
            m_ShowAllButton.button.interactable = data.toolbarsEnabled;
            m_ToolbarsEnabled = data.toolbarsEnabled;
        }

        if (m_CurrentToolState != data.toolState)
        {
            m_CurrentToolState = data.toolState;
            
        }
    }

    void OnHideStuffButtonClicked()
    {
        var toolState = UIStateManager.current.stateData.toolState;
        if (m_HideStuffButton.selected)
        {
            print("Disable Hide Stuff tool");
            toolState.activeTool = ToolType.None;
            m_HideStuffButton.selected = false;
        }
        else
        {
            print("Enable Hide Stuff tool");
            toolState.activeTool = ToolType.HideStuff;
            try
            {
                UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.OpenDialog, DialogType.None));
            } catch (Exception e) { }
            try { 
                UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SetToolState, toolState));
            }
            catch (Exception e) { }
            try { 
                UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.OpenSubDialog, DialogType.None));
            }
            catch (Exception e) { }
            if (FindObjectOfType<RightSideBarController>())
            {
                FindObjectOfType<SoveliaRightSideBarController>().m_MeasureButton.selected = false;
                FindObjectOfType<SoveliaRightSideBarController>().m_SelectButton.selected = false;
            }
            UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SetObjectPicker, FindObjectOfType<SoveliaRightSideBarController>().m_ObjectSelector));
            toolState.activeTool = ToolType.HideStuff;
            m_HideStuffButton.selected = true;
        }
        //UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SetToolState, toolState));
    }

    void OnGhostAllButtonClicked()
    {
        var toolState = UIStateManager.current.stateData.toolState;
        if (m_GhostAllButton.selected)
        {
            print("Disable Ghost All tool");
            m_GhostAllButton.selected = false;
            FindObjectOfType<UISelectionController>().m_CurrentLayer = defaultLayer;
            UISelectionController.SetLayer(root.gameObject, defaultLayer);
            if (UIStateManager.current.stateData.toolState.activeTool == ToolType.SelectTool && FindObjectOfType<UISelectionController>().m_CurrentSelectedGameObject)
            {
                UISelectionController.SetLayer(FindObjectOfType<UISelectionController>().m_CurrentSelectedGameObject, "Selection");
            }
        }
        else
        {
            print("Enable Ghost All tool");
            m_GhostAllButton.selected = true;
            FindObjectOfType<UISelectionController>().m_CurrentLayer = ghostLayer;
            UISelectionController.SetLayer(root.gameObject, "GhostFilter");
            if (UIStateManager.current.stateData.toolState.activeTool == ToolType.SelectTool && FindObjectOfType<UISelectionController>().m_CurrentSelectedGameObject)
            {
                UISelectionController.SetLayer(FindObjectOfType<UISelectionController>().m_CurrentSelectedGameObject, "Selection");
            }
        }
    }

    void OnShowAllButtonClicked()
    {
        foreach (Renderer r in root.GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
        if (m_GhostAllButton.selected)
        {
            OnGhostAllButtonClicked();
        }
        showAllThisFrame = true;
    }
}
