using System;
using System.Collections.Generic;

namespace UnityEngine.Reflect.Pipeline.Samples
{
    public class SimpleInstantiatorNode : ReflectNode<SimpleInstantiator>
    {
        public StreamInstanceInput input = new StreamInstanceInput();
        public GameObject prefab;
        
        protected override SimpleInstantiator Create(ReflectBootstrapper hook, ISyncModelProvider provider, IExposedPropertyTable resolver)
        {
            var node = new SimpleInstantiator(prefab);
            input.streamEvent = node.OnStreamEvent;

            return node;
        }
    }

    public class SimpleInstantiator : IReflectNodeProcessor
    {
        readonly Dictionary<StreamKey, GameObject> m_Instances;
        GameObject m_Prefab;
        
        public SimpleInstantiator(GameObject prefab)
        {
            m_Prefab = prefab;
            m_Instances = new Dictionary<StreamKey, GameObject>();
        }

        public void OnStreamEvent(SyncedData<StreamInstance> stream, StreamEvent streamEvent)
        {
            if (streamEvent == StreamEvent.Added)
            {
                var gameObject = Object.Instantiate(m_Prefab);
                gameObject.name = stream.data.instance.Name;
                
                ImportersUtils.SetTransform(gameObject.transform, stream.data.instance.Transform);

                m_Instances[stream.key] = gameObject;
            }
            else if (streamEvent == StreamEvent.Changed)
            {
                if (m_Instances.TryGetValue(stream.key, out var gameObject))
                {
                    Object.Destroy(gameObject);
                    m_Instances.Remove(stream.key);
                }
            }
        }

        public void OnPipelineInitialized()
        {
            // Not needed
        }

        public void OnPipelineShutdown()
        {
            m_Instances.Clear();
        }
    }
}
