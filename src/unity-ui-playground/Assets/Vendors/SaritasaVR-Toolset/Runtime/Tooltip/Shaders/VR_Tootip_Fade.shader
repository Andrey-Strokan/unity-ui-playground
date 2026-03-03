Shader"VR-Toolset/VR_Tooltip/VR_Tootip_Fade"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)

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
            Cull Off
            ZWrite Off
            ZTest[_ZTest]

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            float4 _Color;
            
            struct appdata
            {
                half4 vertex : POSITION;
            };
            
            struct v2f
            {
                half4 vertex : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
        ENDCG
        }
    }
}
