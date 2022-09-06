﻿Shader "Hidden/MeshOutline/Transparent"
{
	Properties
	{ 
		_MainTex("Texture", 2D) = "white" {}
		_Diffuse("Diffuse", Color) = (1,1,1,1)
		_HighlightColor("Highlight Color", Color) = (1,1,0,1)
		_HighlightIntensity("Highlight Intensity", Range(0.0, 2.0)) = 1
	} 		
	SubShader
	{
		Tags
		{ 
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest Always

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "MeshOutline.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Diffuse;
			fixed4 _HighlightColor;
			half _HighlightIntensity;

			v2f vert(appdata v)
			{
				v2f o;				
				
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex); 				
				//转化为世界空间下的顶点坐标				
				o.worldPos = mul(unity_ObjectToWorld, v.pos);
				//转化为世界空间下的法线向量				
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject); 	

				return o;		
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//获取顶点颜色
				fixed4 color = tex2D(_MainTex, i.uv);
				//获取摄像机世界空间下的视角的方向，并归一化
				float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);						
				//通过视线与顶点法线的点乘，来判断该点位置是否在边缘，越是边缘夹角越接近90度，点乘值越接近0
				fixed value = 1 - max(0, dot(worldViewDir, normalize(i.worldNormal)));
				//计算边缘高光			
				fixed3 highlightColor = _HighlightColor * value * _HighlightIntensity;
				//计算最终光照
				fixed3 finalColor = color.rgb * _Diffuse.rgb + highlightColor;

				return fixed4(finalColor, color.a * _Diffuse.a);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}