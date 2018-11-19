Shader "UguiFriend/Outline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineTex("Outline Tex", 2D) = "white" {}
		_Offset("Offset",Range(0,0.2)) =0.1
		_StartAngle("Start Angle",Range(0,360))=0
		_Center("Center",Vector)=(0.5,0.5,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _OutlineTex;
			float4 _OutlineTex_ST;
			float _Offset;
			float _StartAngle;
			float2 _Center;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float offset = _Offset;
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 upCol = tex2D(_MainTex, i.uv+ float2(0, offset));
				fixed4 downCol = tex2D(_MainTex, i.uv + float2(0, -offset));
				fixed4 leftCol = tex2D(_MainTex, i.uv + float2(-offset,0));
				fixed4 rightCol = tex2D(_MainTex, i.uv + float2(offset,0));

				fixed4 upLeftCol = tex2D(_MainTex, i.uv + float2(-offset, offset));
				fixed4 upRightCol = tex2D(_MainTex, i.uv + float2(offset, offset));
				fixed4 downLeftCol = tex2D(_MainTex, i.uv + float2(-offset, -offset));
				fixed4 downRightCol = tex2D(_MainTex, i.uv + float2(offset, -offset));

				fixed a = upCol.a+downCol.a+leftCol.a+rightCol.a+ upLeftCol.a+ upRightCol.a+downLeftCol.a+downRightCol.a;
				
				float2 dirStart = float2(0,-0.5);
				float2 dir = i.uv - float2(0.5,0.5);
				float d = dot(dirStart,dir);

				col.a = d;

				return col;
			}
			ENDCG
		}
	}
}
