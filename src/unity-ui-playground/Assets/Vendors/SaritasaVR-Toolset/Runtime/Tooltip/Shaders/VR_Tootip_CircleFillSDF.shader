Shader"VR-Toolset/VR_Tooltip/VR_Tootip_CircleFillSDF"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}

        _Roundness("Roundness", Float) = 0

        _BorderColor("BorderColor", Color) = (1, 1, 1, 1)
        _BorderWidth("BorderWidth", Float) = 0
        _BorderTransition("BorderTransition", Float) = 0

        _Center("Center", Vector) = (0,0,0,0)
        _FillRadius("FillRadius", float) = 1.0

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
            ZTest [_ZTest]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Advanced Tooltips Functions.hlsl"

            ADVANCED_TOOLTIP_VARIABLES

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                ADVANCED_TOOLTIP_APPDATA_VARIABLES
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
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

            fixed4 frag(v2f i) : SV_Target
            {
                ADVANCED_TOOLTIP_SETUP_INSTANCE_ID(i);

                float2 center = UNITY_ACCESS_INSTANCED_PROP(Props, _Center);
                float2 offsettedUV = float2((i.uv.y - center.y) * i.scale.y, (i.uv.x - center.x) * i.scale.x);
                float inCircle = 1 - step(UNITY_ACCESS_INSTANCED_PROP(Props, _FillRadius), length(offsettedUV));
                fixed4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * inCircle * tex2D(_MainTex, i.uv);
                clip(inCircle - 1);

                ADVANCED_TOOLTIP_APPLY_SDF(i, color);

                return float4(color.xyz, 1);
            }
        ENDCG
        }
    }
}
