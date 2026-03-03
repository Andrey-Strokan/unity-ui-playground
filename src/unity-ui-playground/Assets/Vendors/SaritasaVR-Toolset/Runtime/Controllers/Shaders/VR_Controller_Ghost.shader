Shader "VR-Toolset/VR_Controller Ghost"
{
	Properties
	{
		_ColorTopController("Controller Color Top", Color) = (0.031896, 0.0343398, 0.0368894)
		_ColorBottomController("Controller Color Bottom", Color) = (0.0137021, 0.0144438, 0.0152085)
		_ColorTopHglt("Button highlights Color Top", Color) = (0.031896, 0.0343398, 0.0368894)
		_ColorBottomHglt("Button highlights Color Bottom", Color) = (0.0137021, 0.0144438, 0.0152085)

		_RimFactor("Rim Factor", Range(0.01, 1.0)) = 0.65
		_FresnelPower("Fresnel Power", Range(0.01,1.0)) = 0.16
		
		_ControllerAlpha("Controller Alpha", Range(0, 1)) = 1.0
		_HgltsAlpha("Highlight Alpha", Range(0, 1)) = 1.0

		[Header(Visibility)]
		[Space]
		_ControllerVisibility("Controller Visibility", Range(0, 1)) = 1.0
		_TouchpadVisibility("Touchpad Visibility", Range(0, 1)) = 1.0
		_TriggerVisibility("Trigger Visibility", Range(0, 1)) = 1.0
		_ButtoEnterVisibility("Button Enter Visibility", Range(0, 1)) = 1.0
		_ButtonOneVisibility("Button One Visibility", Range(0, 1)) = 1.0
		_ButtonTwoVisibility("Button Two Visibility", Range(0, 1)) = 1.0
		_GripVisibility("Grip Visibility", Range(0, 1)) = 1.0
		_HgltsVisibility("All highlights Visibility", Range(0, 1)) = 1.0
		_OverallVisibility("Overall Visibility", Range(0, 1)) = 1.0

		[Header(IDs assigment)]
		[Space]
		_ControllerBodyId("Controller body Id", int) = 255
		_TouchpadId("Touch-pad Id", int) = 1
		_TriggerId("Trigger Id", int) = 2
		_ButtonEnterId("Button Enter Id", int) = 3
		_ButtonOneId("Button One Id", int) = 4
		_ButtonTwoId("Button Two Id", int) = 5
		_GripId("Grip Id", int) = 6
	}
	
	SubShader
	{
		Tags
		{
			"RenderType" = "Overlay"
			"Queue" = "Overlay"
		}

		Pass
		{
			Stencil
            {
                Ref 8
                Comp NotEqual
            }
			
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			#define ColorBlack half3(0, 0, 0)
			#define EmissionFactor (0.95)

			CBUFFER_START(UnityPerMaterial)
			half3 _ColorTopController;
			half3 _ColorBottomController;
			half3 _ColorTopHglt;
			half3 _ColorBottomHglt;

			half _RimFactor;
			half _FresnelPower;
			
			half _ControllerAlpha;
			half _HgltsAlpha;

			half _ControllerVisibility;
			half _TouchpadVisibility;
			half _TriggerVisibility;
			half _ButtoEnterVisibility;
			half _ButtonOneVisibility;
			half _ButtonTwoVisibility;
			half _GripVisibility;
			half _HgltsVisibility;
			half _OverallVisibility;

			int _ControllerBodyId;
			int _TouchpadId;
			int _TriggerId;
			int _ButtonEnterId;
			int _ButtonOneId;
			int _ButtonTwoId;
			int _GripId;
			CBUFFER_END

			struct appdata
			{
				half4 vertex : POSITION;
				half3 normal: NORMAL;
				half4 color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				UNITY_FOG_COORDS(1)
				half4 vertex : SV_POSITION;
				half3 viewDir : TEXCOORD3;
				half3 normal: NORMAL;
				half4 color : COLOR;

				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.viewDir = ObjSpaceViewDir(v.vertex);
				o.normal = v.normal;
				o.color = v.color;
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				int index = round(i.color.r * 255.0);
				half a0 = saturate(1 - abs(_ControllerBodyId - index));
				half a1 = saturate(1 - abs(_TouchpadId - index));
				half a2 = saturate(1 - abs(_TriggerId - index));
				half a3 = saturate(1 - abs(_ButtonEnterId - index));
				half a4 = saturate(1 - abs(_ButtonOneId - index));
				half a5 = saturate(1 - abs(_ButtonTwoId - index));
				half a6 = saturate(1 - abs(_GripId - index));

				half3 colorTop = lerp(_ColorTopHglt, _ColorTopController, a0);
				half3 colorBottom = lerp(_ColorBottomHglt, _ColorBottomController, a0);

				half3 normalDirection = normalize(i.normal);
				half3 viewDir = normalize(i.viewDir);
				half viewDotNormal = saturate(dot(viewDir, normalDirection));

				// the higher the rim factor, the greater the effect overall. by default,
				// it's strongest near edges
				half rim = pow(1.0 - viewDotNormal, 0.5) * (1.0 - _RimFactor) + _RimFactor;
				rim = saturate(rim);
				// brighten emission a bit, multiply by factor to reign it in
				rim += rim * 0.5;
				rim *= EmissionFactor;

				half fresnel = saturate(pow(1.0 - viewDotNormal, _FresnelPower));
				half4 color;

				color.rgb = lerp(colorTop, colorBottom, fresnel) * rim;

				half curHgltsAlpha = _HgltsAlpha * _HgltsVisibility;

				half alpha = (
					a0 * _ControllerAlpha * _ControllerVisibility +
					a1 * curHgltsAlpha * _TouchpadVisibility +
					a2 * curHgltsAlpha * _TriggerVisibility +
					a3 * curHgltsAlpha * _ButtoEnterVisibility +
					a4 * curHgltsAlpha * _ButtonOneVisibility +
					a5 * curHgltsAlpha *  _ButtonTwoVisibility +
					a6 * curHgltsAlpha *  _GripVisibility) * _OverallVisibility * rim;

				color.a = alpha;
				return color;
			}
		ENDCG
		}
	}
}
