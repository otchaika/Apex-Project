Shader "Fading/Surface/DissolveGlowLocal_URP"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        _GlowIntensity("Glow Intensity", Range(0.0, 5.0)) = 1
        _GlowScale("Glow Size", Range(0.0, 5.0)) = 1.0
        _Glow("Glow Color", Color) = (1, 0, 0, 1)
        _GlowEnd("Glow End Color", Color) = (1, 1, 0, 1)
        _GlowColFac("Glow Colorshift", Range(0.01, 2.0)) = 0.5

        [Toggle(FADE_PLANE)] _fade_PLANE("FADE_PLANE", Float) = 0
        [Toggle(FADE_SPHERE)] _fade_SPHERE("FADE_SPHERE", Float) = 1
        _SectionPoint("_SectionPoint", Vector) = (0,0,0,1)
        _Radius("_Radius", Float) = 0.8
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Name "DissolveGlowPass"
            Tags { "LightMode"="UniversalForward" }

            Cull Off
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ FADE_PLANE FADE_SPHERE
            #pragma multi_compile _ DISSOLVE_GLOW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            float4 _Color;
            float _Glossiness;
            float _Metallic;

            float _GlowIntensity;
            float _GlowScale;
            float4 _Glow;
            float4 _GlowEnd;
            float _GlowColFac;

            float _Radius;
            float4 _SectionPoint;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            Varyings vert(Attributes v)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(v.positionOS);
                output.uv = v.uv;
                output.worldPos = TransformObjectToWorld(v.positionOS.xyz);
                output.worldNormal = TransformObjectToWorldNormal(v.normalOS);
                return output;
            }

            half4 CalculateGlow(float distance, float4 glowColor, float4 glowEndColor, float glowIntensity, float glowScale, float glowColFac)
            {
                half dPredict = (glowScale - distance) * glowIntensity;
                half dPredictCol = (glowScale * glowColFac - distance) * glowIntensity;

                // Calculate and clamp glow color
                half4 glow = dPredict * lerp(glowColor, glowEndColor, saturate(dPredictCol));
                return saturate(glow);
            }

            float4 frag(Varyings input) : SV_Target
            {
                float3 worldPos = input.worldPos;

                // Calculate dissolve factor
                float3 sectionPoint = _SectionPoint.xyz;
                float radius = _Radius;
                float distance = length(worldPos - sectionPoint);
                float dissolveFactor = saturate(1.0 - (distance / radius));

                // Calculate base color
                float4 baseColor = tex2D(_MainTex, input.uv) * _Color;

                // Calculate glow
                float4 glow = CalculateGlow(dissolveFactor, _Glow, _GlowEnd, _GlowIntensity, _GlowScale, _GlowColFac);

                // Combine colors
                float4 finalColor = lerp(baseColor, glow, dissolveFactor);
                finalColor.a = dissolveFactor; // Alpha for blending

                return finalColor;
            }
            ENDHLSL
        }
    }
}
