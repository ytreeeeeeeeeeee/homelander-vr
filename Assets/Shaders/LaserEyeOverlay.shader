Shader "Custom/LaserEyeOverlay"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 0, 0, 1)

        _Intensity ("Intensity", Range(0, 2)) = 1
        _MaxAlpha ("Max Alpha", Range(0, 1)) = 0.45

        _RadiusX ("Clear Radius X", Range(0.1, 1.5)) = 0.55
        _RadiusY ("Clear Radius Y", Range(0.1, 1.5)) = 0.38
        _FadeWidth ("Fade Width", Range(0.01, 1.0)) = 0.35

        _PulseSpeed ("Pulse Speed", Range(0, 20)) = 4
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.12

        _WaveFrequency ("Wave Frequency", Range(0, 40)) = 14
        _WaveSpeed ("Wave Speed", Range(0, 20)) = 5
        _WaveAmount ("Wave Amount", Range(0, 1)) = 0.10
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
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

            sampler2D _MainTex;
            fixed4 _Color;

            float _Intensity;
            float _MaxAlpha;

            float _RadiusX;
            float _RadiusY;
            float _FadeWidth;

            float _PulseSpeed;
            float _PulseAmount;

            float _WaveFrequency;
            float _WaveSpeed;
            float _WaveAmount;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float4 color    : COLOR;
                float2 uv       : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Переводим UV в диапазон [-0.5 .. 0.5]
                float2 p = uv - 0.5;

                // Нормализуем под эллипс:
                // внутри "чистой зоны" значение < 1
                float2 ellipse;
                ellipse.x = p.x / _RadiusX;
                ellipse.y = p.y / _RadiusY;

                float d = length(ellipse);

                // Базовая маска:
                // 0 в центре, затем плавно возрастает к краям
                float baseMask = smoothstep(1.0, 1.0 + _FadeWidth, d);

                // Пульсация
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;

                // Волны:
                // начинаются от границы "чистой зоны" и идут наружу
                float wavePhase = (d - 1.0) * _WaveFrequency - _Time.y * _WaveSpeed;
                float wave = 1.0 + sin(wavePhase) * _WaveAmount;

                // Чтобы волны не шумели в центре, усиливаем их только там,
                // где уже есть покраснение
                wave = lerp(1.0, wave, baseMask);

                float alpha = baseMask * pulse * wave * _Intensity * _MaxAlpha;
                alpha = saturate(alpha);

                fixed4 col = _Color;
                col.a *= alpha;

                // Учитываем цвет UI-элемента
                col *= i.color;

                return col;
            }
            ENDCG
        }
    }
}
