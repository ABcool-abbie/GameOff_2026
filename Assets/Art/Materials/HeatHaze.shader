Shader "Custom/HeatHaze"
{
    Properties
    {
        [MainTexture] _MainTex ("Mask (alpha)", 2D) = "white" {}
        _Strength ("Max Strength", Float) = 0.01
        _WaveScale ("Wave Scale", Float) = 8
        _Speed ("Speed", Float) = 1.5
        _Tint ("Heat Tint", Color) = (1, 0.85, 0.7, 1)
        _TintAmount ("Tint Amount", Range(0, 1)) = 0.15
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
                float4 screenPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_CameraSortingLayerTexture);
            SAMPLER(sampler_CameraSortingLayerTexture);

            float _Strength, _WaveScale, _Speed, _TintAmount;
            float4 _Tint;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color;
                o.screenPos = ComputeScreenPos(o.positionCS);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                
                float mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).a;
                float amount = i.color.a * mask;
                clip(amount - 0.001);


                float t = _Time.y * _Speed;
                float2 p = i.uv * _WaveScale;
                float2 offset;
                offset.x = sin(p.y * 3.1 + t * 2.0) * 0.6 + sin(p.y * 7.3 - t * 3.1) * 0.4;
                offset.y = sin(p.x * 2.7 + t * 1.7) * 0.5 + sin(p.x * 5.9 + t * 2.3) * 0.5;

                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                half4 col = SAMPLE_TEXTURE2D(_CameraSortingLayerTexture, sampler_CameraSortingLayerTexture,
                                             screenUV + offset * _Strength * amount);

                col.rgb = lerp(col.rgb, _Tint.rgb, _TintAmount * amount);
                col.a = 1;
                return col;
            }
            ENDHLSL
        }
    }
}