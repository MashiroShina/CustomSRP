using UnityEngine;
using UnityEngine.Rendering;

namespace InfinityExtend.Rendering.Runtime
{
    public static class InfinityGraphicsUtillity
    {
        public static Material BlitMaterial
        {
            get
            {
                return new Material(Shader.Find("Hidden/InfinityDrawFullScreen"));
            }
        }

        public static Mesh FullScreenMeshOrigin_Game;

        public static Mesh FullScreenMeshOrigin_Scene;

        public static Mesh FullScreenMesh_Game
        {
            get
            {
                if (FullScreenMeshOrigin_Game != null) {
                    return FullScreenMeshOrigin_Game;
                }

                FullScreenMeshOrigin_Game = new Mesh { name = "FullScreen Mesh" };

                FullScreenMeshOrigin_Game.vertices = new Vector3[] {
                    new Vector3(-1, -1, 0),
                    new Vector3(-1,  1, 0),
                    new Vector3( 1,  1, 0),
                    new Vector3( 1, -1, 0)
                };

                FullScreenMeshOrigin_Game.uv = new Vector2[] {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                };

                FullScreenMeshOrigin_Game.SetIndices(new int[] { 0, 1, 2, 3 }, MeshTopology.Quads, 0, false);
                FullScreenMeshOrigin_Game.UploadMeshData(false);
                return FullScreenMeshOrigin_Game;
            }
        }

        public static Mesh FullScreenMesh_Scene
        {
            get
            {
                if (FullScreenMeshOrigin_Scene != null)
                {
                    return FullScreenMeshOrigin_Scene;
                }

                FullScreenMeshOrigin_Scene = new Mesh { name = "FullScreen Mesh" };

                FullScreenMeshOrigin_Scene.vertices = new Vector3[] {
                    new Vector3(-1, -1, 0),
                    new Vector3(-1,  1, 0),
                    new Vector3( 1,  1, 0),
                    new Vector3( 1, -1, 0)
                };

                FullScreenMeshOrigin_Scene.uv = new Vector2[] {
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1)
                };

                FullScreenMeshOrigin_Scene.SetIndices(new int[] { 0, 1, 2, 3 }, MeshTopology.Quads, 0, false);
                FullScreenMeshOrigin_Scene.UploadMeshData(false);
                return FullScreenMeshOrigin_Scene;
            }
        }

