//using DynamicRaycastJobs;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    public GameObject testCube;
    // Start is called before the first frame update
    void Start()
    {
    }

    [Button]
    public void InitCube()
    {
        /*
        SRCMesh drcm = testCube.AddComponent<SRCMesh>();
        drcm.renderComponent = testCube.GetComponent<Renderer>();
        drcm.objectType = ObjectType.RigidMesh;
        drcm.rootTransform = testCube.transform;
        drcm.Initialize(testCube.GetComponent<Renderer>(), testCube.transform);
    */
    }

    // Update is called once per frame
    void Update()
    {/*
        if (Input.GetMouseButtonDown(0))
        {
            DRCHitInfo hit = new DRCHitInfo();
            DRCS.RayCast(DRCRay.RayCastMainCam(), 10f, ref hit, true);
            print(hit.hitObject);
        }
        */
    }
}
