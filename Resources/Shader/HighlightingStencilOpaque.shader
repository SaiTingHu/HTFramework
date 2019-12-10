Shader "Hidden/Highlighted/StencilOpaque"
{
	CGINCLUDE
	#include "HighlightingInclude.cginc"
	ENDCG
	
	SubShader
	{
		Pass
		{
			ZWrite Off
			ZTest Always
			Lighting Off
			Fog { Mode Off }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	}
	
	Fallback Off
}
