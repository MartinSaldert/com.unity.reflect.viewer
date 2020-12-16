using System.Collections;
using System.Collections.Generic;
using Unity.Reflect.Viewer.UI;
using UnityEngine;

public class HideInAR : MonoBehaviour
{
    public List<GameObject> objects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private NavigationMode lastFrameNavigation = NavigationMode.Orbit;

    // Update is called once per frame
    void Update()
    {
        if (UIStateManager.current.stateData.navigationState.navigationMode != lastFrameNavigation && UIStateManager.current.stateData.navigationState.navigationMode == NavigationMode.AR)
        { // In AR
            foreach (GameObject go in objects)
            {
                go.SetActive(false);
            }
        } else if (UIStateManager.current.stateData.navigationState.navigationMode != lastFrameNavigation && UIStateManager.current.stateData.navigationState.navigationMode != NavigationMode.AR)
        {
            foreach (GameObject go in objects)
            {
                go.SetActive(true);
            }
        }
        lastFrameNavigation = UIStateManager.current.stateData.navigationState.navigationMode;
    }
}
