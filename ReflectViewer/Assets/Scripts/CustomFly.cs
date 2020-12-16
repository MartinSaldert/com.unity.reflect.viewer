using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomFly : MonoBehaviour
{
    [SerializeField] XRController m_XrController;
    public float flySpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        m_XrController = GetComponent<XRController>();
    }

    // Update is called once per frame
    void Update()
    {
        return;
        if (!m_XrController.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out var isButtonPressed))
            return;

        if (isButtonPressed)
        {
            GetComponentInParent<XRRig>().transform.position += transform.forward * flySpeed * Time.deltaTime;
        }
    }
}
