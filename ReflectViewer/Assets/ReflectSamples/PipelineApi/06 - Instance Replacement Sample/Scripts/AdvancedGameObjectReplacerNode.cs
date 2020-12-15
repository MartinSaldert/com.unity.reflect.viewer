using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Reflect;

namespace UnityEngine.Reflect.Pipeline.Samples
{
    [Serializable]
    public class AdvancedGameObjectReplacerNodeSettings
    {
        [Serializable]
        public class ReplacementEntry
        {
            public string category;
            public string family;
            public GameObject prefab;
        }

        public List<ReplacementEntry> entries;
    }

    public class AdvancedGameObjectReplacerNode : ReflectNode<AdvancedGameObjectReplacer>
    {
        public StreamInstanceInput input = new StreamInstanceInput();
        public StreamInstanceOutput output = new StreamInstanceOutput();
        public AdvancedGameObjectReplacerNodeSettings settings;


        protected override AdvancedGameObjectReplacer Create(ReflectBootstrapper hook, ISyncModelProvider provider, IExposedPropertyTable resolver)
        {
            var node = new AdvancedGameObjectReplacer(settings, output);
            input.streamBegin = output.SendBegin;
            input.streamEvent = node.OnStreamInstanceEvent;
            input.streamEnd = output.SendEnd;

            return node;
        }
    }

    public class AdvancedGameObjectReplacer : IReflectNodeProcessor
    {
        readonly AdvancedGameObjectReplacerNodeSettings m_Settings;
        readonly DataOutput<StreamInstance> m_Output;

        public AdvancedGameObjectReplacer(AdvancedGameObjectReplacerNodeSettings settings, DataOutput<StreamInstance> output)
        {
            m_Settings = settings;
            m_Output = output;
        }

        public void OnStreamInstanceEvent(SyncedData<StreamInstance> stream, StreamEvent streamEvent)
        {
            if (streamEvent == StreamEvent.Added)
            {
                var streamInstance = stream.data;
                var metadata = streamInstance.instance.Metadata;

                if (metadata != null)
                {
                    if (metadata.Parameters.TryGetValue("Category", out var category) && metadata.Parameters.TryGetValue("Family", out var family))
                    {
                        foreach (var entry in m_Settings.entries)
                        {
                            if (category.Value.Contains(entry.category) && family.Value.Contains(entry.family))
                            {
                                var gameObject = Object.Instantiate(entry.prefab);
                                ImportersUtils.SetTransform(gameObject.transform, streamInstance.instance.Transform);

                                return;
                            }
                        }
                    }
                }
            }

            m_Output.SendStreamEvent(stream, streamEvent);
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
