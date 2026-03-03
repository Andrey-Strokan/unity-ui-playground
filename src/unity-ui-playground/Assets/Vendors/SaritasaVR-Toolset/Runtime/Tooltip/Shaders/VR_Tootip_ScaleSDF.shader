Shader"VR-Toolset/VR_Tooltip/VR_Tootip_ScaleSDF"
{
    Properties
    {
        _Color("Color of background", Color) = (1, 1, 1, 1)
        _MainTex("Main Texture", 2D) = "white" {}

        _Roundness("Roundness", Float) = 0

        _BorderColor("BorderColor", Color) = (1, 1, 1, 1)
        _BorderWidth("BorderWidth", Float) = 0
        _BorderTransition("BorderTransition", Float) = 0

        _Stencil("Stencil", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        Pass
        {
            Stencil
            {
                Ref[_Stencil]
                Pass Replace
            }

            Cull Off
            ZTest[_ZTest]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Advanced Tooltips Functions.hlsl"
            #include "UnityCG.cginc"

            ADVANCED_TOOLTIP_VARIABLES

            float4 _MainTex_ST;
            sampler2D _MainTex;

            struct appdata
            {
                half4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                ADVANCED_TOOLTIP_APPDATA_VARIABLES
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                ADVANCED_TOOLTIP_V2F_VARIABLES
            };

            v2f vert(appdata v)
            {
                v2f o;
                ADVANCED_TOOLTIP_VERT_CALC(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                ADVANCED_TOOLTIP_SETUP_INSTANCE_ID(i);
                fixed4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * tex2D(_MainTex, i.uv);

                const float sdf = -sdRoundedBox(
                    i.pos,
                    i.scale,
                    UNITY_ACCESS_INSTANCED_PROP(Props, _Roundness) * i.scale.x / 0.1); // This line is different from ADVANCED_TOOLTIP_APPLY_SDF.
                                                                                 // Because scale tooltip has animations that changes its scale,
                                                                                 // thats why we have to change roundness to keep ratio.

                clip(sdf);

                ApplyBorderColor(
                    color,
                    UNITY_ACCESS_INSTANCED_PROP(Props, _BorderColor),
                    sdf,
                    UNITY_ACCESS_INSTANCED_PROP(Props, _BorderWidth),
                    UNITY_ACCESS_INSTANCED_PROP(Props, _BorderTransition));

                return color;
            }
        ENDCG
        }
    }
}
