Shader "UI/Transitions/Wind"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.85, 0.9, 0.95, 1)
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

            float hash(float n)
            {
                return frac(sin(n * 12.9898) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.texcoord;

                // Cada fila horizontal arranca su barrido en un momento distinto
                float rows = 28.0;
                float row = floor(uv.y * rows);
                float h = hash(row);

                float streakWidth = 0.4;
                float startOffset = h * (1.0 - streakWidth);
                float localProgress = saturate((_Progress - startOffset) / streakWidth);

                float xPos = localProgress;
                float coverage = step(uv.x, xPos);

                // Estela brillante cerca del borde de avance (efecto de viento)
                float edgeWidth = 0.05 + h * 0.05;
                float trail = smoothstep(xPos - edgeWidth, xPos, uv.x) * coverage;

                fixed4 col = _Color;
                col.rgb = lerp(col.rgb, fixed3(1, 1, 1), trail * 0.7);
                col.a = coverage * _Color.a;

                return col * i.color;
            }
            ENDCG
        }
    }
}
