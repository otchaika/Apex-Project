Shader "Custom/PainterlyBrushSpecular"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _ShadingGradient ("Shading Gradient", 2D) = "white" {} // Gradient texture for specular variation
        _BrushMask ("Brush Mask Texture", 2D) = "white" {}     // Brush stroke texture
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularStrength ("Specular Strength", Range(0, 1)) = 0.5
        _SpecularSharpness ("Specular Sharpness", Range(1, 128)) = 16.0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            sampler2D _ShadingGradient;
            sampler2D _BrushMask;
            float4 _SpecularColor;
            float _SpecularStrength;
            float _SpecularSharpness;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.position = TransformObjectToHClip(v.position);
                o.uv = v.uv;
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.worldPos = TransformObjectToWorld(v.position);
                o.viewDir = NormalizeViewDir(o.worldPos);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Sample base texture
                half3 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb;

                // Diffuse Lighting
                half3 lightDir = _WorldSpaceLightPos0.xyz; // Main light direction
                half nDotL = saturate(dot(i.normal, lightDir));
                half3 diffuse = baseColor * nDotL;

                // Specular Highlights
                half3 refl = reflect(-lightDir, i.normal);
                half specularIntensity = pow(saturate(dot(i.viewDir, refl)), _SpecularSharpness);

                // Sample gradient based on specular intensity
                half3 gradientColor = SAMPLE_TEXTURE2D(_ShadingGradient, sampler_ShadingGradient, float2(specularIntensity, 0)).rgb;

                // Apply brush stroke texture as a mask
                half brushMask = SAMPLE_TEXTURE2D(_BrushMask, sampler_BrushMask, i.uv).r;
                half3 specular = gradientColor * _SpecularColor.rgb * specularIntensity * brushMask * _SpecularStrength;

                // Combine diffuse and specular
                half3 finalColor = diffuse + specular;

                return half4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }
}
