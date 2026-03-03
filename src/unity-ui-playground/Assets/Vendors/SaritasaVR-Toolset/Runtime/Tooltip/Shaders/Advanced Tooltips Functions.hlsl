#ifndef __ADVANCED_TOOLTIPS_FUNCTIONS__
#define __ADVANCED_TOOLTIPS_FUNCTIONS__
#include "Sdf Functions.hlsl"
#pragma multi_compile_instancing

#define ADVANCED_TOOLTIP_SETUP_INSTANCE_ID(v2f) \
UNITY_SETUP_INSTANCE_ID(v2f); \

#define ADVANCED_TOOLTIP_APPDATA_VARIABLES \
UNITY_VERTEX_INPUT_INSTANCE_ID \

#define ADVANCED_TOOLTIP_VARIABLES \
UNITY_INSTANCING_BUFFER_START(Props) \
float4 _Color; \
float _FillRadius; \
float4 _Center; \
half4 _BorderColor; \
float _Roundness; \
float _BorderTransition; \
float _BorderWidth; \
UNITY_INSTANCING_BUFFER_END(Props) \

#define ADVANCED_TOOLTIP_V2F_VARIABLES \
float2 pos : TEXCOORD1; \
float2 scale : TEXCOORD2; \
UNITY_VERTEX_INPUT_INSTANCE_ID \

#define ADVANCED_TOOLTIP_VERT_CALC(v, o) \
UNITY_SETUP_INSTANCE_ID(v); \
UNITY_TRANSFER_INSTANCE_ID(v, o); \
o.scale = GetObjectScale(); \
o.pos = GetObjectPosition(v.uv, o.scale); \

#define ADVANCED_TOOLTIP_APPLY_SDF(v2f, color) \
const float sdf = -sdRoundedBox(v2f.pos, v2f.scale, UNITY_ACCESS_INSTANCED_PROP(Props, _Roundness)); \
clip(sdf); \
ApplyBorderColor( \
    color, \
    UNITY_ACCESS_INSTANCED_PROP(Props, _BorderColor), \
    sdf, \
    UNITY_ACCESS_INSTANCED_PROP(Props, _BorderWidth), \
    UNITY_ACCESS_INSTANCED_PROP(Props, _BorderTransition)); \

float2 GetObjectScale()
{
    return float2(
        length(unity_ObjectToWorld._m00_m10_m20),
        length(unity_ObjectToWorld._m01_m11_m21)
        );
}

float2 GetObjectPosition(float2 vertCoord, float2 scale)
{
    return (2 * vertCoord - 1.0) * scale;
}

/// <summary>
/// color = Input and output color to modify.
/// borderColor = Color of the border.
/// sdf = Signed distance field value. Gradient from edge to the center (0-1).
/// width = Width of the border.
/// transition = Smoothness of the border transition.
/// </summary>
void ApplyBorderColor(inout float4 color, float4 borderColor, float sdf, float width, float transition)
{
    float border = smoothstep(width, width + transition, sdf);
    color = lerp(borderColor, color, border);
    color.a *= borderColor.a;
}
#endif