using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SUBMesh : MonoBehaviour
{
    public Material mat;
    void Start()
    {
        #region 设置Materials
        // 程序事先设定的几个Material
        Material[] materials = new Material[] {
            mat,mat,mat,mat
        };

        this.GetComponent<MeshRenderer>().materials = materials;
        #endregion


        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        // 正四面体的顶点坐标
        Vector3[] vertices = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(Mathf.Sqrt(3)/2, 0.5f, 0),
            new Vector3(Mathf.Sqrt(3) / 6, 0.5f, Mathf.Sqrt(6) / 3)
        };

        mesh.vertices = vertices;
        mesh.subMeshCount = 4;

        int[] triangle = new int[] { 0, 1, 2 };
        mesh.SetTriangles(triangle, 0);

        triangle = new int[] { 0, 3, 1 };
        mesh.SetTriangles(triangle, 1);

        Debug.Log(mesh.subMeshCount);

        triangle = new int[] { 0, 2, 3 };
        mesh.SetTriangles(triangle, 2);

        triangle = new int[] { 1, 3, 2 };
        mesh.SetTriangles(triangle, 3);

        mesh.RecalculateNormals();
    }
    
}
