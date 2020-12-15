using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Reflect.Model;
using UnityEngine;

namespace UnityEngine.Reflect.Pipeline.Samples
{

    [Serializable]
    public class AdvancedMaterialReplacementSettings
    {
        public string keyword;
        public Material material;
    }

    public class AdvancedMaterialReplacementNode : MaterialConverterNode
    {
        public AdvancedMaterialReplacementSettings settings;

        protected override MaterialConverter Create(ReflectBootstrapper hook, ISyncModelProvider provider, IExposedPropertyTable resolver)
        {
            var converter = new AdvancedMaterialConverter(hook.services.eventHub, hook.services.memoryTracker, textureCacheParam.value, output, settings);

            input.streamEvent = converter.OnStreamEvent;

            return converter;
        }
    }
    
    class AdvancedMaterialConverter : MaterialConverter
    {
        readonly AdvancedMaterialReplacementSettings m_Settings;

        public AdvancedMaterialConverter(EventHub hub, MemoryTracker memTracker, ITextureCache textureCache, IOutput<SyncedData<Material>> output, AdvancedMaterialReplacementSettings settings)
            : base(hub, memTracker, textureCache, output)
        {
            m_Settings = settings;
        }

        protected override Material Import(SyncMaterial syncMaterial)
        {
            if (syncMaterial.Name.Contains(m_Settings.keyword))
            {
                return m_Settings.material;
            }

            return base.Import(syncMaterial);
        }
    }
}
