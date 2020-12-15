using System;
using System.Collections.Generic;
using System.Linq;
using SharpFlux;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Reflect;
using UnityEngine.Reflect.Viewer.Pipeline;

namespace Unity.Reflect.Viewer.UI
{
    public class UISelectionController : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] float m_Tolerance;
#pragma warning restore CS0649

        protected readonly List<Tuple<GameObject, RaycastHit>> m_Results = new List<Tuple<GameObject, RaycastHit>>();

        HighlightFilterInfo m_CurrentHighlightFilter;
        ObjectSelectionInfo m_CurrentObjectSelectionInfo;
        [HideInInspector]
        public GameObject m_CurrentSelectedGameObject;

        Vector2? m_PreviousScreenPoint;
        public int m_CurrentLayer;
        bool m_SelectMode;
        bool m_Pressed;
        ToolState? m_CachedToolState;
        NavigationState? m_CachedNavigationState;

        [HideInInspector]
        public ISpatialPicker<Tuple<GameObject, RaycastHit>> m_ObjectPicker;
        protected Camera m_Camera;

        void Awake()
        {
            UIStateManager.stateChanged += OnStateDataChanged;
            UIStateManager.projectStateChanged += OnProjectStateDataChanged;

            OrphanUIController.onPointerClick += OnPointerClick;
        }

        void OnStateDataChanged(UIStateData data)
        {
            bool somethingChanged = false;

            if (m_CachedNavigationState != data.navigationState)
            {
                m_CachedNavigationState = data.navigationState;
                somethingChanged = true;
            }
            if (m_CachedToolState != data.toolState)
            {
                m_SelectMode = data.toolState.activeTool == ToolType.SelectTool;
                m_CachedToolState = data.toolState;
                somethingChanged = true;
            }

            if (somethingChanged)
                UpdateSelectedObjectHighlight();
        }

        void OnProjectStateDataChanged(UIProjectStateData data)
        {
            print("Project state data changed");
            m_ObjectPicker = data.objectPicker;
            if (data.objectSelectionInfo != m_CurrentObjectSelectionInfo)
            {
                ResetSelectedObjectHighlight();

                var selectedObject = data.objectSelectionInfo.CurrentSelectedObject();
                if (selectedObject != null)
                {
                    var metadata = selectedObject.GetComponent<Metadata>();
                    if (metadata == null)
                    {
                        while (selectedObject.transform.parent != null)
                        {
                            selectedObject = selectedObject.transform.parent.gameObject;
                            metadata = selectedObject.GetComponent<Metadata>();
                            if (metadata != null)
                                break;
                        }
                    }

                    if (metadata != null)
                    {
                        m_CurrentLayer = selectedObject.layer;
                    }
                    else
                    {
                        selectedObject = null;
                    }
                }

                m_CurrentSelectedGameObject = selectedObject;
                m_CurrentObjectSelectionInfo = data.objectSelectionInfo;

                UpdateSelectedObjectHighlight();
            }

            if (data.highlightFilter != m_CurrentHighlightFilter)
            {
                if (m_CurrentSelectedGameObject != null)
                {
                    m_CurrentLayer = m_CurrentSelectedGameObject.layer;
                    UpdateSelectedObjectHighlight();
                }

                m_CurrentHighlightFilter = data.highlightFilter;
            }
        }

        void ResetSelectedObjectHighlight()
        {
            if (m_CurrentSelectedGameObject != null)
            {
                SetLayer(m_CurrentSelectedGameObject, m_CurrentLayer);
            }
        }

        void UpdateSelectedObjectHighlight()
        {
            if (m_CurrentSelectedGameObject != null)
            {
                if (ShouldDisplaySelection())
                {
                    SetLayer(m_CurrentSelectedGameObject, "Selection");
                }
                else
                {
                    SetLayer(m_CurrentSelectedGameObject, m_CurrentLayer);
                }
            }
        }

        bool ShouldDisplaySelection()
        {
            if (m_CachedNavigationState != null)
            {
                return m_CachedNavigationState.Value.selectionUsageCount != 0;
            }

            return false;
        }

        public static void SetLayer(GameObject obj, string layerName)
        {
            SetLayer(obj, LayerMask.NameToLayer(layerName));
        }

        public static void SetLayer(GameObject obj, int layer)
        {
            obj.SetLayerRecursively(layer);
        }

        void OnPointerClick(BaseEventData data)
        {
            if (!m_SelectMode)
                return;

            var screenPoint = data.currentInputModule.input.mousePosition;

            var info = new ObjectSelectionInfo();

            if (m_PreviousScreenPoint.HasValue && (screenPoint - m_PreviousScreenPoint.Value).magnitude <= m_Tolerance)
            {
                info.selectedObjects = m_CurrentObjectSelectionInfo.selectedObjects;
                info.currentIndex = m_CurrentObjectSelectionInfo.selectedObjects.Count == 0
                    ? 0
                    : (m_CurrentObjectSelectionInfo.currentIndex + 1) %
                    m_CurrentObjectSelectionInfo.selectedObjects.Count;
            }
            else
            {
                if (m_Camera == null || !m_Camera.gameObject.activeInHierarchy)
                {
                    m_Camera = Camera.main;
                    if (m_Camera == null)
                    {
                        Debug.LogError($"[{nameof(UISelectionController)}] active main camera not found!");
                        return;
                    }
                }

                print("Picking: " + m_ObjectPicker);
                m_ObjectPicker.Pick(m_Camera.ScreenPointToRay(screenPoint), m_Results);
                // send a copy of the list to preserve previous selection info
                info.selectedObjects = m_Results.Select(x => x.Item1).ToList();

                info.currentIndex = 0;
            }

            m_PreviousScreenPoint = screenPoint;
            UIStateManager.current.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.SelectObjects, info));
        }

        protected List<RaycastHit> ResultSelect()
        {
            return m_Results.Select(x => x.Item2).ToList();
        }




    }
    
}


