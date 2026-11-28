Shader "Custom/Glitch"
{
    Properties
    {
        [PerRendererData] _MainTex ("Mask (Sprite)", 2D) = "white" {}
        _Strength ("Block Shift", Range(0, 0.1)) = 0.03
        _SplitAmount ("RGB Split", Range(0, 0.02)) = 0.004
        _NoiseAmount ("Static Noise", Range(0, 1)) = 0.25
        _ScanlineAmount ("Scanlines", Range(0, 1)) = 0.15
        _BlockRows ("Block Rows", Float) = 24
        _Speed ("Glitch Speed", Float) = 12
        _BurstChance ("Burst Chance", Range(0, 1)) = 0.35
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Tags { "LightMode"="Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_CameraSortingLayerTexture);
            SAMPLER(sampler_CameraSortingLayerTexture);

            CBUFFER_START(UnityPerMaterial)
                float _Strength;
                float _SplitAmount;
                float _NoiseAmount;
                float _ScanlineAmount;
                float _BlockRows;
                float _Speed;
                float _BurstChance;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 color : COLOR;
            };

            float Hash11(float p)
            {
                p = frac(p * 0.1031);
                p *= p + 33.33;
                p *= p + p;
                return frac(p);
            }

            float Hash21(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            half3 SampleScene(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_CameraSortingLayerTexture, sampler_CameraSortingLayerTexture, uv).rgb;
            }

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.screenPos = ComputeScreenPos(o.positionCS);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float amount = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).a * i.color.a;
                clip(amount - 0.001);

                float2 suv = i.screenPos.xy / i.screenPos.w;

                float step01 = floor(_Time.y * _Speed);
                float burst = step(1.0 - _BurstChance, Hash11(step01 * 1.37));

                float row = floor(suv.y * _BlockRows);
                float rowOn = step(0.6, Hash21(float2(row * 3.1, step01 + 7.0)));
                float shift = (Hash21(float2(row, step01)) - 0.5) * 2.0 * _Strength * rowOn * burst * amount;

                float split = _SplitAmount * (0.3 + burst) * amount;

                half3 col;
                col.r = SampleScene(suv + float2(shift + split, 0)).r;
                col.g = SampleScene(suv + float2(shift, 0)).g;
                col.b = SampleScene(suv + float2(shift - split, 0)).b;

                float n = Hash21(floor(suv * _ScreenParams.xy * 0.5) + frac(_Time.y * 7.0) * 100.0);
                col = lerp(col, n.xxx, _NoiseAmount * (0.4 + 0.6 * burst) * amount);

                float scan = 0.5 + 0.5 * sin(suv.y * _ScreenParams.y * 1.5);
                col *= 1.0 - _ScanlineAmount * scan * amount;

                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
}
