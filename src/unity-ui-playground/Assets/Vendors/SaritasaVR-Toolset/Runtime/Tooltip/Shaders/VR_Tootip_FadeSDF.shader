Shader"VR-Toolset/VR_Tooltip/VR_Tootip_FadeSDF"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
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
            "RenderType" = "Transparent"
        }

        Pass
        {
            Stencil
            {
                Ref[_Stencil]
                Pass Replace
            }
            
            Cull Off
            ZWrite Off
            ZTest [_ZTest]
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
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
                fixed4 rawColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                fixed4 color = rawColor * tex2D(_MainTex, i.uv);
                ADVANCED_TOOLTIP_APPLY_SDF(i, color);
                return float4(color.rgb, rawColor.a);
            }
        ENDCG
        }
    }
}
