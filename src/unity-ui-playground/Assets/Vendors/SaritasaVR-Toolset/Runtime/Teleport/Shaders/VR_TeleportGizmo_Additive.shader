// UnlitTransparentGPUInstancing Utilize GPU instancing for buttons.
Shader "VR-Toolset/VR_Teleport Gizmo Additive"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white"
	}

		SubShader
	{
		ZWrite Off
		//ZTest Off
		Blend One One
		//Blend SrcAlpha OneMinusSrcAlpha
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent+1000"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;


			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				//float4 color : COLOR;
				//UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				//float4 color: COLOR;
				//UNITY_VERTEX_INPUT_INSTANCE_ID // necessary only if you want to access instanced properties in fragment Shader.
			};


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				//o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return _Color * tex2D(_MainTex, i.texcoord);
			}
			ENDCG
		}
	}
}