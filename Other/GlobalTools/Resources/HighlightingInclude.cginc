// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef HIGHLIGHTING_CG_INCLUDED
#define HIGHLIGHTING_CG_INCLUDED

#include "UnityCG.cginc"

// Opaque
uniform fixed4 _Outline;

struct appdata_vert
{
	float4 vertex : POSITION;
};

float4 vert(appdata_vert v) : POSITION
{
	return UnityObjectToClipPos(v.vertex);
}

fixed4 frag() : COLOR
{
	return _Outline;
}

// Transparent
uniform sampler2D _MainTex;
uniform float4 _MainTex_ST;
uniform fixed _Cutoff;

struct appdata_vert_tex
{
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
};

struct v2f
{
	float4 pos : POSITION;
	float2 texcoord : TEXCOORD0;
};

v2f vert_alpha(appdata_vert_tex v)
{
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
	return o;
}

fixed4 frag_alpha(v2f i) : COLOR
{
	clip(tex2D(_MainTex, i.texcoord).a - _Cutoff);
	return _Outline;
}
#endif