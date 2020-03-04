Shader "Fur/FurRimColorShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Specular ("Specular", Color) = (1, 1, 1, 1)
        _FurShininess ("Shininess", Range(0.01, 256.0)) = 112.0
        _Cutoff ("Cutoff", Range(0.01, 1.0)) = 0.5
        
        _MainTex ("Texture", 2D) = "white" { }
        _FurTex ("Fur Pattern", 2D) = "white" { }
        
        _FurLength ("Fur Length", Range(0.0, 1)) = 0.5
        _FurDensity ("Fur Density", Range(0, 2)) = 0.11
        _FurThinness ("Fur Thinness", Range(0.01, 10)) = 1
        _FurShading ("Fur Shading", Range(0.0, 1)) = 0.25
        _AOColor ("AO Color", Color) = (1,1,1,1)      
        _ShadowColor ("Shadow Color", Color) = (1,1,1,1)
        _ShadowRange ("ShadowRange", Range(0.01, 1.0)) = 0.5


        _ForceGlobal ("Force Global", Vector) = (0, 0, 0, 0)
        _ForceLocal ("Force Local", Vector) = (0, 0, 0, 0)
        
        _RimColor ("Rim Color", Color) = (0, 0, 0, 1)
        _RimPower ("Rim Power", Range(0.0, 8.0)) = 6.0

        _WindAmplitude ("Wind Amplitude", Float) = 0.01
	    _WindFrequency ("Wind Frequency", Float) = 5
	    _WindDistribution ("Wind Distribution", Float) = 120
    }
    
    Category
    {

       // Tags { /*"LightMode" = "ShadowCaster"*/ "RenderType" = "AlphaTest" "IgnoreProjector" = "True" "Queue" = "AlphaTest" }
                    
        Tags { "RenderType"="Opaque"  }
        Cull Off
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        
        SubShader
        {
            Pass
            {Tags { "LightMode" = "SRP0402_Pass0" }
                CGPROGRAM
                
                #pragma vertex vert_surface
                #pragma fragment frag_surface
                #define FURSTEP 0.00
                #include "FurHelper.cginc"

                ENDCG
                
            }

            Pass
            {Tags { "LightMode" = "SRP0402_Pass2" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.03
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass3" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.06
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass4" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.09
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass5" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.12
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass6" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.15
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass7" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.18
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass8" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.21
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass9" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.24
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass10" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.27
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass11" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.30
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
             Pass
            {Tags { "LightMode" = "SRP0402_Pass12" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.33
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass13" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.36
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass14" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.39
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass15" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.42
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass16" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.45
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pas17" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.48
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass18" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.51
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass19" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.54
                #include "FurHelper.cginc"
                
                ENDCG
                
            }
            
            Pass
            {Tags { "LightMode" = "SRP0402_Pass20" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.57
                #include "FurHelper.cginc"
                
                ENDCG
                
            }

            Pass
            {Tags { "LightMode" = "SRP0402_Pass21" }
                CGPROGRAM
                
                #pragma vertex vert_base
                #pragma fragment frag_base
                #define FURSTEP 0.60
                #include "FurHelper.cginc"
                
                ENDCG
                
            } 
        }
        FallBack "Transparent/Cutout/VertexLit"
    }
}