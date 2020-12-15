using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Reflect;

public class SoveliaFreeFlyCamera : FreeFlyCamera
{
    public void SetTarget(Vector3 pos)
    {
        m_DesiredLookAt = pos;
        OrbitAroundLookAt(Vector2.zero);
    }

    public Vector3 GetTarget()
    {
        return m_DesiredLookAt;
    }
}
