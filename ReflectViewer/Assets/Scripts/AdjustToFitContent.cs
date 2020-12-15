using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustToFitContent : MonoBehaviour
{
    public float tileHeight = 60f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int count = 0;
        foreach (Transform t in transform)
        {
            if (t.gameObject.activeSelf)
            {
                count++;
            }
        }
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, tileHeight * count);
    }
}
