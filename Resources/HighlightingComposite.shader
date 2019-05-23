// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Highlighted/Composite"
{
	Properties
	{
		_MainTex ("", 2D) = "" {}
		_BlurTex ("", 2D) = "" {}
		_StencilTex ("", 2D) = "" {}
	}
	
	SubShader
	{
		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off
			Lighting Off
			Fog { Mode off }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 pos : POSITION;
				half2 uv[2] : TEXCOORD0;
			};
			
			uniform sampler2D _MainTex;
			uniform sampler2D _BlurTex;
			uniform sampler2D _StencilTex;
			float4 _MainTex_TexelSize;
			
			v2f vert (appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				
				o.uv[0] = v.texcoord.xy;
				o.uv[1] = v.texcoord.xy;
				
				if (_MainTex_TexelSize.y < 0)
					o.uv[1].y = 1 - o.uv[1].y;
				
				return o;
			}
			
			fixed4 frag(v2f i) : COLOR
			{
				fixed4 framebuffer = tex2D(_MainTex, i.uv[0]);
				fixed4 stencil = tex2D(_StencilTex, i.uv[1]);
				
				if (any(stencil.rgb))
				{
					return framebuffer;
				}
				else
				{
					fixed4 blurred = tex2D(_BlurTex, i.uv[1]);
					fixed4 color;
					color.rgb = lerp(framebuffer.rgb, blurred.rgb, saturate(blurred.a - stencil.a));
					color.a = framebuffer.a;
					return color;
				}
			}
			
			ENDCG
		}
	}
	
	SubShader
	{
		Pass
		{
			SetTexture [_MainTex] {}
		}
	}
	
	Fallback Off
}