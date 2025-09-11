// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Unlit/ScannerDotShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GradientTex ("Gradient Texture", 2D) = "white"{}
        _DotScale ("Dot Scale", float) = 1.0
        _SampleDistance ("Color Sample Distance", float) = 10
        _AlphaFalloff("Alpha Falloff", float) = 5
        _AlphaMin("Min Alpha", float) = 0.2
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
            // make fog work
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nolightprobe nolightmap forwardadd

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            StructuredBuffer<float3> _AllInstancesTransformBuffer;
            CBUFFER_START(UnityPerMaterial)
                float _DotScale = 1;
                float _SampleDistance = 10;
                float _AlphaFalloff;
                float _AlphaMin;

            CBUFFER_END

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

            v2f vert (appdata v, uint instanceID: SV_InstanceID)
            {
                v2f o;

                // Get camera right and up vectors from unity's built-in variable
                float3 camRight = UNITY_MATRIX_V[0].xyz;
                float3 camUp = UNITY_MATRIX_V[1].xyz;

                float3 perDotPivotPos = _AllInstancesTransformBuffer[instanceID];
                // Get the object position in world space (center of billboard)
                //float3 objPos = float3(0, -3,6);//mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                // The quad vertices are assumed to be in local space around zero,
                // for example: (-0.5, -0.5), (0.5, -0.5), (0.5, 0.5), (-0.5, 0.5)
                // We offset along camera right and up vectors scaled by vertex.xy

                float3 diff = perDotPivotPos - _WorldSpaceCameraPos;
                float distSq = max(dot(diff, diff), .000001);
                distSq = distSq*rsqrt(distSq);
                o.camDist = saturate(distSq/_SampleDistance);

                if(o.camDist > 0.5){
                    _DotScale /= 2*o.camDist;
                    }
                float3 worldPos = perDotPivotPos + camRight * v.vertex.x*_DotScale + camUp * v.vertex.y*_DotScale;


                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                o.uv = v.uv;//TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 gradientColor = tex2D(_GradientTex, float2(i.camDist, 0.5));
                half4 dotTex = tex2D(_MainTex, i.uv);
                float alphaScale = _AlphaMin + (1-_AlphaMin) * exp2(-1.0 * _AlphaFalloff * i.camDist);
                dotTex.a *= alphaScale;

                return gradientColor * dotTex;
            }
            ENDHLSL
        }
    }
}
