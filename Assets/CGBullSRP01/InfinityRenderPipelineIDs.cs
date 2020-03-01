using UnityEngine;
using UnityEngine.Rendering;

namespace InfinityExtend.Rendering.Runtime
{
    public static class InfinityShaderIDs
    {
        public static int RT_DepthBuffer = Shader.PropertyToID("_GBufferDepth");
        public static RenderTargetIdentifier ID_SceneDepth = new RenderTargetIdentifier(RT_DepthBuffer);
        public static int RT_GBufferBaseColor = Shader.PropertyToID("_GBufferBaseColor");
        public static int RT_GBufferMicroface = Shader.PropertyToID("_GBufferMicroface");
        public static int RT_GBufferNormal = Shader.PropertyToID("_GBufferNormal");
        public static int RT_GBufferEmissive = Shader.PropertyToID("_GBufferEmissive");
        public static RenderTargetIdentifier[] ID_GBuffer = {RT_GBufferBaseColor, RT_GBufferMicroface, RT_GBufferNormal, RT_GBufferEmissive};

        public static int RT_MainTexture = Shader.PropertyToID("_MainTex");
        public static int RT_BlitSourceTexture = Shader.PropertyToID("_BlitSourceTexture");
    }
    
    public static class InfinityPassIDs {
        public static ShaderTagId PreDepthPass = new ShaderTagId("Infinity_Prepass");
        public static ShaderTagId GBufferPass = new ShaderTagId("Infinity_GBufferPass");
    }

    public static class InfinityRenderQueue
    {
        public enum Priority
        {
            Background = UnityEngine.Rendering.RenderQueue.Background,
            OpaqueLast = UnityEngine.Rendering.RenderQueue.GeometryLast,
        }
        public static readonly RenderQueueRange k_RenderQueue_AllOpaque = new RenderQueueRange { lowerBound = (int)Priority.Background, upperBound = (int)Priority.OpaqueLast };
    }
}