
Shader "Custom/Foliage"
{
    Properties
    {
        [Header(Textura opcional)]
        [Toggle(_USE_TEX)] _UseTex ("Usar textura com alpha", Float) = 0
        _MainTex ("Textura (alpha)", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5

        [Header(Colors)]
        _BottomColor ("Bottom Color", Color) = (0.30, 0.06, 0.08, 1)
        _TopColor ("Top Color", Color) = (0.85, 0.28, 0.18, 1)
        _PatchColor ("Tint Color (variacao por arvore)", Color) = (0.95, 0.55, 0.15, 1)
        _TintVariation ("Tint Variation", Range(0, 1)) = 0.3

        [Header(Crown shape)]
        _TreeHeight ("Tree Height", Float) = 6
        _CrownCenterY ("Crown Center Y", Float) = 4
        _SphericalNormals ("Spherical Normals", Range(0, 1)) = 0.8

        [Header(Wind)]
        _WindStrength ("Wind Strength", Float) = 0.15
        _WindSpeed ("Wind Speed", Float) = 1.0
        _WindScale ("Wind Scale", Float) = 0.1
        _Flutter ("Leaf Flutter", Float) = 0.04

        [Header(Lighting)]
        _Translucency ("Translucency", Range(0, 1)) = 0.4
        _AmbientBoost ("Ambient Boost", Range(0, 2)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "RenderPipeline"="UniversalPipeline" "Queue"="AlphaTest" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float _Cutoff;
            half4 _BottomColor;
            half4 _TopColor;
            half4 _PatchColor;
            float _TintVariation;
            float _TreeHeight;
            float _CrownCenterY;
            float _SphericalNormals;
            float _WindStrength;
            float _WindSpeed;
            float _WindScale;
            float _Flutter;
            float _Translucency;
            float _AmbientBoost;
        CBUFFER_END

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

        float3 ApplyWind(float3 posWS, float3 posOS)
        {
            float3 origin = float3(UNITY_MATRIX_M._m03, UNITY_MATRIX_M._m13, UNITY_MATRIX_M._m23);
            float hgt = saturate(posOS.y / max(_TreeHeight, 0.01));
            float weight = hgt * hgt;

            float t = _Time.y * _WindSpeed;
            float gust = ValueNoise(origin.xz * _WindScale + float2(t * 0.4, t * 0.2)) * 2.0 - 1.0;
            float sway = (gust + 0.3) * _WindStrength * weight * _TreeHeight;

            float phase = dot(posWS, float3(1.7, 2.3, 1.1));
            float flutter = sin(_Time.y * 5.0 + phase) * _Flutter * weight;

            posWS.x += sway + flutter;
            posWS.z += sway * 0.5 + flutter * 0.6;
            posWS.y += flutter * 0.4;
            return posWS;
        }
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _USE_TEX
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                half3 color : TEXCOORD3;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 posWS = TransformObjectToWorld(IN.positionOS);
                posWS = ApplyWind(posWS, IN.positionOS);

                float3 toCenter = IN.positionOS - float3(0, _CrownCenterY, 0);
                float3 sphericalOS = normalize(toCenter + 1e-5);
                float3 nOS = normalize(lerp(IN.normalOS, sphericalOS, _SphericalNormals));

                float hgt = saturate(IN.positionOS.y / max(_TreeHeight, 0.01));
                half3 col = lerp(_BottomColor.rgb, _TopColor.rgb, smoothstep(0.15, 1.0, hgt));
                float3 origin = float3(UNITY_MATRIX_M._m03, UNITY_MATRIX_M._m13, UNITY_MATRIX_M._m23);
                float rnd = Hash21(origin.xz * 3.17);
                col = lerp(col, col * _PatchColor.rgb * 1.6, rnd * _TintVariation);

                OUT.positionWS = posWS;
                OUT.positionCS = TransformWorldToHClip(posWS);
                OUT.normalWS = TransformObjectToWorldDir(nOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = col;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                #if defined(_USE_TEX)
                    half a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).a;
                    clip(a - _Cutoff);
                #endif

                float3 n = normalize(IN.normalWS);
                float3 V = normalize(_WorldSpaceCameraPos - IN.positionWS);

                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                float wrap = dot(n, mainLight.direction) * 0.5 + 0.5;
                wrap = smoothstep(0.2, 0.85, wrap);
                half3 diffuse = mainLight.color * wrap * lerp(0.4, 1.0, mainLight.shadowAttenuation);
                half3 ambient = SampleSH(n) * _AmbientBoost;

                half3 col = IN.color * (diffuse + ambient);

                float back = pow(saturate(dot(V, -mainLight.direction)), 4.0);
                col += IN.color * mainLight.color * back * _Translucency;

                return half4(col, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #pragma shader_feature_local _USE_TEX
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            float3 _LightDirection;
            float3 _LightPosition;

            struct AttributesS
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct VaryingsS
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            VaryingsS vertShadow(AttributesS IN)
            {
                VaryingsS OUT;
                float3 posWS = TransformObjectToWorld(IN.positionOS);
                posWS = ApplyWind(posWS, IN.positionOS);
                float3 nWS = TransformObjectToWorldNormal(IN.normalOS);

                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                    float3 lightDir = normalize(_LightPosition - posWS);
                #else
                    float3 lightDir = _LightDirection;
                #endif

                float4 posCS = TransformWorldToHClip(ApplyShadowBias(posWS, nWS, lightDir));
                #if UNITY_REVERSED_Z
                    posCS.z = min(posCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    posCS.z = max(posCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif

                OUT.positionCS = posCS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 fragShadow(VaryingsS IN) : SV_Target
            {
                #if defined(_USE_TEX)
                    half a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).a;
                    clip(a - _Cutoff);
                #endif
                return 0;
            }
            ENDHLSL
        }
    }
}
