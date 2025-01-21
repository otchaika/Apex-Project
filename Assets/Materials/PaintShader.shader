Shader "Custom/PaintShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} // Base texture with transparency
        _MaskTex ("Mask Texture", 2D) = "black" {} // Mask texture for painting
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Enable blending for transparency
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaskTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Sample the base texture and the mask texture
                half4 baseColor = tex2D(_MainTex, i.uv);
                half4 maskColor = tex2D(_MaskTex, i.uv);

                // Determine the final RGB color:
                // Show mask color if mask has alpha, otherwise show base texture color
                float3 finalColor = (maskColor.a > 0.0) ? maskColor.rgb : baseColor.rgb;

                // Determine the final alpha:
                // Fully transparent only if BOTH are transparent (alpha = 0.0)
                float finalAlpha = max(baseColor.a, maskColor.a);

                return half4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent"
}
