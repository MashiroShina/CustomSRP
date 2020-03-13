using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class MDebug : MonoBehaviour
{
    void OnEnable()
    {
        SRP0701Instance.afterSkybox += MyAfterSkybox;
        SRP0701Instance.afterOpaqueObject += MyAfterOpaque;
        SRP0701Instance.afterTransparentObject += MyAfterTransparent;
        SRP0701Instance.myDebug += MyDebug;
        SRP0501Instance.afterSkybox += MyAfterSkybox;
        SRP0501Instance.afterOpaqueObject += MyAfterOpaque;
        SRP0501Instance.afterTransparentObject += MyAfterTransparent;
        SRP0501Instance.myDebug += MyDebug;
        SRP0702Instance.afterSkybox += MyAfterSkybox;
        SRP0702Instance.afterOpaqueObject += MyAfterOpaque;
        SRP0702Instance.afterTransparentObject += MyAfterTransparent;
        SRP0702Instance.myDebug += MyDebug;
    }

    void OnDisable()
    {
        CleanUp();
    }

    void OnDestroy()
    {
        CleanUp();
    }

    private void CleanUp()
    {
        SRP0702Instance.myDebug -= MyDebug;
        SRP0702Instance.afterSkybox -= MyAfterSkybox;
        SRP0702Instance.afterOpaqueObject -= MyAfterOpaque;
        SRP0702Instance.afterTransparentObject -= MyAfterTransparent;
        
        SRP0701Instance.myDebug -= MyDebug;
        SRP0701Instance.afterSkybox -= MyAfterSkybox;
        SRP0701Instance.afterOpaqueObject -= MyAfterOpaque;
        SRP0701Instance.afterTransparentObject -= MyAfterTransparent;
        
        SRP0501Instance.afterSkybox -= MyAfterSkybox;
        SRP0501Instance.afterOpaqueObject -= MyAfterOpaque;
        SRP0501Instance.afterTransparentObject -= MyAfterTransparent;
        SRP0501Instance.myDebug -= MyDebug;
    }

    private void MyAfterSkybox(Camera cam, ScriptableRenderContext context)
    {
        //Debug.Log("after skybox is called");
    }

    private void MyAfterOpaque(Camera cam, ScriptableRenderContext context)
    {
        //Debug.Log("after opaque is called");
    }



    private void MyAfterTransparent(Camera camera, ScriptableRenderContext context,RenderTargetIdentifier RTid,RenderTextureDescriptor Desc)
    {
    }
    private RenderTexture debugRT;
    private void MyDebug(Camera camera, ScriptableRenderContext context,RenderTargetIdentifier RTid,RenderTextureDescriptor Desc,CommandBuffer cmd=null) 
    {
        if (cmd!=null)
        {
            cmd.name = cmd.name+"HaveCMD!!!!!!!!!!!!!!";
            debugRT=new RenderTexture(Desc);
            cmd.Blit(RTid,debugRT);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
            debugRT.Release();
        }
        else
        {
            CommandBuffer cmdTempId = new CommandBuffer();
            cmdTempId.name = "("+camera.name+")"+ "Setup TempRT";
            debugRT=new RenderTexture(Desc);
            cmdTempId.Blit(RTid,debugRT);
            context.ExecuteCommandBuffer(cmdTempId);
            cmdTempId.Release();
            debugRT.Release();  
        }
       
        
    }
    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), debugRT, ScaleMode.ScaleToFit, false, 1);
    }
}
