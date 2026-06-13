Shader "UI/Transitions/WaterWave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.15, 0.55, 0.9, 1)
        _Progress ("Progress", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
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
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            float _Progress;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.texcoord;
                float t = _Time.y;

                // Suma de ondas senoidales para simular el oleaje
                float wave = sin(uv.x * 12.0 + t * 2.0) * 0.020
                           + sin(uv.x * 25.0 - t * 3.5) * 0.012
                           + sin(uv.x * 6.0 + t * 1.2) * 0.030;

                // El margen asegura cobertura total en Progress = 0 y 1
                float margin = 0.08;
                float waterLevel = lerp(-margin, 1.0 + margin, _Progress);
                float surface = waterLevel + wave;

                float edge = 0.01;
                float belowWater = smoothstep(surface + edge, surface - edge, uv.y);

                float foamWidth = 0.04;
                float foam = smoothstep(surface - foamWidth, surface, uv.y) * belowWater;

                fixed4 col = _Color;
                col.rgb = lerp(col.rgb, fixed3(1, 1, 1), foam * 0.5);
                col.a = belowWater * _Color.a;

                return col * i.color;
            }
            ENDCG
        }
    }
}
