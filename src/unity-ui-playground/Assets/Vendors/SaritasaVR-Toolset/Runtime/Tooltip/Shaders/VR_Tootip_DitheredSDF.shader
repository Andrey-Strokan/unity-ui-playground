Shader"VR-Toolset/VR_Tooltip/Dithered Transparent/VR_Tootip_DitheredSDF"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}

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

            #include "UnityCG.cginc"
            #include "Dither Functions.hlsl"
            #include "Advanced Tooltips Functions.hlsl"

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
                float4 spos : TEXCOORD3;
                ADVANCED_TOOLTIP_V2F_VARIABLES
            };

            v2f vert(appdata v)
            {
                v2f o;
                ADVANCED_TOOLTIP_VERT_CALC(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.spos = ComputeScreenPos(o.vertex);
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                ADVANCED_TOOLTIP_SETUP_INSTANCE_ID(i);
    
                float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * tex2D(_MainTex, i.uv);
                ditherClip(i.spos.xy / i.spos.w, color.a);
    
                ADVANCED_TOOLTIP_APPLY_SDF(i, color);

                return color;
            }
            ENDCG
        }
    }
}
