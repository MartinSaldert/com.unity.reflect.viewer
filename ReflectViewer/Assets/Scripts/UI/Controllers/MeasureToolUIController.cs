using System.Collections.Generic;
using SharpFlux;
using Unity.TouchFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.Reflect.Viewer.UI
{
    /// <summary>
    /// Controller responsible for managing the selection of the clipping tool
    /// </summary>
    [RequireComponent(typeof(DialogWindow))]
    public class MeasureToolUIController : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField, Tooltip("Does dialog closes on selection.")]
        public bool m_CloseOnSelect;
        [SerializeField, Tooltip("Button to toggle the dialog.")]
        Button m_DialogButton;
        [SerializeField, Tooltip("List of button controls.")]
        List<ButtonControl> m_ButtonControls = new List<ButtonControl>();
#pragma warning restore CS0649

        ButtonControl m_ActiveButtonControl;
        MeasureTool? m_cachedMeasureTool;

        void Awake()
        {
            UIStateManager.stateChanged += OnStateDataChanged;

            foreach (var buttonControl in m_ButtonControls)
            {
                buttonControl.onControlTap.AddListener(OnButtonTap);
            }
        }

        void OnStateDataChanged(UIStateData stateData)
        {
            if (m_cachedMeasureTool == null || m_cachedMeasureTool != stateData.toolState.measureTool)
            {
                m_cachedMeasureTool = stateData.toolState.measureTool;

                var i = 0;
                foreach (var buttonControl in m_ButtonControls)
                {
                    var on = (i == (int) stateData.toolState.measureTool);
                    buttonControl.@on = on;
                    i++;
                }
            }
        }

        void OnButtonTap(BaseEventData eventData)
        {
            var buttonControl = eventData.selectedObject.GetComponent<ButtonControl>();
            if (buttonControl == null)
                return;

            var measureTool = (MeasureTool)m_ButtonControls.IndexOf(buttonControl);

            if (m_CloseOnSelect)
            {
                UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.OpenDialog, DialogType.None));
            }

            var toolState = UIStateManager.current.stateData.toolState;
            toolState.measureTool = measureTool;
            UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SetToolState, toolState));
            UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SetObjectPicker, FindObjectOfType<MeasureController>().m_ObjectSelector));
        }
    }
}

