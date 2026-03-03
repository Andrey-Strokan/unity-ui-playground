Shader "VR-Toolset/VR_Controller Ghost Depth Clear"
{
	Properties
	{
	}

	SubShader
	{
		Tags 
		{
			"RenderType" = "Overlay" 
			"Queue" = "Overlay" 
		}

		// Write depth values so that you see topmost layer.
		Pass
		{
			Name "DepthClear"

			Stencil
            {
                Ref 8
                Comp NotEqual
            }
			
			ZTest Always
			ZWrite On
			ColorMask 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 vert(float4 vertex : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}

			void frag()
			{
			}
			ENDCG
		}
	}
}
