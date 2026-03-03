Shader"VR-Toolset/VR_Tooltip/Dithered Transparent/VR_Tootip_Dithered"
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}

        _Stencil("Stencil", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Overlay"
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
            #include "Dither Functions.hlsl"
            #include "Sdf Functions.hlsl"
            #include "Advanced Tooltips Functions.hlsl"

            UNITY_INSTANCING_BUFFER_START(Props)
            float4 _Color;
            UNITY_INSTANCING_BUFFER_END(Props)

            float4 _MainTex_ST;
            sampler2D _MainTex;

            struct appdata
            {
                half4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 pos : TEXCOORD1;
                float2 scale : TEXCOORD2;
                float4 spos : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
};

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.spos = ComputeScreenPos(o.vertex);
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * tex2D(_MainTex, i.uv);
                ditherClip(i.spos.xy / i.spos.w, color.a);
                return color;
            }

            ENDCG
        }
    }
}
