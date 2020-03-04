using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class te : RenderPipeline
{private static readonly ShaderTagId m_PassName = new ShaderTagId("SRP0102_Pass");
   protected override void Render(ScriptableRenderContext context, Camera[] cameras)
   {
      BeginFrameRendering(context,cameras);
      foreach (var cam in cameras)
      {
         BeginCameraRendering(context,cam);

         ScriptableCullingParameters cullpar;
         if (!cam.TryGetCullingParameters(out cullpar))
         {
            continue;
         }

         var cull = context.Cull(ref cullpar);
         
         context.SetupCameraProperties(cam);
         
         CommandBuffer cmd=new CommandBuffer();
         context.ExecuteCommandBuffer(cmd);
         cmd.Release();

         var sortsetting=new SortingSettings(cam);
         DrawingSettings dw=new DrawingSettings(m_PassName,sortsetting);
         FilteringSettings flit=new FilteringSettings(RenderQueueRange.all);

         if (true)
         {
            sortsetting.criteria = SortingCriteria.CommonOpaque;
            dw.sortingSettings = sortsetting;
            flit.layerMask = cam.cullingMask;
            flit.renderingLayerMask = 1;
            flit.renderQueueRange=RenderQueueRange.opaque;
         }
         context.Submit();
         EndCameraRendering(context,cam);
      }
      EndFrameRendering(context,cameras);
   }
}
