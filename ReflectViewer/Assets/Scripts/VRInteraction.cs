using System.Collections;
using System.Collections.Generic;
using Unity.Reflect.Viewer.UI;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRInteraction : MonoBehaviour
{
    public XRController controller;
    public bool pressedLastFrame = false;
    public bool isPressing = false;
    private XRInteractorLineVisual line;
    private void Start()
    {
        controller = GetComponent<XRController>();
        line = GetComponent<XRInteractorLineVisual>();
    }
    // Start is called before the first frame update
    void Update()
    {
        pressedLastFrame = isPressing;
        if (!controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var isButtonPressed))
        {
            isPressing = false;
            ShortLaser();
            return;
        }

        if (!isButtonPressed)
        {
            isPressing = false;
            ShortLaser();
            return;
        } else
        {
            isPressing = true;
        }
        if (isPressing)
        {
            LongLaser();
        } else {
            ShortLaser();
        }
    }
    public void LateUpdate()
    {
    }
    public void LongLaser()
    {
        Vector3 pos = FindObjectOfType<SoveliaUISelectionController>().PressScreen(Input.mousePosition, transform);
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
