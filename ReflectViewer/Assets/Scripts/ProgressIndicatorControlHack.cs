using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressIndicatorControlHack : MonoBehaviour
{
    [SerializeField]
    GameObject m_ProgressBgImage;

    [SerializeField]
    Image m_ProgressImage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_ProgressImage.transform.localEulerAngles += new Vector3(0f, 0f, 180f * Time.deltaTime);
    }

    private void LateUpdate()
    {
        m_ProgressBgImage.transform.localEulerAngles = Vector3.zero;
    }
}
