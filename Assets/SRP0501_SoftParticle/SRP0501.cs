using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

[ExecuteInEditMode]
public class SRP0501 : RenderPipelineAsset
{
    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/Render Pipeline/SRP0501", priority = 1)]
    static void CreateSRP0501()
    {
        var instance = ScriptableObject.CreateInstance<SRP0501>();
        UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/SRP0501.asset");
    }
    #endif

    protected override RenderPipeline CreatePipeline()
    {
        return new SRP0501Instance();
    }
}

public class SRP0501Instance : RenderPipeline
{
    private static readonly ShaderTagId m_PassName = new ShaderTagId("SRP0501_Pass"); //The shader pass tag just for SRP0501

    private static Material depthOnlyMaterial;
    private static int m_DepthRTid = Shader.PropertyToID("_CameraDepthTexture");
    private static RenderTargetIdentifier m_DepthRT = new RenderTargetIdentifier(m_DepthRTid);
    private int depthBufferBits = 24;

    
    
    //Custom callbacks
    public static event Action<Camera,ScriptableRenderContext> afterSkybox;
    public static event Action<Camera,ScriptableRenderContext> afterOpaqueObject;
    public static event Action<Camera,ScriptableRenderContext,RenderTargetIdentifier,RenderTextureDescriptor> afterTransparentObject;
    public static event Action<Camera,ScriptableRenderContext,RenderTargetIdentifier,RenderTextureDescriptor,CommandBuffer> myDebug;
    public SRP0501Instance()
    {
        depthOnlyMaterial = new Material(Shader.Find("Hidden/CustomSRP/SRP0501/DepthOnly"));
    }
    public static void AfterSkybox(Camera camera, ScriptableRenderContext context)
    {
        afterSkybox?.Invoke(camera,context);
    }

    public static void AfterOpaqueObject(Camera camera, ScriptableRenderContext context)
    {
        afterOpaqueObject?.Invoke(camera,context);
    }

    public static void AfterTransparentObject(Camera camera, ScriptableRenderContext context,RenderTargetIdentifier RTid,RenderTextureDescriptor Desc)
    {
        afterTransparentObject?.Invoke(camera,context,RTid,Desc);
    }
    public static void MyDebug(Camera camera, ScriptableRenderContext context,RenderTargetIdentifier RTid,RenderTextureDescriptor Desc,CommandBuffer cmd)
    {
        myDebug?.Invoke(camera,context,RTid,Desc,cmd);
    }
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        BeginFrameRendering(context,cameras);

        foreach (Camera camera in cameras)
        {
            BeginCameraRendering(context,camera);

            //Culling
            ScriptableCullingParameters cullingParams;
            if (!camera.TryGetCullingParameters(out cullingParams))
                continue;
            CullingResults cull = context.Cull(ref cullingParams);

            //Camera setup some builtin variables e.g. camera projection matrices etc
            context.SetupCameraProperties(camera);

            //Get the setting from camera component
            bool drawSkyBox = camera.clearFlags == CameraClearFlags.Skybox? true : false;
            bool clearDepth = camera.clearFlags == CameraClearFlags.Nothing? false : true;
            bool clearColor = camera.clearFlags == CameraClearFlags.Color? true : false;

            //Set Depth texture temp RT
            CommandBuffer cmdTempId = new CommandBuffer();
            cmdTempId.name = "("+camera.name+")"+ "Setup TempRT";
            RenderTextureDescriptor depthRTDesc = new RenderTextureDescriptor(camera.pixelWidth, camera.pixelHeight);
                depthRTDesc.colorFormat = RenderTextureFormat.Depth;
                depthRTDesc.depthBufferBits = depthBufferBits;
                cmdTempId.GetTemporaryRT(m_DepthRTid, depthRTDesc,FilterMode.Bilinear);
            context.ExecuteCommandBuffer(cmdTempId);
            cmdTempId.Release();

            //Setup DrawSettings and FilterSettings
            var sortingSettings = new SortingSettings(camera);
            DrawingSettings drawSettings = new DrawingSettings(m_PassName, sortingSettings);
            FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.all);
            DrawingSettings drawSettingsDepth = new DrawingSettings(m_PassName, sortingSettings)
            {
                perObjectData = PerObjectData.None,
                overrideMaterial = depthOnlyMaterial,
                overrideMaterialPassIndex = 0
            };

            //Clear Depth Textureß
            CommandBuffer cmdDepth = new CommandBuffer();
            cmdDepth.name = "("+camera.name+")"+ "Depth Clear Flag";
            cmdDepth.SetRenderTarget(m_DepthRT); //Set CameraTarget to the depth texture
         
            cmdDepth.ClearRenderTarget(false, true, Color.green);
            //debug
            MyDebug(camera, context,m_DepthRT,depthRTDesc,null);
            context.ExecuteCommandBuffer(cmdDepth);
            cmdDepth.Release();

            //Draw Depth with Opaque objects
            sortingSettings.criteria = SortingCriteria.CommonOpaque;
            drawSettingsDepth.sortingSettings = sortingSettings;
            filterSettings.renderQueueRange = RenderQueueRange.opaque;
            context.DrawRenderers(cull, ref drawSettingsDepth, ref filterSettings);

           
            
            //To let shader has _CameraDepthTexture
            CommandBuffer cmdDepthTexture = new CommandBuffer();
            cmdDepthTexture.name = "("+camera.name+")"+ "Depth Texture";
            cmdDepthTexture.SetGlobalTexture(m_DepthRTid,m_DepthRT);

            context.ExecuteCommandBuffer(cmdDepthTexture);
            cmdDepthTexture.Release();

            //Camera clear flag
            CommandBuffer cmd = new CommandBuffer();
            cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget); //Rember to reset target
            cmd.ClearRenderTarget(clearDepth, clearColor, camera.backgroundColor);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();

            //Skybox
            if(drawSkyBox)  {  context.DrawSkybox(camera);  }

            AfterSkybox(camera, context);
            //Opaque objects
            sortingSettings.criteria = SortingCriteria.CommonOpaque;
            drawSettings.sortingSettings = sortingSettings;
            filterSettings.renderQueueRange = RenderQueueRange.opaque;
            context.DrawRenderers(cull, ref drawSettings, ref filterSettings);
            AfterOpaqueObject(camera, context);
            //Transparent objects
            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawSettings.sortingSettings = sortingSettings;
            filterSettings.renderQueueRange = RenderQueueRange.transparent;
            context.DrawRenderers(cull, ref drawSettings, ref filterSettings);
           
            //Clean Up
            CommandBuffer cmdclean = new CommandBuffer();
            cmdclean.name = "("+camera.name+")"+ "Clean Up";
            cmdclean.ReleaseTemporaryRT(m_DepthRTid);
            context.ExecuteCommandBuffer(cmdclean);
            cmdclean.Release();

            context.Submit();
            
            EndCameraRendering(context,camera);
        }

        EndFrameRendering(context,cameras);
    }
}