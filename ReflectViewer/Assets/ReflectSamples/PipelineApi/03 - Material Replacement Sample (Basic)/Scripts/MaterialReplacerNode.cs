using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Reflect;

namespace UnityEngine.Reflect.Pipeline.Samples
{
    [Serializable]
    public class MaterialReplacerNodeSettings
    {
        public string keyword;
        public Material material;
    }

    public class MaterialReplacerNode : ReflectNode<MaterialReplacer>
    {
        public GameObjectInput input = new GameObjectInput();
        public MaterialReplacerNodeSettings settings;

        protected override MaterialReplacer Create(ReflectBootstrapper hook, ISyncModelProvider provider, IExposedPropertyTable resolver)
        {
            var node = new MaterialReplacer(settings);
            input.streamEvent = node.OnGameObjectEvent;
            return node;
        }
    }

    public class MaterialReplacer : IReflectNodeProcessor
    {
        readonly MaterialReplacerNodeSettings m_Settings;

        public MaterialReplacer(MaterialReplacerNodeSettings settings)
        {
            m_Settings = settings;
        }

        public void OnGameObjectEvent(SyncedData<GameObject> stream, StreamEvent streamEvent)
        {
            if (streamEvent == StreamEvent.Added)
            {
                if (!stream.data.TryGetComponent(out MeshRenderer meshRenderer))
                    return;

                var materials = meshRenderer.sharedMaterials;
                for (int i = 0; i < materials.Length; ++i)
                {
                    if (materials[i].name.Contains(m_Settings.keyword))
                    {
                        materials[i] = m_Settings.material;
                    }
                }

                meshRenderer.sharedMaterials = materials;
            }
        }

        public void OnPipelineInitialized()
        {
            // not needed
        }

        public void OnPipelineShutdown()
        {
            // not needed
        }
    }
}
