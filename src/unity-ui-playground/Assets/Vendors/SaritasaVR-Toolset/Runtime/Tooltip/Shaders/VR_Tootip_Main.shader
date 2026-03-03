Shader "VR-Toolset/VR_Tooltip/VR_Tootip_Main"
{
    Properties
    {
        _Color("Color of background", Color) = (1, 1, 1, 1)
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
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            UNITY_INSTANCING_BUFFER_START(Props)
            float4 _Color;
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                half4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            }
        ENDCG
        }
    }
}
