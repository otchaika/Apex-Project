Shader "Custom/PaintShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "black" {} // Mask texture for painting
        _PaintColor ("Paint Color", Color) = (1,0,0,1) // Red color by default
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _PaintColor;
            float4 _MainTex_ST;
            float4 _MaskTex_ST;

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

                // Use alpha from the mask texture to blend the paint color
                // If the mask has alpha (non-zero), apply paint; otherwise, use base color
                half4 finalColor = (maskColor.a > 0.0) ? maskColor : baseColor;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
