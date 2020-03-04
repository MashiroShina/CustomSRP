#ifndef FurHelper
#define FurHelper
#pragma target 3.0

            
            #include "UnityCG.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
			#include "Lighting.cginc" 
			#include "AutoLight.cginc"

struct v2f
{
    float4 pos: SV_POSITION;
    half4 uv: TEXCOORD0;
    float3 worldNormal: TEXCOORD1;
    float3 worldPos: TEXCOORD2;
    SHADOW_COORDS(3)
};

CBUFFER_START(UnityPerMaterial)

    fixed4 _Color;
    fixed4 _Specular;
    half _FurShininess;
    half _ShadowRange;
    half _WindAmplitude;
    half _WindFrequency;
    half _WindDistribution;
    fixed _Cutoff;
    
    sampler2D _MainTex;
    half4 _MainTex_ST;
    sampler2D _FurTex;
    half4 _FurTex_ST;
    
    fixed _FurLength;
    fixed _FurDensity;
    fixed _FurThinness;
    fixed _FurShading;
    
    fixed4 _AOColor;
    fixed4 _ShadowColor;
    
    float4 _ForceGlobal;
    float4 _ForceLocal;
    
    
    fixed4 _RimColor;
    half _RimPower;

CBUFFER_END

v2f vert_surface(appdata_base v)
{
    v2f o;
    /*v.vertex.x += abs( sin(v.vertex.z * _WindDistribution + _Time.y * _WindFrequency) * _WindAmplitude) * v.texcoord.y;
    v.vertex.z += abs( cos(v.vertex.y * _WindDistribution + _Time.y * _WindFrequency) * _WindAmplitude) * v.texcoord.y;*/

    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

    return o;
}

v2f vert_base(appdata_base v)
{
    v2f o;
    v.vertex.x += /*abs*/( sin(/*v.vertex.z */ _WindDistribution + _Time.y * _WindFrequency) * _WindAmplitude) * v.texcoord.y*FURSTEP;
    v.vertex.y += /*abs*/( cos(/*v.vertex.y */ _WindDistribution + _Time.y * _WindFrequency) * _WindAmplitude) * v.texcoord.y*FURSTEP;

    float3 P = v.vertex.xyz + v.normal * _FurLength * FURSTEP;
    P += clamp(mul(unity_WorldToObject, _ForceGlobal).xyz + _ForceLocal.xyz, -1, 1) * pow(FURSTEP, 3) * _FurLength;
    o.pos = UnityObjectToClipPos(float4(P, 1.0));
    o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.uv.zw = TRANSFORM_TEX(v.texcoord, _FurTex);
    //clip(_FurTex.a - _Cutoff);
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    TRANSFER_SHADOW(o)  
    return o;
}

fixed4 frag_surface(v2f i): SV_Target
{
    
    fixed3 worldNormal = normalize(i.worldNormal);
    fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
    fixed3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
    fixed3 worldHalf = normalize(worldView + worldLight);
    fixed shadow = SHADOW_ATTENUATION(i);  
    //clip(texColor.a - _Cutoff);
    fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color;
    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo*_AOColor;
    fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldNormal, worldLight));
    fixed3 lightting = saturate(dot(worldNormal, worldLight));
    fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, worldHalf)), _FurShininess);

    fixed3 color = ambient + diffuse + specular;
    
    return fixed4(color*shadow, 1.0);
    //return fixed4(diffuse, 1.0);
   // return fixed4(ambient, 1.0);
}

fixed4 frag_base(v2f i): SV_Target
{
    fixed3 worldNormal = normalize(i.worldNormal);
    fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
    fixed3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
    fixed3 worldHalf = normalize(worldView + worldLight);
    
    fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color;
    albedo -= (pow(1 - FURSTEP, 3)) * _FurShading;
    half rim = 1.0 - saturate(dot(worldView, worldNormal));
    albedo += fixed4(_RimColor.rgb * pow(rim, _RimPower), 1.0);
    
    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo*_AOColor;
    fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldNormal, worldLight));
    fixed3 lightting = pow(saturate(dot(worldNormal, worldLight)),_ShadowRange);
    fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, worldHalf)), _FurShininess);

    /*if (lightting.x < _ShadowRange){
		 diffuse = _ShadowColor;
	 }*/

    fixed3 color = ambient + diffuse + specular;
    fixed3 noise = tex2D(_FurTex, i.uv.zw * _FurThinness).rgb;
   
    fixed alpha = clamp(noise - (FURSTEP * FURSTEP) * _FurDensity, 0, 1);
    clip(alpha-_Cutoff);

    return fixed4(color, 1);
    //return fixed4(ambient, alpha);
}
#endif // FurHelper