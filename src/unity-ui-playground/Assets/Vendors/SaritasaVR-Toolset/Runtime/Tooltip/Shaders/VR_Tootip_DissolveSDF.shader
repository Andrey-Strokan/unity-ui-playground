Shader"VR-Toolset/VR_Tooltip/VR_Tootip_DissolveSDF"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _DissolveMap("Dissolve Map", 2D) = "white" {}

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
                Ref [_Stencil]
                Pass Replace
            }

            Cull Off
            ZTest[_ZTest]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Advanced Tooltips Functions.hlsl"

            ADVANCED_TOOLTIP_VARIABLES

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DissolveMap;
            float4 _DissolveMap_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                ADVANCED_TOOLTIP_APPDATA_VARIABLES
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 disolveUV : TEXCOORD3;
                float4 vertex : SV_POSITION;
                ADVANCED_TOOLTIP_V2F_VARIABLES
            };

            v2f vert (appdata v)
            {
                v2f o;
                ADVANCED_TOOLTIP_VERT_CALC(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.disolveUV = TRANSFORM_TEX(v.uv, _DissolveMap);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                ADVANCED_TOOLTIP_SETUP_INSTANCE_ID(i);
                fixed4 mask = tex2D(_DissolveMap, i.disolveUV);
                fixed4 rawColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                clip(mask.r - (1 - rawColor.a));
                fixed4 color = tex2D(_MainTex, i.uv) * rawColor;

                ADVANCED_TOOLTIP_APPLY_SDF(i, color);

                return color;
}
            ENDCG
        }
    }
}
