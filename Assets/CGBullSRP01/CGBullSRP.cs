using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace InfinityExtend.Rendering.Runtime
{
    [ExecuteInEditMode]
    public class CGBullSRP : RenderPipelineAsset
    {
        [UnityEditor.MenuItem("Assets/Create/Render Pipeline/CGBullSRP01", priority = 1)]
        static void CreateSRP0101()
        {
            var instance = ScriptableObject.CreateInstance<CGBullSRP>();
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/CGBullSRP0101.asset");
        }

        protected override RenderPipeline CreatePipeline()
        {
            return new CGBullSRPInst();
        }
        protected override void OnValidate() {
            //Fixed
        }

        protected override void OnDisable() {

        }
    }

    public class CGBullSRPInst : RenderPipeline
    {
        private static readonly ShaderTagId m_PassName = new ShaderTagId("CGBullSRP_Pass");

        public CGBullSRPInst()
        {
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            BeginFrameRendering(context, cameras);
            foreach (var RenderCamera in cameras)
            {
#if UNITY_EDITOR
                //seting UI in SceneEdit
                bool isSceneViewCam = RenderCamera.cameraType == CameraType.SceneView;
                if (isSceneViewCam)
                {
                    ScriptableRenderContext.EmitWorldGeometryForSceneView(RenderCamera);
                }
#endif

                //////Set Camera Property
                BeginCameraRendering(context, RenderCamera);
                context.SetupCameraProperties(RenderCamera);

                /////Cull Object
                ScriptableCullingParameters cullingParameters;
                if (!RenderCamera.TryGetCullingParameters(out cullingParameters))
                {
                    continue;
                }

                CullingResults CullingData = context.Cull(ref cullingParameters);

                //Drwa PipeLine Pass
                RenderingContent(isSceneViewCam, RenderCamera, CullingData, context);

                ///Submit RenderContent
                context.Submit();
                EndCameraRendering(context, RenderCamera);
            }
        }

        private void RenderingContent(bool isSceneViewCam, Camera RenderCamera, CullingResults CullingData,
            ScriptableRenderContext RenderContent)
        {
            //////Draw PrePass
            ///设置FilterSetting以及DrawSetting
            FilteringSettings FilterSetting_My_PrePass = new FilteringSettings
            {
                renderingLayerMask = 1,
                layerMask = RenderCamera.cullingMask,
                renderQueueRange = RenderQueueRange.opaque
            };
            DrawingSettings DrawSetting_My_PrePass = new DrawingSettings(
                InfinityPassIDs.PreDepthPass, new SortingSettings(RenderCamera)
                    {criteria = SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges})
            {
                enableInstancing = true,
                enableDynamicBatching = true,
                perObjectData = PerObjectData.Lightmaps
            };
            ///从CommandBufferPool来获得池化的CB并申请TemporalRT设置RenderTarget和ShaderPass后进行DrawRender
            CommandBuffer CommandList_PrePass = CommandBufferPool.Get(InfinityPassIDs.PreDepthPass.name);
            CommandList_PrePass.GetTemporaryRT(InfinityShaderIDs.RT_DepthBuffer, RenderCamera.pixelWidth, RenderCamera.pixelHeight, 24, FilterMode.Point, RenderTextureFormat.Depth);
            CommandList_PrePass.SetRenderTarget(InfinityShaderIDs.ID_SceneDepth);
            CommandList_PrePass.ClearRenderTarget(true, true, RenderCamera.backgroundColor);
            RenderContent.ExecuteCommandBuffer(CommandList_PrePass);
            CommandBufferPool.Release(CommandList_PrePass);
            RenderContent.DrawRenderers(CullingData, ref DrawSetting_My_PrePass, ref FilterSetting_My_PrePass);


            //////Draw GBufferPass
            FilteringSettings FilterSetting_My_GBuffer = new FilteringSettings
            {
                renderingLayerMask = 1,
                layerMask = RenderCamera.cullingMask,
                renderQueueRange = RenderQueueRange.opaque,
            };
            DrawingSettings DrawSetting_My_GBuffer = new DrawingSettings(InfinityPassIDs.GBufferPass, new SortingSettings(RenderCamera) { criteria = SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges })
            {
                enableInstancing = true,
                enableDynamicBatching = true,
                perObjectData = PerObjectData.Lightmaps
            };
            CommandBuffer CommandList_GBufferPass = CommandBufferPool.Get(InfinityPassIDs.GBufferPass.name);
            CommandList_GBufferPass.GetTemporaryRT(InfinityShaderIDs.RT_GBufferBaseColor, RenderCamera.pixelWidth, RenderCamera.pixelHeight, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
            CommandList_GBufferPass.GetTemporaryRT(InfinityShaderIDs.RT_GBufferMicroface, RenderCamera.pixelWidth, RenderCamera.pixelHeight, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
            CommandList_GBufferPass.GetTemporaryRT(InfinityShaderIDs.RT_GBufferNormal, RenderCamera.pixelWidth, RenderCamera.pixelHeight, 0, FilterMode.Point, RenderTextureFormat.ARGB2101010);
            CommandList_GBufferPass.GetTemporaryRT(InfinityShaderIDs.RT_GBufferEmissive, RenderCamera.pixelWidth, RenderCamera.pixelHeight, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
            CommandList_GBufferPass.SetRenderTarget(InfinityShaderIDs.ID_GBuffer, InfinityShaderIDs.ID_SceneDepth);
            CommandList_PrePass.ClearRenderTarget(false, true, RenderCamera.backgroundColor);
            RenderContent.ExecuteCommandBuffer(CommandList_GBufferPass);
            CommandBufferPool.Release(CommandList_GBufferPass);
            RenderContent.DrawRenderers(CullingData, ref DrawSetting_My_GBuffer, ref FilterSetting_My_GBuffer);

            //////Draw SkyBox
            RenderContent.DrawSkybox(RenderCamera);

            //////Draw Gizmos on SceneView 
#if UNITY_EDITOR
            if (isSceneViewCam)
            {
                RenderContent.DrawGizmos(RenderCamera, GizmoSubset.PostImageEffects);
            }
#endif

            //////Bilt To Screen
            CommandBuffer CommandList_BiltToCamera = CommandBufferPool.Get("InfinityCommandList");
            bool IsGameView; //检测当前View是不是GameView
            if (RenderCamera.targetTexture != null)
            {
                IsGameView = false;
            }
            else
            {
                IsGameView = true;
            }

            CommandList_BiltToCamera.DrawFullScreen(IsGameView, InfinityShaderIDs.RT_GBufferBaseColor,
                RenderCamera.targetTexture);
            ///释放所有RT
            CommandList_BiltToCamera.ReleaseTemporaryRT(InfinityShaderIDs.RT_DepthBuffer);
            CommandList_BiltToCamera.ReleaseTemporaryRT(InfinityShaderIDs.RT_GBufferBaseColor);
            CommandList_BiltToCamera.ReleaseTemporaryRT(InfinityShaderIDs.RT_GBufferMicroface);
            CommandList_BiltToCamera.ReleaseTemporaryRT(InfinityShaderIDs.RT_GBufferNormal);
            CommandList_BiltToCamera.ReleaseTemporaryRT(InfinityShaderIDs.RT_GBufferEmissive);
            RenderContent.ExecuteCommandBuffer(CommandList_BiltToCamera);
            CommandBufferPool.Release(CommandList_BiltToCamera);
        }
    }
}
