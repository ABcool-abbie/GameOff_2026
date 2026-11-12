Shader "UI/NightVisionVHS"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _NightColor ("Night Color", Color) = (0.3,1,0.3,1)
        _Brightness ("Brightness", Float) = 1.3
        _ChromaKeep ("Keep Color Fringe", Range(0,1)) = 0.5
        _ChromaOffset ("Chroma Offset", Float) = 0.004
        _ScanlineCount ("Scanline Count", Float) = 270
        _ScanlineStrength ("Scanline Strength", Range(0,1)) = 0.35
        _WobbleAmount ("Line Wobble", Float) = 0.004
        _TearStrength ("Tear Band", Float) = 0.03
        _NoiseStrength ("Noise", Range(0,1)) = 0.15
        _VignetteStrength ("Vignette", Range(0,1)) = 0.6
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color, _NightColor;
            float _Brightness, _ChromaKeep, _ChromaOffset;
            float _ScanlineCount, _ScanlineStrength;
            float _WobbleAmount, _TearStrength;
            float _NoiseStrength, _VignetteStrength;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            float hash (float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float t = _Time.y;

                float row = floor(uv.y * _ScanlineCount);
                uv.x += (hash(float2(row, floor(t * 12))) - 0.5) * _WobbleAmount;

                float bandY = frac(t * 0.1);
                float appear = step(0.7, hash(float2(floor(t * 0.3), 1.0)));
                uv.x += smoothstep(0.04, 0.0, abs(uv.y - bandY)) * _TearStrength * appear;

                float3 col;
                col.r = tex2D(_MainTex, uv + float2(_ChromaOffset, 0)).r;
                col.g = tex2D(_MainTex, uv).g;
                col.b = tex2D(_MainTex, uv - float2(_ChromaOffset, 0)).b;

                float lum = dot(col, float3(0.299, 0.587, 0.114));
                col = (lum + (col - lum) * _ChromaKeep) * _NightColor.rgb * _Brightness;

                float scan = sin(uv.y * _ScanlineCount * 6.28318) * 0.5 + 0.5;
                col *= 1 - _ScanlineStrength * scan;

                col += (hash(uv * _ScreenParams.xy + t) - 0.5) * _NoiseStrength;

                float2 d = i.uv - 0.5;
                col *= saturate(1 - dot(d, d) * 4 * _VignetteStrength);

                return fixed4(col * i.color.rgb, i.color.a);
            }
            ENDCG
        }
    }
}