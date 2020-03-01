Shader "Unlit/MUnlit"
{
   Properties {
        [Header (Microface)]
        [Toggle (_UseAlbedoTex)]UseBaseColorTex ("UseBaseColorTex", Range(0, 1)) = 0
        [NoScaleOffset]_MainTex ("BaseColorTexture", 2D) = "white" {}
        _BaseColorTile ("BaseColorTile", Range(0, 100)) = 1
        _BaseColor ("BaseColor", Color) = (1, 1, 1, 1)
        _SpecularLevel ("SpecularLevel", Range(0, 1)) = 0.5
        _Reflectance ("Reflectance", Range(0, 1)) = 0
        _Roughness ("Roughness", Range(0, 1)) = 0


        [Header (Normal)]
        [NoScaleOffset]_NomralTexture ("NomralTexture", 2D) = "bump" {}
        _NormalTile ("NormalTile", Range(0, 100)) = 1


        [Header (Iridescence)]
        [Toggle (_Iridescence)] Iridescence ("Iridescence", Range(0, 1)) = 0
        _Iridescence_Distance ("Iridescence_Distance", Range(0, 1)) = 1

		[Header(PixelDepthOffset)]
        _PixelDepthOffsetVaule ("PixelDepthOffsetVaule", Range(-1, 1)) = 0
        
	}
	SubShader
	{
		Tags{ "RenderPipeline" = "InfinityRenderPipeline" "IgnoreProjector" = "True" "RenderType" = "InfinityStandar" }
		LOD 100

		Pass
		{
			Name "PrePass"
			Tags { "LightMode" = "Infinity_PrePass" }
			ZTest LEqual ZWrite On Cull Back
			ColorMask 0 

			HLSLPROGRAM
			#pragma target 4.5
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			//#include "../Private/VertexFactory.hlsl"
			//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

			struct Attributes
			{
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
			//	UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			//	UNITY_VERTEX_INPUT_INSTANCE_ID
			};
            CBUFFER_START(UnityPerFrame)
				float4x4 unity_MatrixVP;
			CBUFFER_END

			CBUFFER_START(UnityPerDraw)
				float4x4 unity_ObjectToWorld;
				float4x4 unity_WorldToObject;
				float4 unity_LODFade;
				float4 unity_WorldTransformParams;
				float4 unity_RenderingLayer;
			CBUFFER_END
			Varyings vert(Attributes In)
			{
				Varyings Out;
				Out.uv = In.uv;
				float4 WorldPos = mul(unity_ObjectToWorld, float4(In.vertex.xyz, 1.0));
				Out.vertex = mul(unity_MatrixVP, WorldPos);
				return Out;
			}

			float4 frag(Varyings In) : SV_Target
			{
				/*UNITY_SETUP_INSTANCE_ID(In);
				if (In.uv.x < 0.5) {
					discard;
				}*/
				return 0;
			}
			ENDHLSL
		}

		Pass
		{
			Name "GBufferPass"
			Tags { "LightMode" = "Infinity_GBufferPass" }
			ZTest Equal ZWrite Off Cull Back

			HLSLPROGRAM
			#pragma target 4.5
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			//#include "../Private/VertexFactory.hlsl"
			//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"


			Texture2D _MainTex; SamplerState sampler_MainTex;
			CBUFFER_START(UnityPerMaterial)
				float _PixelDepthOffsetVaule;
			CBUFFER_END

			CBUFFER_START(UnityPerFrame)
				float4x4 unity_MatrixVP;
			CBUFFER_END

			CBUFFER_START(UnityPerDraw)
				float4x4 unity_ObjectToWorld;
				float4x4 unity_WorldToObject;
				float4 unity_LODFade; 
				float4 unity_WorldTransformParams; 
				float4 unity_RenderingLayer;
			CBUFFER_END
			/*CBUFFER_START(UnityPerMaterial)
				float _PixelDepthOffsetVaule;
			CBUFFER_END*/
			
			struct MeshData
			{
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 vertex : POSITION;
			//	UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PixelData
			{
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float4 vertex : SV_POSITION;
				//UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			PixelData vert (MeshData In)
			{
				PixelData Out;
				Out.uv = In.uv;
				Out.normal = normalize(mul(In.normal, (float3x3)unity_WorldToObject));
				Out.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, float4(In.vertex.xyz, 1.0)));
				return Out;
			}
			
			void frag (PixelData In, out float4 BaseColor : SV_Target0, out float4 Microface : SV_Target1, out float4 WorldNormal : SV_Target2, out float4 Emissive : SV_Target3)
			{
				BaseColor = _MainTex.Sample(sampler_MainTex, In.uv);
				Microface = float4(BaseColor.b, BaseColor.r, BaseColor.g, 1.0);
				WorldNormal = float4(In.normal, 1.0);
				Emissive = float4(1.0, 0.5, 0.1, 1.0);
			}
			ENDHLSL
		}
    }
}
