Shader "Unlit/AreaShader"
{
	Properties
	{
		_ScaleY("Scale Y",Range(0,1))=1
		_AreaTex ("Area Tex", 2D) = "white" {}
		_OffsetTex("OffsetTex",2D)="white"{}
		_LineTex("LineTex",2D)="white"{}
		_LineThickness("Line Thickness",Range(0,0.2))=0.02
		_LineColor("Line Color",Color)=(1,1,1,1)

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _AreaTex;
			float4 _AreaTex_ST;
			sampler2D _OffsetTex;
			float4 _OffsetTex_ST;
			sampler2D _LineTex;
			float4 _LineTex_ST;
			float _LineThickness;
			fixed4 _LineColor;
			float _ScaleY;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _AreaTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float scaleY = _ScaleY;

				fixed4 col = tex2D(_AreaTex, i.uv);
				fixed4 offsetCol = tex2D(_OffsetTex,i.uv);
				col.a *= offsetCol.r*scaleY -_LineThickness >= i.uv.y ? 1 : 0;
				col *= col.a;

				fixed4 lineCol = tex2D(_LineTex,i.uv);
				lineCol.a = (offsetCol.r*scaleY - _LineThickness <= i.uv.y&&offsetCol.r*scaleY >= i.uv.y) ? 1 : 0;
				lineCol *= lineCol.a*_LineColor;

				fixed4 finalCol = col + lineCol;

				return finalCol;
			}
			ENDCG
		}
	}
}
