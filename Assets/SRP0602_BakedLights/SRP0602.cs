using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class SRP0602 : RenderPipelineAsset
{
    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/Render Pipeline/SRP0602", priority = 1)]
    static void CreateSRP0602()
    {
        var instance = ScriptableObject.CreateInstance<SRP0602>();
        UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/SRP0602.asset");
    }
    #endif

    protected override RenderPipeline CreatePipeline()
    {
        return new SRP0602Instance();
    }
}

public class SRP0602Instance : RenderPipeline
{
    private static readonly ShaderTagId m_PassName = new ShaderTagId("SRP0602_Pass"); //The shader pass tag just for SRP0602
//Realtime Lights
    static int lightColorID = Shader.PropertyToID("_LightColorArray");
    static int lightDataID = Shader.PropertyToID("_LightDataArray");
    static int lightSpotDirID = Shader.PropertyToID("_LightSpotDirArray");
    private const int lightCount = 16;      
    Vector4[] lightColor = new Vector4[lightCount];
    Vector4[] lightData = new Vector4[lightCount];
    Vector4[] lightSpotDir = new Vector4[lightCount];

    public SRP0602Instance()
    {
        #if UNITY_EDITOR
        SupportedRenderingFeatures.active = new SupportedRenderingFeatures()
        {           
            //Lighting Settings - Mixed Lighting - default
            defaultMixedLightingModes =
            SupportedRenderingFeatures.LightmapMixedBakeModes.Subtractive,
            //Lighting Settings - Mixed Lighting - supported
            mixedLightingModes =
            SupportedRenderingFeatures.LightmapMixedBakeModes.Subtractive |
            SupportedRenderingFeatures.LightmapMixedBakeModes.IndirectOnly |
            SupportedRenderingFeatures.LightmapMixedBakeModes.Shadowmask,

            //Lighting Settings - Lightmapping Settings - supported
            lightmapsModes = LightmapsMode.CombinedDirectional,
            //LightmapsMode.CombinedDirectional |

            //Lighting Settings - Other Settings - Fog
            overridesFog = true,

            //Lighting Settings - Other Settings
            overridesOtherLightingSettings = true,

            //Lighting Settings - Environment
            overridesEnvironmentLighting = false,
            
            //Light Component - Mode - supported
            lightmapBakeTypes =
            LightmapBakeType.Baked |
            LightmapBakeType.Mixed,// |
            //LightmapBakeType.Realtime
            
            //MeshRenderer component
            motionVectors = false,
            receiveShadows = true,
            lightProbeProxyVolumes = true,

            //ReflectionProbe component
            reflectionProbes = true,
            reflectionProbeModes = SupportedRenderingFeatures.ReflectionProbeModes.None, //ReflectionProbeModes.Rotation

            //Material
            editableMaterialRenderQueue = true,

            //
            rendererPriority = false
        };
        
        #endif
    }

    private void SetUpRealtimeLightingVariables(ScriptableRenderContext context, CullingResults cull)
    {
        for (int i = 0; i < lightCount; i++)
        {
            lightColor[i] = Vector4.zero;
            lightData[i] = Vector4.zero;
            lightSpotDir[i] = Vector4.zero;
            if (i>=cull.visibleLights.Length)
            {
                continue;
            }

            VisibleLight light = cull.visibleLights[i];
            if (light.lightType==LightType.Directional)
            {
                lightData[i] = light.localToWorldMatrix.MultiplyVector(Vector3.back);
                lightColor[i] = light.finalColor;
                lightColor[i].w = -1; //for identifying it is a directional light in shader
            }
            else
            {
                continue;
            }
        }

        CommandBuffer cmdLight = CommandBufferPool.Get("Set-up Light Buffer");
        cmdLight.SetGlobalVectorArray(lightDataID, lightData);
        cmdLight.SetGlobalVectorArray(lightColorID, lightColor);
        cmdLight.SetGlobalVectorArray(lightSpotDirID, lightSpotDir);
        context.ExecuteCommandBuffer(cmdLight);
        CommandBufferPool.Release(cmdLight);
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
            
            //SetUp Lighting variables
            SetUpRealtimeLightingVariables(context,cull);


            //Get the setting from camera component
            bool drawSkyBox = camera.clearFlags == CameraClearFlags.Skybox? true : false;
            bool clearDepth = camera.clearFlags == CameraClearFlags.Nothing? false : true;
            bool clearColor = camera.clearFlags == CameraClearFlags.Color? true : false;

            //Camera clear flag
            CommandBuffer cmd = new CommandBuffer();
            cmd.ClearRenderTarget(clearDepth, clearColor, camera.backgroundColor);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();

            //Setup DrawSettings and FilterSettings
            var sortingSettings = new SortingSettings(camera);
            DrawingSettings drawSettings = new DrawingSettings(m_PassName, sortingSettings)
            {
                perObjectData = PerObjectData.Lightmaps | 
                                PerObjectData.LightProbe | 
                                PerObjectData.LightProbeProxyVolume |
                                PerObjectData.ReflectionProbes
            };
            FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.all);
            GraphicsSettings.useScriptableRenderPipelineBatching = false; 
            // ^if it's true it breaks the baked data

            //Skybox
            if(drawSkyBox)  {  context.DrawSkybox(camera);  }

            //Opaque objects
            sortingSettings.criteria = SortingCriteria.CommonOpaque;
            drawSettings.sortingSettings = sortingSettings;
            filterSettings.renderQueueRange = RenderQueueRange.opaque;
            context.DrawRenderers(cull, ref drawSettings, ref filterSettings);

            //Transparent objects
            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawSettings.sortingSettings = sortingSettings;
            filterSettings.renderQueueRange = RenderQueueRange.transparent;
            context.DrawRenderers(cull, ref drawSettings, ref filterSettings);

            context.Submit();
            
            EndCameraRendering(context,camera);
        }

        EndFrameRendering(context,cameras);
    }
}