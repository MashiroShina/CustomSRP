Shader "CustomSRP/SRP0602/LightmapBaked"
{
	Properties
	{
		_MainTex ("_MainTex (RGBA)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			Tags { "LightMode" = "SRP0602_Pass" }

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "../_General/ShaderLibrary/Input/Transformation.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 lightmapUV : TEXCOORD1;
				float3 normalOS : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 lightmapUV : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float3 normalWS : NORMAL;
			};

			CBUFFER_START(UnityPerMaterial)
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _LightColorArray[16];
				float4 _LightDataArray[16];
				float4 _LightSpotDirArray[16];
			CBUFFER_END
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.lightmapUV = v.lightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
				o.normalWS = TransformObjectToWorldNormal(v.normalOS);
				return o;
			}
			float3 ShadeDirectionalLight(float3 normalWS, float3 albedo, float3 lightDirectionWS, float3 lightColor)
            {
                float attenuation = 5.0;
                float n_dot_l = max(dot(normalWS, lightDirectionWS), 0.0);
                return albedo * lightColor * n_dot_l * attenuation;
            }
            float4 CalculateLight(v2f IN, int i, float4 albedo)
			{
				if (_LightColorArray[i].w == -1) //-1 is directional
				{
					albedo.rgb += ShadeDirectionalLight(IN.normalWS, albedo.rgb, _LightDataArray[i].xyz, _LightColorArray[i].rgb);
				}

				return albedo;
			}
			float4 frag (v2f IN) : SV_Target
			{
				float4 lightmap = SAMPLE_TEXTURE2D(unity_Lightmap, samplerunity_Lightmap, IN.lightmapUV);
				float4 col = tex2D(_MainTex, IN.uv) * _Color;
                float4 albedo = tex2D(_MainTex, IN.uv) * _Color;
                
                                for (int id = 0; id < min(unity_LightData.y,4); id++) 
                                {
                                    int i = unity_LightIndices[0][id];
                                    albedo = CalculateLight(IN, i,albedo);
                                }
                
                                for (int id2 = 4; id2 < min(unity_LightData.y,8); id2++) 
                                {
                                    int i = unity_LightIndices[1][id2-4];
                                    albedo = CalculateLight(IN, i,albedo);
                                }                                
                if(lightmap.r == 0){
                 return albedo;
                }
               
				return col * lightmap;
			}
			ENDHLSL
		}
	}
}
