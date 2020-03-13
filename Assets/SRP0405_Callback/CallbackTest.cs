using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CallbackTest : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    void OnEnable()
    {
        SRP0405Instance.afterSkybox += MyAfterSkybox;
        SRP0405Instance.afterOpaqueObject += MyAfterOpaque;
        SRP0405Instance.afterTransparentObject += MyAfterTransparent;
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
        SRP0405Instance.afterSkybox -= MyAfterSkybox;
        SRP0405Instance.afterOpaqueObject -= MyAfterOpaque;
        SRP0405Instance.afterTransparentObject -= MyAfterTransparent;
    }

    private void MyAfterSkybox(Camera cam, ScriptableRenderContext context)
    {
        //Debug.Log("after skybox is called");
    }

    private void MyAfterOpaque(Camera cam, ScriptableRenderContext context)
    {
        //Debug.Log("after opaque is called");
    }

    public Vector3 pos=new Vector3(0,0,0);
    public Vector3 scale=new Vector3(1,1,1);
    public Vector3 rotation=new Vector3(0,0,0);
    public  Matrix4x4 Matrix {
        get {
            float radX = rotation.x * Mathf.Deg2Rad;
            float radY = rotation.y * Mathf.Deg2Rad;
            float radZ = rotation.z * Mathf.Deg2Rad;
            float sinX = Mathf.Sin(radX);
            float cosX = Mathf.Cos(radX);
            float sinY = Mathf.Sin(radY);
            float cosY = Mathf.Cos(radY);
            float sinZ = Mathf.Sin(radZ);
            float cosZ = Mathf.Cos(radZ);
			
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetColumn(0, new Vector4(
                cosY * cosZ,
                cosX * sinZ + sinX * sinY * cosZ,
                sinX * sinZ - cosX * sinY * cosZ,
                0f
            ));
            matrix.SetColumn(1, new Vector4(
                -cosY * sinZ,
                cosX * cosZ - sinX * sinY * sinZ,
                sinX * cosZ + cosX * sinY * sinZ,
                0f
            ));
            matrix.SetColumn(2, new Vector4(
                sinY,
                -sinX * cosY,
                cosX * cosY,
                0f
            ));
            matrix.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
            return matrix;
        }
    }
    private void MyAfterTransparent(Camera cam, ScriptableRenderContext context)
    {
        pos = transform.position;
        scale = transform.localScale;
        rotation = transform.eulerAngles;
        Matrix4x4 matrix;
        
        matrix = Matrix4x4.identity; //单位矩阵

        matrix.m03 = pos.x; //x平移量可以在Inspector面板拖动
        matrix.m13 = pos.y; //y轴平移量
        matrix.m23 = pos.z; //z轴平移量
        
        matrix.m00 = scale.x; //x平移量可以在Inspector面板拖动
        matrix.m11 = scale.y; //y轴平移量
        matrix.m22 = scale.z; //z轴平移量
        //Debug.Log("after transparent is called");
        CommandBuffer cmd = new CommandBuffer();
        
        cmd.DrawMesh(mesh, matrix*Matrix,material,0,0);
        context.ExecuteCommandBuffer(cmd);
    }
}
