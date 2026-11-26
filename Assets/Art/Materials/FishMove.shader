Shader "Custom/FishMove"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite", 2D) = "white" {}
        _Strength ("Strength", Float) = 0.004
        _WaveScale ("Wave Scale", Float) = 6
        _Speed ("Speed", Float) = 0.6
        _TopBias ("Top Bias", Range(0, 1)) = 0.6
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
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Strength, _WaveScale, _Speed, _TopBias;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float t = _Time.y * _Speed;
                float2 p = i.uv * _WaveScale;
                float2 offset;
                offset.x = sin(p.y * 2.3 + t) * 0.7 + sin(p.y * 5.1 - t * 1.3) * 0.3;
                offset.y = sin(p.x * 1.9 + t * 0.8) * 0.3;

                float bias = lerp(1, i.uv.y, _TopBias);

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + offset * _Strength * bias);
                return col * i.color;
            }
            ENDHLSL
        }
    }
}
