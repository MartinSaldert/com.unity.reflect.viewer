using System;
using System.Collections.Generic;

namespace UnityEngine.Reflect.Pipeline.Samples
{
    public enum MetadataParameterComparisonType
    {
        All, 
        Any
    }
    
    public interface IMetadataFilterNodeSettings //: IReflectPipelineSettings
    {
        MetadataParameterComparisonType metadataParameterComparisonType { get; }
        List<Metadata.Parameter> parameters { get; }
    }
    
    [Serializable]
    public class MetadataFilterNodeSettings : IMetadataFilterNodeSettings
    {
        [SerializeField] MetadataParameterComparisonType m_MetadataParameterComparisonType = MetadataParameterComparisonType.Any;
        [SerializeField] List<Metadata.Parameter> m_Parameters = null;

        public MetadataParameterComparisonType metadataParameterComparisonType => m_MetadataParameterComparisonType;
        public List<Metadata.Parameter> parameters => m_Parameters;
        public IExposedPropertyTable exposedPropertyTable { private get; set; }
    }
}
