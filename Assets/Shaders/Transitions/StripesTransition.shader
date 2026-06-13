Shader "UI/Transitions/Stripes"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.1, 0.1, 0.1, 1)
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

                // Franjas verticales que se llenan de abajo hacia arriba con un desfase en cascada
                float stripeCount = 10.0;
                float stripeIndex = floor(uv.x * stripeCount);

                float cascade = 0.5;
                float phase = (stripeIndex / stripeCount) * cascade;

                float localProgress = saturate((_Progress - phase) / (1.0 - cascade));

                float coverage = step(uv.y, localProgress);

                float edge = 0.04;
                float trail = smoothstep(localProgress - edge, localProgress, uv.y) * coverage;

                fixed4 col = _Color;
                col.rgb = lerp(col.rgb, fixed3(1, 1, 1), trail * 0.6);
                col.a = coverage * _Color.a;

                return col * i.color;
            }
            ENDCG
        }
    }
}
