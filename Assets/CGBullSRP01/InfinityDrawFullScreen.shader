Shader "Hidden/InfinityDrawFullScreen"
{
    SubShader
    {
        Tags{ "RenderPipeline" = "InfinityRenderPipeline" }
		Pass
		{
			Name"DefaultDrawFullScreen"
			ZTest Always ZWrite Off  Blend Off Cull Off

			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 4.5
				#pragma only_renderers d3d11 vulkan

				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

				Texture2D _MainTex;
				SamplerState sampler_MainTex;

				struct Attributes
				{
					float2 uv : TEXCOORD0;
					float4 vertex : POSITION;
				};

				struct Varyings
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				Varyings vert(Attributes v)
				{
					Varyings o;
					o.uv = v.uv;
					o.vertex = v.vertex;
					return o;
				}

				float4 frag(Varyings i) : SV_Target
				{
					float2 UV = i.uv.xy;
					return _MainTex.SampleLevel(sampler_MainTex, UV, 0);
				}
			ENDHLSL
		}

		Pass
		{
			Name"RenderGraphDrawFullScreen"
			ZTest Always ZWrite Off  Blend Off Cull Off

			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 4.5
				#pragma only_renderers d3d11 vulkan

				#define SAMPLE_TEXTURE2D_X_LOD(textureName, samplerName, coord2, lod) SAMPLE_TEXTURE2D_ARRAY_LOD(textureName, samplerName, coord2, 0, lod)
				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

				Texture2DArray _MainTex;
				SamplerState sampler_MainTex, sampler_PointClamp;

				struct Attributes
				{
					float2 uv : TEXCOORD0;
					float4 vertex : POSITION;
				};

				struct Varyings
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				Varyings vert(Attributes v)
				{
					Varyings o;
					o.uv = v.uv;
					o.vertex = v.vertex;
					return o;
				}

				float4 frag(Varyings i) : SV_Target
				{
					float2 UV = i.uv.xy;
					return SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_MainTex, UV, 0);
				}
			ENDHLSL
		}
    }
	Fallback Off
}