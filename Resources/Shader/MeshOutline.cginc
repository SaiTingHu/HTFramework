#ifndef MESHOUTLINE
#define MESHOUTLINE

struct appdata
{
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : NORMAL;
};
struct v2f
{
	float2 uv : TEXCOORD0;
	float4 pos : SV_POSITION;
	float3 worldNormal : NORMAL;
	float4 worldPos : TEXCOORD1;
};

#endif