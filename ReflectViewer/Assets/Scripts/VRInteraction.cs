using System.Collections;
using System.Collections.Generic;
using Unity.Reflect.Viewer.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRInteraction : MonoBehaviour
{
    [SerializeField] InputActionAsset m_InputActionAsset;
    [SerializeField] float m_TriggerThreshold = 0.5f;
    InputAction m_SelectAction;
    public bool pressedLastFrame = false;
    public bool isPressing = false;
    private XRInteractorLineVisual line;
    private void Start()
    {
        m_SelectAction = m_InputActionAsset["VR/Select"];
        line = GetComponent<XRInteractorLineVisual>();
    }
    // Start is called before the first frame update
    void LateUpdate()
    {
        pressedLastFrame = isPressing;
        bool isButtonPressed = m_SelectAction.ReadValue<float>() > m_TriggerThreshold;

        if (!isButtonPressed)
        {
            isPressing = false;
            ShortLaser();
            return;
        }
        else
        {
            isPressing = true;
        }
        if (isPressing)
        {
            LongLaser();
        }
        else
        {
            ShortLaser();
        }
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
}
