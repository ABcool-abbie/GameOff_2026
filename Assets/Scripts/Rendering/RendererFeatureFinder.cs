using System.Reflection;
using UnityEngine.Rendering.Universal;

namespace AnythingWorksForMe.Rendering
{
    internal static class RendererFeatureFinder<T> where T : ScriptableRendererFeature
    {
        private static readonly FieldInfo RendererDataListField =
            typeof(UniversalRenderPipelineAsset).GetField(
                "m_RendererDataList",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static T cachedFeature;
        private static UniversalRenderPipelineAsset cachedAsset;

        public static T Find()
        {
            UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
            if (asset == null)
                return null;

            if (cachedFeature != null && cachedAsset == asset)
                return cachedFeature;

            cachedAsset = asset;
            cachedFeature = null;

            if (RendererDataListField?.GetValue(asset) is ScriptableRendererData[] rendererDataList)
            {
                for (int i = 0; i < rendererDataList.Length; i++)
                {
                    ScriptableRendererData data = rendererDataList[i];
                    if (data == null || data.rendererFeatures == null)
                        continue;

                    for (int j = 0; j < data.rendererFeatures.Count; j++)
                    {
                        if (data.rendererFeatures[j] is T feature)
                        {
                            cachedFeature = feature;
                            return feature;
                        }
                    }
                }
            }

            return null;
        }
    }
}
