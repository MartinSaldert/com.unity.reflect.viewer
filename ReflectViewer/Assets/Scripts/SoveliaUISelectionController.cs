using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Reflect.Viewer.UI;
using UnityEngine;

public class SoveliaUISelectionController : UISelectionController
{
    public Camera orbitCam;
    public Camera ARCam;
    public Vector3 PressScreen(Vector2 screenPoint, Transform controller = null)
    {
        try
        {
        } catch (Exception e)
        {
            print(e);
        }
        Vector3 nada = new Vector3(0f, -100000000, 0f); // Pressed the ether
        Ray ray = new Ray();
        if (UIStateManager.current.stateData.navigationState.navigationMode == NavigationMode.AR)
        {
            m_Camera = ARCam;
            ray = m_Camera.ScreenPointToRay(screenPoint);
        }
        else if (UIStateManager.current.stateData.navigationState.navigationMode == NavigationMode.VR)
        {
            if (controller)
            {
                ray = new Ray(controller.position, controller.forward);
            }
            else
            {
                return nada;
            }
        }
        else
        {
            m_Camera = orbitCam;
            ray = m_Camera.ScreenPointToRay(screenPoint);
        }

        if (m_Camera == null || !m_Camera.gameObject.activeInHierarchy)
        {
            m_Camera = Camera.main;
            if (m_Camera == null)
            {
                Debug.LogError($"[{nameof(UISelectionController)}] active main camera not found!");
                return nada;
            }
            ray = m_Camera.ScreenPointToRay(screenPoint);
        }
        print("Camera: " + m_Camera);
        m_ObjectPicker.Pick(ray, m_Results);
        List<RaycastHit> l = ResultSelect();
        if (l.Count > 0)
        {
            foreach (RaycastHit rh in l)
            {
                if (rh.transform.GetComponent<Renderer>() && rh.transform.GetComponent<Renderer>().enabled)
                {
                    return rh.point;
                }
            }
            return nada;
        }
        else
        {
            return nada;
        }
    }

    public Transform PressScreenTransform(Vector2 screenPoint, Transform controller = null)
    {
        Ray ray = new Ray();
        if (UIStateManager.current.stateData.navigationState.navigationMode == NavigationMode.AR)
        {
            m_Camera = ARCam;
            ray = m_Camera.ScreenPointToRay(screenPoint);
        }
        else if (UIStateManager.current.stateData.navigationState.navigationMode == NavigationMode.VR)
        {
            if (controller)
            {
                FindObjectOfType<VRInteraction>().HideVisual();
                ray = new Ray(controller.position, controller.forward);
            }
            else
            {
                return null;
            }
        }
        else
        {
            m_Camera = orbitCam;
            ray = m_Camera.ScreenPointToRay(screenPoint);
            print(m_Camera);
        }
        if (m_Camera == null || !m_Camera.gameObject.activeInHierarchy)
        {
            print("Camera not active");
            m_Camera = Camera.main;
            if (m_Camera == null)
            {
                Debug.LogError($"[{nameof(UISelectionController)}] active main camera not found!");
                return null;
            }
            ray = m_Camera.ScreenPointToRay(screenPoint);
        }
        print("Camera: " + m_Camera);
        m_ObjectPicker.Pick(ray, m_Results);
        List<RaycastHit> l = ResultSelect();
        if (UIStateManager.current.stateData.navigationState.navigationMode == NavigationMode.VR)
        {
            FindObjectOfType<VRInteraction>().ShowVisual();
        }
        if (l.Count > 0)
        {
            foreach (RaycastHit rh in l)
            {
                //print("Raycasted: " + rh.transform);
                if (rh.transform.GetComponent<Renderer>() && rh.transform.GetComponent<Renderer>().enabled)
                {

                    return rh.transform;
                }
            }
            return null;
        }
        else
        {
            return null;
        }
    }
}
