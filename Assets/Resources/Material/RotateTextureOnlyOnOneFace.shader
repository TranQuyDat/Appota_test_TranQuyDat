Shader "Custom/RotateTextureWithLighting"
{
    Properties
    {
        _MainTex ("Arrow Texture", 2D) = "white" {}
        _Rotation ("Rotation", Range(0, 360)) = 0
        _TargetNormal ("Target Normal", Vector) = (0, 1, 0, 0)
        _BaseColor ("Base Color", Color) = (0.5, 0.5, 0.5, 1)
        _ArrowScale ("Arrow UV Scale", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert addshadow
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float _Rotation;
        float _ArrowScale;
        float4 _TargetNormal;
        fixed4 _BaseColor;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = _BaseColor.rgb;
            o.Alpha = _BaseColor.a;

            float similarity = dot(normalize(IN.worldNormal), normalize(_TargetNormal.xyz));
            if (similarity > 0.95)
            {
                // Tính toán UV đã xoay
                float2 centeredUV = (IN.uv_MainTex - 0.5f) / _ArrowScale;
                float rad = radians(_Rotation);
                float cosA = cos(rad);
                float sinA = sin(rad);
                float2 rotatedUV = float2(
                    centeredUV.x * cosA - centeredUV.y * sinA,
                    centeredUV.x * sinA + centeredUV.y * cosA
                ) + 0.5f;

                fixed4 arrow = tex2D(_MainTex, rotatedUV);
                o.Albedo = lerp(o.Albedo, arrow.rgb, arrow.a);
            }
        }
        ENDCG
    }

    FallBack "Diffuse"
}
