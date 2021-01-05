using System.Collections;
using System.Collections.Generic;
using Unity.Reflect.Viewer.UI;
using UnityEngine;
using Unity.Reflect.Viewer.UI;
using UnityEngine.InputSystem;
using UnityEngine.Reflect.Viewer.Pipeline;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class VRInteraction : MonoBehaviour
{
    [SerializeField] InputActionAsset m_InputActionAsset;
    [SerializeField] float m_TriggerThreshold = 0.5f;
    [SerializeField] Transform m_ControllerTransform;
    InputAction m_SelectAction;
    public bool pressedLastFrame = false;
    public bool isPressing = false;
    private XRInteractorLineVisual line;
    [SerializeField] XRBaseInteractable m_Target;
    ISpatialPicker<Tuple<GameObject, RaycastHit>> m_ObjectPicker;
    Ray m_Ray;
    readonly List<Tuple<GameObject, RaycastHit>> m_Results = new List<Tuple<GameObject, RaycastHit>>();
    private bool isActive = false;
    private void Start()
    {
        m_SelectAction = m_InputActionAsset["VR/Select"];
        line = GetComponent<XRInteractorLineVisual>();
        UIStateManager.stateChanged += OnStateDataChanged;
        UIStateManager.projectStateChanged += OnProjectStateDataChanged;
        m_Target.gameObject.SetActive(false);
    }

    void OnStateDataChanged(UIStateData data)
    {
        isActive = false;
        if (data.toolState.activeTool == ToolType.MeasureTool || data.toolState.activeTool == ToolType.HideStuff)
        {
            isActive = true;
        }

        if (m_Target == null || m_Target.gameObject == null)
            return;

        m_Target.gameObject.SetActive(isActive);
    }

    void OnProjectStateDataChanged(UIProjectStateData data)
    {
        m_ObjectPicker = data.objectPicker;
    }

    // Start is called before the first frame update
    void LateUpdate()
    {
        pressedLastFrame = isPressing;
        bool isButtonPressed = m_SelectAction.ReadValue<float>() > m_TriggerThreshold;
        UpdateTarget();

        if (m_Target == null || m_Target.gameObject == null)
            return;

        m_Target.gameObject.SetActive(isActive);

        if (!isButtonPressed)
        {
            isPressing = false;
            //ShortLaser();
            return;
        }
        else
        {
            isPressing = true;
        }
        if (isPressing)
        {
            //LongLaser();
        }
        else
        {
            //ShortLaser();
        }
    }

    void UpdateTarget()
    {
        m_Ray.origin = m_ControllerTransform.position;
        m_Ray.direction = m_ControllerTransform.forward;

        // disable the target first so it doesn't interfere with the raycasts
        m_Target.gameObject.SetActive(false);

        // pick
        m_Results.Clear();
        try
        {
            m_ObjectPicker.Pick(m_Ray, m_Results);
            if (m_Results.Count == 0)
                return;

            foreach (Tuple<GameObject, RaycastHit> t in m_Results)
            {
                if (t.Item1.GetComponent<Renderer>() && t.Item1.GetComponent<Renderer>().enabled)
                {
                    m_Target.transform.position = t.Item2.point;
                    m_Target.gameObject.SetActive(true);
                    break;
                }
            }
        } catch (Exception e)
        {

        }

        // enable the target if there is a valid hit
        
        
    }

    public void LongLaser()
    {
        Vector3 pos = FindObjectOfType<SoveliaUISelectionController>().PressScreen(Input.mousePosition, transform);  // Think error is here
        if (pos.y > -10000000)
        {
            line.lineLength = Vector3.Distance(transform.position, pos);
        }
        else
        {
            ShortLaser();
        }
    }

    public void ShortLaser()
    {
        line.lineLength = 0.3f;
    }

    public void HideVisual()
    {
        m_Target.gameObject.SetActive(false);
    }

    public void ShowVisual()
    {
        m_Target.gameObject.SetActive(true);
    }
}
