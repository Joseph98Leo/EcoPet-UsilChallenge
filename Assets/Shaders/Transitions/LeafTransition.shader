Shader "UI/Transitions/Leaf"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.25, 0.6, 0.2, 1)
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

            float sdCircle(float2 p, float2 center, float radius)
            {
                return length(p - center) - radius;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.texcoord - 0.5;
                float t = _Time.y;

                // Pequeña rotación al "desplegarse" más una oscilación sutil
                float angle = lerp(-0.4, 0.05, smoothstep(0.0, 0.6, _Progress)) + sin(t * 0.5) * 0.01;
                float s = sin(angle);
                float c = cos(angle);
                float2 p = float2(c * uv.x - s * uv.y, s * uv.x + c * uv.y);

                // La hoja crece desde el centro hasta cubrir toda la pantalla
                float size = _Progress * 3.1;
                float radius = size * 0.62;
                float offset = size * 0.42;

                // Forma de hoja: interseccion de dos circulos (vesica)
                float leaf = max(sdCircle(p, float2(-offset, 0), radius), sdCircle(p, float2(offset, 0), radius));

                float edge = 0.02;
                float inside = smoothstep(edge, -edge, leaf);

                // Vena central de la hoja
                float vein = abs(p.y) - max(size * 0.012, 0.0008);
                float veinMask = (1.0 - smoothstep(0.0, 0.01, vein)) * inside;

                fixed4 col = _Color;
                col.rgb *= lerp(1.0, 0.7, veinMask);
                col.a *= inside;

                return col * i.color;
            }
            ENDCG
        }
    }
}
