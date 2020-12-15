using System;
using UnityEngine;
using UnityEngine.Reflect.Pipeline;

namespace Unity.Reflect.Samples
{
    public class MetadataHardFilteringSample : MonoBehaviour
    {
        MetadataHardFilter m_MetadataFilterProcessor;

        void Start()
        {
            // Create a pipeline asset

            var pipelineAsset = ScriptableObject.CreateInstance<PipelineAsset>();
            
            // Create the node required for this sample

            var filterNode = pipelineAsset.CreateNode<MetadataHardFilterNode>();

            // Create the rest of the pipeline

            var projectStreamer = pipelineAsset.CreateNode<ProjectStreamerNode>();
            var instanceProvider = pipelineAsset.CreateNode<SyncObjectInstanceProviderNode>();
            var dataProvider = pipelineAsset.CreateNode<DataProviderNode>();
            var meshConverter = pipelineAsset.CreateNode<MeshConverterNode>();
            var materialConverter = pipelineAsset.CreateNode<MaterialConverterNode>();
            var textureConverter = pipelineAsset.CreateNode<TextureConverterNode>();
            var instanceConverter = pipelineAsset.CreateNode<InstanceConverterNode>();
            
            // Inputs / Outputs
            
            pipelineAsset.CreateConnection(projectStreamer.assetOutput, instanceProvider.input);
            
            // The MetadataHardFilterNode needs to be between the SyncObjectInstanceProviderNode and the DataProviderNode
            pipelineAsset.CreateConnection(instanceProvider.output, filterNode.input);

            pipelineAsset.CreateConnection(filterNode.output, dataProvider.instanceInput);
            pipelineAsset.CreateConnection(dataProvider.syncMeshOutput, meshConverter.input);
            pipelineAsset.CreateConnection(dataProvider.syncMaterialOutput, materialConverter.input);
            pipelineAsset.CreateConnection(dataProvider.syncTextureOutput, textureConverter.input);
            pipelineAsset.CreateConnection(dataProvider.instanceDataOutput, instanceConverter.input);

            // Params
            
            pipelineAsset.SetParam(dataProvider.hashCacheParam, projectStreamer);
            pipelineAsset.SetParam(materialConverter.textureCacheParam, textureConverter);
            pipelineAsset.SetParam(instanceConverter.materialCacheParam, materialConverter);
            pipelineAsset.SetParam(instanceConverter.meshCacheParam, meshConverter);

            // Add a ReflectPipeline node and start the pipeline

            var reflectBehaviour = gameObject.AddComponent<ReflectPipeline>();
            
            reflectBehaviour.pipelineAsset = pipelineAsset;
            reflectBehaviour.InitializeAndRefreshPipeline(new SampleSyncModelProvider());

            // Once the pipeline is started, keep a link to the processor node so we can control filtering from it
            
            m_MetadataFilterProcessor = filterNode.processor;
        }

        void OnGUI()
        {
            var rect = new Rect(0,0, 200, 20);

            var categories = m_MetadataFilterProcessor.categories;

            foreach (var category in categories)
            {
                var currentVisibility = m_MetadataFilterProcessor.IsVisible(category);
                var newVisibility = GUI.Toggle(rect, currentVisibility, category);
                
                if (currentVisibility != newVisibility)
                {
                    m_MetadataFilterProcessor.SetVisibility(category, newVisibility);
                }

                rect.y += rect.height + 3;
            }
        }
    }
}
