Shader "Unlit/ScannerDotShaderTemporary"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GradientTex ("Gradient Texture", 2D) = "white"{}
        _DotScale ("Dot Scale", float) = 1.0
        _SampleDistance ("Color Sample Distance", float) = 10
        _Fade("Fade", float) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float camDist: TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _GradientTex;
            float4 _MainTex_ST;
            float _DotScale;
            float _SampleDistance;
            float _Fade;

            v2f vert (appdata v)
            {
                v2f o;

                float3 camRight = UNITY_MATRIX_V[0].xyz;
                float3 camUp = UNITY_MATRIX_V[1].xyz;

                // Sample object's world position from the origin
                float3 objWorldPos = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                float3 diff = objWorldPos - _WorldSpaceCameraPos;
                o.camDist = saturate(dot(diff, diff) / _SampleDistance);

                float scale = _DotScale*_Fade;
                if (o.camDist > 0.5)
                {
                    scale *= 2.0 * o.camDist;
                }

                float3 worldPos = objWorldPos + camRight * v.vertex.x * scale + camUp * v.vertex.y * scale;

                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 gradientColor = tex2D(_GradientTex, float2(i.camDist, 0.5));
                gradientColor *= _Fade;
                half4 dotTex = tex2D(_MainTex, i.uv);
                return gradientColor * dotTex;
            }

            ENDHLSL
        }
    }
}