        public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier colorAttachment, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, ClearFlag clearFlags, Color clearColor, TextureDimension dimension)
        {
            if (dimension == TextureDimension.Tex2DArray) {
                CoreUtils.SetRenderTarget(cmd, colorAttachment, clearFlags, clearColor, 0, CubemapFace.Unknown, -1);
            } else {
                CoreUtils.SetRenderTarget(cmd, colorAttachment, colorLoadAction, colorStoreAction, clearFlags, clearColor);
            }   
        }

        public static void DrawFullScreenRenderGraph(this CommandBuffer CommandList, bool IsGameView, RTHandle Source, RenderTargetIdentifier Desc)
        {
            CommandList.SetRenderTarget(Desc);
            //SetRenderTarget(CommandList, Desc, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, ClearFlag.None, Color.black, TextureDimension.Tex2DArray);
            CommandList.SetGlobalTexture(InfinityShaderIDs.RT_MainTexture, Source);
            CommandList.DrawMesh(IsGameView ? FullScreenMesh_Game : FullScreenMesh_Scene, Matrix4x4.identity, BlitMaterial, 0, 1);
        }

        public static void DrawFullScreenRenderGraph(this CommandBuffer CommandList, bool IsGameView, RTHandle Source, RenderTargetIdentifier Desc, MaterialPropertyBlock MaterialPropertyBlock = null)
        {
            CommandList.SetRenderTarget(Desc);
            MaterialPropertyBlock.SetTexture(InfinityShaderIDs.RT_MainTexture, Source);
            Debug.Log(IsGameView);
            CommandList.DrawMesh(IsGameView ? FullScreenMesh_Game : FullScreenMesh_Scene, Matrix4x4.identity, BlitMaterial, 0, 1, MaterialPropertyBlock);
        }

        public static void DrawFullScreen(this CommandBuffer CommandList, bool IsGameView, RenderTargetIdentifier Source, RenderTargetIdentifier Desc)
        {
            CommandList.SetRenderTarget(Desc);
            CommandList.SetGlobalTexture(InfinityShaderIDs.RT_MainTexture, Source);
            CommandList.DrawMesh(IsGameView ? FullScreenMesh_Game : FullScreenMesh_Scene, Matrix4x4.identity, BlitMaterial, 0, 0);
        }

        public static void DrawFullScreen(this CommandBuffer CommandList, RenderTargetIdentifier Source, RenderTargetIdentifier Desc)
        {
            CommandList.SetRenderTarget(Desc);
            CommandList.SetGlobalTexture(InfinityShaderIDs.RT_MainTexture, Source);
            CommandList.DrawMesh(FullScreenMesh_Game, Matrix4x4.identity, BlitMaterial, 0, 0);
        }

        public static void DrawFullScreen(this CommandBuffer CommandList, RenderTargetIdentifier ColorBuffer, Material DrawMaterial, int DrawPass)
        {
            CommandList.SetRenderTarget(ColorBuffer);
            CommandList.DrawMesh(FullScreenMesh_Game, Matrix4x4.identity, DrawMaterial, 0, DrawPass);
        }

        public static void DrawFullScreen(this CommandBuffer CommandList, RenderTargetIdentifier ColorBuffer, Material DrawMaterial, int DrawPass, MaterialPropertyBlock MaterialPropertyBlock = null)
        {
            CommandList.SetRenderTarget(ColorBuffer);
            CommandList.DrawMesh(FullScreenMesh_Game, Matrix4x4.identity, DrawMaterial, 0, DrawPass, MaterialPropertyBlock);
        }

        public static void DrawFullScreen(this CommandBuffer CommandList, RenderTargetIdentifier ColorBuffer, RenderTargetIdentifier DepthBuffer, Material DrawMaterial, int DrawPass)
        {
            CommandList.SetRenderTarget(ColorBuffer, DepthBuffer);
            CommandList.DrawMesh(FullScreenMesh_Game, Matrix4x4.identity, DrawMaterial, 0, DrawPass);
        }

        public static void DrawFullScreen(this CommandBuffer CommandList, RenderTargetIdentifier ColorBuffer, RenderTargetIdentifier DepthBuffer, Material DrawMaterial, int DrawPass, MaterialPropertyBlock MaterialPropertyBlock = null)
        {
            CommandList.SetRenderTarget(ColorBuffer, DepthBuffer);
            CommandList.DrawMesh(FullScreenMesh_Game, Matrix4x4.identity, DrawMaterial, 0, DrawPass, MaterialPropertyBlock);
        }

        public static void DrawFullScreen(this CommandBuffer CommandList, RenderTargetIdentifier[] ColorBuffers, RenderTargetIdentifier DepthBuffer, Material DrawMaterial, int DrawPass)
        {
            CommandList.SetRenderTarget(ColorBuffers, DepthBuffer);
            CommandList.DrawMesh(FullScreenMesh_Game, Matrix4x4.identity, DrawMaterial, 0, DrawPass);
        }

        public static void DrawFullScreen(this CommandBuffer CommandList, RenderTargetIdentifier[] ColorBuffers, RenderTargetIdentifier DepthBuffer, Material DrawMaterial, int DrawPass, MaterialPropertyBlock MaterialPropertyBlock = null)
        {
            CommandList.SetRenderTarget(ColorBuffers, DepthBuffer);
            CommandList.DrawMesh(FullScreenMesh_Game, Matrix4x4.identity, DrawMaterial, 0, DrawPass, MaterialPropertyBlock);
        }

        public static Rect GetViewport(Camera RenderCamera)
        {
            return new Rect(RenderCamera.pixelRect.x, RenderCamera.pixelRect.y, RenderCamera.pixelWidth, RenderCamera.pixelHeight);
        }
    }
}