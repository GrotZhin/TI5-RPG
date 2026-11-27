
Shader "Custom/Grass"
{
    Properties
    {
        [Header(Colors)]
        _BaseColor ("Base Color", Color) = (0.10, 0.32, 0.12, 1)
        _TipColor ("Tip Color", Color) = (0.58, 0.88, 0.28, 1)
        _PatchColor ("Patch Color", Color) = (0.85, 0.85, 0.25, 1)
        _PatchScale ("Patch Scale", Float) = 0.06
        _PatchAmount ("Patch Amount", Range(0, 1)) = 0.35

        [Header(Height)]
        _Height ("Height", Float) = 0.6
        _HeightVariation ("Height Variation", Range(0, 1)) = 0.4

        [Header(Wind)]
        _WindStrength ("Wind Strength", Float) = 0.35
        _WindSpeed ("Wind Speed", Float) = 1.2
        _WindScale ("Wind Scale", Float) = 0.15
        _WindDir ("Wind Direction (XZ)", Vector) = (1, 0.4, 0, 0)

        [Header(Lighting)]
        _Translucency ("Translucency", Range(0, 1)) = 0.5
        _AmbientBoost ("Ambient Boost", Range(0, 2)) = 1

        [Header(Distance)]
        _FadeStart ("Fade Start", Float) = 40
        _FadeEnd ("Fade End", Float) = 60
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _TipColor;
                half4 _PatchColor;
                float _PatchScale;
                float _PatchAmount;
                float _Height;
                float _HeightVariation;
                float _WindStrength;
                float _WindSpeed;
                float _WindScale;
                float4 _WindDir;
                float _Translucency;
                float _AmbientBoost;
                float _FadeStart;
                float _FadeEnd;
            CBUFFER_END

            float4 _GrassPlayerPos;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 color : TEXCOORD1;
                half height01 : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float ValueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0);
                float a = Hash21(i);
                float b = Hash21(i + float2(1, 0));
                float c = Hash21(i + float2(0, 1));
                float d = Hash21(i + float2(1, 1));
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                float4x4 m = GetObjectToWorldMatrix();
                float3 origin = float3(m._m03, m._m13, m._m23);
                float rnd = Hash21(origin.xz * 7.13);

                float dist = distance(_WorldSpaceCameraPos, origin);
                float fade = saturate((_FadeEnd - dist) / max(_FadeEnd - _FadeStart, 0.01));
                float h = _Height * lerp(1.0 - _HeightVariation, 1.0, rnd) * fade;

                float t = IN.uv.y;
                float3 posOS = IN.positionOS;
                posOS.y *= h;
                float3 posWS = mul(m, float4(posOS, 1.0)).xyz;

                float2 dir = normalize(_WindDir.xy + 1e-5);
                float2 windUV = origin.xz * _WindScale + dir * _Time.y * _WindSpeed;
                float gust = ValueNoise(windUV) * 2.0 - 1.0;
                float flutter = sin(_Time.y * 3.0 + rnd * 6.2831) * 0.15;
                float bend = (gust + flutter + 0.3) * _WindStrength * t * t * h;
                posWS.xz += dir * bend;
                posWS.y -= abs(bend) * 0.25 * t;

                float radius = max(_GrassPlayerPos.w, 0.001);
                float3 away = origin - _GrassPlayerPos.xyz;
                away.y = 0;
                float d = length(away);
                float push = saturate(1.0 - d / radius);
                push *= push;
                posWS.xz += (away.xz / max(d, 0.001)) * push * h * 0.9 * t * t;
                posWS.y -= push * h * 0.5 * t * t;

                float patch = ValueNoise(origin.xz * _PatchScale);
                half3 tip = lerp(_TipColor.rgb, _PatchColor.rgb, patch * _PatchAmount);
                half3 col = lerp(_BaseColor.rgb, tip, pow(t, 1.3));
                col *= 0.9 + rnd * 0.2;

                OUT.positionWS = posWS;
                OUT.positionCS = TransformWorldToHClip(posWS);
                OUT.color = col;
                OUT.height01 = t;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                float3 up = float3(0, 1, 0);
                float wrap = dot(up, mainLight.direction) * 0.5 + 0.5;
                half3 diffuse = mainLight.color * wrap * mainLight.shadowAttenuation;
                half3 ambient = SampleSH(up) * _AmbientBoost;

                half3 col = IN.color * (diffuse + ambient);

                float3 V = normalize(_WorldSpaceCameraPos - IN.positionWS);
                float back = pow(saturate(dot(V, -mainLight.direction)), 4.0);
                col += IN.color * mainLight.color * back * IN.height01 * _Translucency;

                col *= lerp(0.55, 1.0, saturate(IN.height01 * 2.0));

                return half4(col, 1);
            }
            ENDHLSL
        }
    }
}
