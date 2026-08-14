Shader "Custom/Laser"
{
    Properties
    {
        _GlowColor ("Glow Color", Color) = (1, 0, 0, 1)
        _CoreColor ("Core Color", Color) = (1, 0.8, 0.7, 1)

        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 2
        _CoreIntensity ("Core Intensity", Range(0, 20)) = 8

        _CoreWidth ("Core Width", Range(0.01, 1)) = 0.15
        _CoreSoftness ("Core Softness", Range(0.001, 0.5)) = 0.08

        _GlowPower ("Glow Falloff", Range(0.1, 8)) = 2

        _PulseStrength ("Pulse Strength", Range(0, 1)) = 0.15
        _PulseScale ("Pulse Scale", Float) = 25
        _PulseSpeed ("Pulse Speed", Float) = 8
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off

        Blend One One

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _GlowColor;
            float4 _CoreColor;

            float _GlowIntensity;
            float _CoreIntensity;

            float _CoreWidth;
            float _CoreSoftness;
            float _GlowPower;

            float _PulseStrength;
            float _PulseScale;
            float _PulseSpeed;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // uv.y:
                //
                // 0 ---------------- edge
                // 0.5 -------------- center
                // 1 ---------------- edge

                float distanceFromCenter =
                    abs(i.uv.y - 0.5) * 2.0;

                // Мягкое широкое тело лазера.
                float glow =
                    pow(
                        saturate(1.0 - distanceFromCenter),
                        _GlowPower
                    );

                // Узкое яркое ядро.
                float core =
                    1.0 - smoothstep(
                        _CoreWidth,
                        _CoreWidth + _CoreSoftness,
                        distanceFromCenter
                    );

                // Небольшое движение энергии вдоль луча.
                float pulse =
                    sin(
                        i.uv.x * _PulseScale -
                        _Time.y * _PulseSpeed
                    );

                pulse = pulse * 0.5 + 0.5;

                float intensity =
                    lerp(
                        1.0 - _PulseStrength,
                        1.0,
                        pulse
                    );

                float3 color =
                    _GlowColor.rgb * glow * _GlowIntensity
                    +
                    _CoreColor.rgb * core * _CoreIntensity;

                color *= intensity;

                return float4(color, 1);
            }

            ENDCG
        }
    }
}