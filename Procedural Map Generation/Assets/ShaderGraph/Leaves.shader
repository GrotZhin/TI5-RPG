Shader "Custom/Leaves"
{
    Properties
    {
        [Header(Colors)]
        _ColorA ("Color A", Color) = (0.80, 0.18, 0.10, 1)
        _ColorB ("Color B", Color) = (0.95, 0.50, 0.12, 1)
        _ColorC ("Color C", Color) = (0.95, 0.80, 0.25, 1)

        [Header(Shape)]
        _Size ("Leaf Size", Float) = 0.18
        _SizeVariation ("Size Variation", Range(0, 1)) = 0.5

        [Header(Motion)]
        _FallSpeed ("Fall Speed", Float) = 0.8
        _Sway ("Sway Amount", Float) = 0.6
        _SpinSpeed ("Spin Speed", Float) = 1.5
        _Tilt ("Tilt Amount (rad)", Float) = 0.8
        _WindDir ("Wind Direction (XZ)", Vector) = (1, 0.3, 0, 0)
        _WindSpeed ("Wind Speed", Float) = 1.2

        [Header(Lighting)]
        _Translucency ("Translucency", Range(0, 1)) = 0.5
        _AmbientBoost ("Ambient Boost", Range(0, 2)) = 1
        _Emission ("Emission", Range(0, 1)) = 0.1

        [Header(Box (definido via script))]
        _BoxMin ("Box Min", Vector) = (0, 0, 0, 0)
        _BoxSize ("Box Size", Vector) = (30, 12, 30, 0)
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "RenderPipeline"="UniversalPipeline" "Queue"="AlphaTest" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _ColorA;
                half4 _ColorB;
                half4 _ColorC;
                float _Size;
                float _SizeVariation;
                float _FallSpeed;
                float _Sway;
                float _SpinSpeed;
                float _Tilt;
                float4 _WindDir;
                float _WindSpeed;
                float _Translucency;
                float _AmbientBoost;
                float _Emission;
                float4 _BoxMin;
                float4 _BoxSize;
            CBUFFER_END

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
                float3 normalWS : TEXCOORD1;
                half3 color : TEXCOORD2;
                float2 uv : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float Hash13(float3 p)
            {
                p = frac(p * 0.1031);
                p += dot(p, p.zyx + 31.32);
                return frac((p.x + p.y) * p.z);
            }

            float3 RotX(float3 v, float a)
            {
                float s, c; sincos(a, s, c);
                return float3(v.x, c * v.y - s * v.z, s * v.y + c * v.z);
            }

            float3 RotY(float3 v, float a)
            {
                float s, c; sincos(a, s, c);
                return float3(c * v.x + s * v.z, v.y, -s * v.x + c * v.z);
            }

            float3 RotZ(float3 v, float a)
            {
                float s, c; sincos(a, s, c);
                return float3(c * v.x - s * v.y, s * v.x + c * v.y, v.z);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                float4x4 m = GetObjectToWorldMatrix();
                float3 u = float3(m._m03, m._m13, m._m23);

                float r1 = Hash13(u * 13.7);
                float r2 = Hash13(u * 29.3 + 7.1);
                float r3 = Hash13(u * 51.9 + 3.3);
                float r4 = Hash13(u * 83.1 + 9.7);

                float t = _Time.y;
                float3 size3 = _BoxSize.xyz;
                float3 p = u * size3;

                p.y -= t * _FallSpeed * lerp(0.6, 1.3, r1);
                float2 wdir = normalize(_WindDir.xy + 1e-5);
                p.xz += wdir * _WindSpeed * t * lerp(0.7, 1.2, r2);
                float3 pn = frac(p / size3);
                p = pn * size3;

                float3 edge = min(pn, 1.0 - pn);
                float fade = smoothstep(0.0, 1.0, saturate(min(edge.y * 6.0, min(edge.x * 10.0, edge.z * 10.0))));

                float ph = r3 * 6.2831;
                p.x += sin(t * 1.3 + ph) * _Sway;
                p.z += cos(t * 1.1 + ph * 1.7) * _Sway * 0.8;
                p.y += sin(t * 2.1 + ph) * _Sway * 0.15;

                float3 center = _BoxMin.xyz + p;

                float yaw = ph + t * _SpinSpeed * lerp(-1.0, 1.0, r2);
                float tx = sin(t * 2.3 + ph * 2.0) * _Tilt;
                float tz = cos(t * 1.9 + ph) * _Tilt;

                float scale = _Size * lerp(1.0 - _SizeVariation, 1.0, r4) * fade;
                float3 local = IN.positionOS * scale;
                local = RotY(RotX(RotZ(local, tz), tx), yaw);

                float3 n = RotY(RotX(RotZ(float3(0, 1, 0), tz), tx), yaw);

                float3 posWS = center + local;

                half3 col = lerp(lerp(_ColorA.rgb, _ColorB.rgb, saturate(r1 * 2.0)),
                                 _ColorC.rgb, saturate(r1 * 2.0 - 1.0));
                col *= 0.9 + r4 * 0.2;

                OUT.positionWS = posWS;
                OUT.positionCS = TransformWorldToHClip(posWS);
                OUT.normalWS = n;
                OUT.color = col;
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float x = IN.uv.x - 0.5;
                float y = IN.uv.y;
                float w = 0.5 * pow(saturate(sin(3.14159 * y)), 0.8) * (1.0 - y * 0.4);
                clip(w - abs(x));

                half3 base = IN.color;

                float edgeDark = saturate((w - abs(x)) / max(w, 0.001) * 4.0);
                base *= lerp(0.75, 1.0, edgeDark);
                base *= 1.0 - step(abs(x), 0.015) * 0.25;
                base *= 1.0 - step(0.46, frac((y - abs(x) * 0.9) * 7.0)) * 0.12;

                float3 V = normalize(_WorldSpaceCameraPos - IN.positionWS);
                float3 n = normalize(IN.normalWS);
                if (dot(n, V) < 0) n = -n;

                Light mainLight = GetMainLight();
                float ndl = abs(dot(n, mainLight.direction));
                half3 diffuse = mainLight.color * (0.35 + 0.65 * ndl);
                half3 ambient = SampleSH(n) * _AmbientBoost;

                half3 col = base * (diffuse + ambient);

                float back = pow(saturate(dot(V, -mainLight.direction)), 3.0);
                col += base * mainLight.color * back * _Translucency;
                col += base * _Emission;

                return half4(col, 1);
            }
            ENDHLSL
        }
    }
}