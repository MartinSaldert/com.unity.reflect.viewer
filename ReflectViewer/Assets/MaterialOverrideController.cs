using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Reflect;

public class MaterialOverrideController : MonoBehaviour
{
    public MaterialOverrideObject materialOverrideObject;
    public Transform root;
    public float minFrameTime = 0.1f;
    public float maxFrameTime = 10f;
    private float currentFrameTime;
    public string property = "SoveliaAppearance";
    // Start is called before the first frame update
    void Start()
    {
        currentFrameTime = maxFrameTime;
        StartCoroutine("OverrideMaterials");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator OverrideMaterials()
    {
        Stopwatch stopWatch = new Stopwatch();
        int loopChangesMade = 0;
        int frameChangesMade = 0;
        int index = 0;
        while (true)
        {
            stopWatch.Restart();
            
            while (true)
            {
                if (root.childCount < 1)
                {
                    break;
                }
                if (index >= root.childCount)
                {
                    if (loopChangesMade > 5)
                    {
                        currentFrameTime = maxFrameTime;
                    } else
                    {
                        currentFrameTime = minFrameTime;
                    }
                    loopChangesMade = 0;
                    index = 0;
                }
                Transform o = root.GetChild(index);
                Renderer r = o.GetComponent<Renderer>();
                Metadata md = o.GetComponent<Metadata>();
                if (r && md)
                {
                    string p = md.GetParameter(property);
                    if (p != "")
                    {
                        bool found = false;
                        foreach (MaterialOverride mo in materialOverrideObject.materialOverrides)
                        {
                            string s = mo.property;
                            if (s.ToLower() == p.ToLower())
                            {
                                Material[] mats = r.sharedMaterials;
                                bool changedMats = false;
                                for (int j = 0; j< mats.Length; j++)
                                {
                                    //print(mats[j].name + " | " + mo.material.name);
                                    if (mats[j].name != mo.material.name)
                                    {
                                        mats[j] = mo.material;
                                        loopChangesMade++;
                                        frameChangesMade++;
                                        changedMats = true;
                                    }
                                        
                                }
                                if (changedMats)
                                {
                                    r.sharedMaterials = mats;
                                }
                                found = true;
                            }
                            
                            if (found)
                            {
                                break;
                            }
                        }
                    }
                }
                index++;
                if (stopWatch.Elapsed.Milliseconds >= currentFrameTime)
                {
                    //print("Frametime: " + stopWatch.Elapsed.Milliseconds + "ms | Frame Changes: " + frameChangesMade + " | Loop Changes: " + loopChangesMade);
                    frameChangesMade = 0;
                    break;
                }
            }
            yield return null;

        }
    }
}
