Shader "Custom/LaserEyeOverlay"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0, 0, 1)

        _Intensity ("Intensity", Range(0, 3)) = 1
        _Thickness ("Thickness", Range(0.01, 0.5)) = 0.18
        _Softness ("Softness", Range(0.001, 0.5)) = 0.12

        _PulseSpeed ("Pulse Speed", Range(0, 30)) = 8
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.1

        _NoiseScale ("Noise Scale", Range(1, 100)) = 20
        _NoiseSpeed ("Noise Speed", Range(0, 10)) = 2
        _NoiseAmount ("Noise Amount", Range(0, 1)) = 0.15

        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

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

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;

            fixed4 _Color;

            float _Intensity;
            float _Thickness;
            float _Softness;

            float _PulseSpeed;
            float _PulseAmount;

            float _NoiseScale;
            float _NoiseSpeed;
            float _NoiseAmount;


            v2f vert(appdata_t v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;

                return o;
            }


            float Random(float2 p)
            {
                return frac(
                    sin(dot(p, float2(12.9898, 78.233))) *
                    43758.5453
                );
            }


            float Noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);

                float a = Random(i);
                float b = Random(i + float2(1.0, 0.0));
                float c = Random(i + float2(0.0, 1.0));
                float d = Random(i + float2(1.0, 1.0));

                float2 smoothF = f * f * (3.0 - 2.0 * f);

                return lerp(
                    lerp(a, b, smoothF.x),
                    lerp(c, d, smoothF.x),
                    smoothF.y
                );
            }


            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float2 centeredUv = uv - 0.5;

                // Сохраняем круглую форму независимо
                // от соотношения сторон Canvas.
                centeredUv.x *= _ScreenParams.x / _ScreenParams.y;

                float distanceFromCenter = length(centeredUv);

                float innerRadius = 0.35;
                float outerRadius = innerRadius + _Thickness;

                float edge = smoothstep(
                    innerRadius,
                    outerRadius,
                    distanceFromCenter
                );

                float2 noiseUv =
                    uv * _NoiseScale +
                    float2(
                        _Time.y * _NoiseSpeed,
                        _Time.y * _NoiseSpeed * 0.73
                    );

                float noise = Noise(noiseUv);

                noise = lerp(
                    1.0,
                    noise,
                    _NoiseAmount
                );

                float pulse =
                    1.0 +
                    sin(_Time.y * _PulseSpeed) *
                    _PulseAmount;

                float alpha =
                    edge *
                    noise *
                    pulse *
                    _Intensity;

                fixed4 color = _Color;

                color.rgb *= _Intensity;
                color.a *= alpha;

                color *= i.color;

                return color;
            }

            ENDCG
        }
    }
}
