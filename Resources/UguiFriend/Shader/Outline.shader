Shader "UguiFriend/Outline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineTex("Outline Tex", 2D) = "white" {}
		_Offset("Offset",Range(0,0.2)) =0.1
		_StartAngle("Start Angle",Range(0,1))=0
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
			
			float2 rotate(float2 dir,float angle) {
				float x = dir.x*cos(angle) - dir.y*sin(angle);
				float y= dir.x*sin(angle) - dir.y*cos(angle);

				return float2(x,y);
			}

			float getRotateAngle(float2 a,float2 b)
			{
				const float epsilon = 1.0e-6;
				const float nyPI = acos(-1.0);
				float dist, dot, degree, angle;
				float x1 = a.x;
				float y1 = a.y;
				float x2 = b.x;
				float y2 = b.y;

				dist = sqrt(x1*x1+y1*y1);
				x1 /= dist;
				y1 /= dist;
				dist = sqrt(x2*x2+y2*y2);
				x2 /= dist;
				y2 /= dist;

				dot = x1 * x2 + y1 * y2;
				if (abs(dot - 1.0) <= epsilon) {
					angle = 0;
				}
				else if (abs(dot + 1.0) <= epsilon) {
					angle = nyPI;
				}
				else 
				{
					float cross;
					angle = acos(dot);
					cross = x1 * y2 - x2 * y1;
					if (cross < 0) {
						angle = 2 * nyPI - angle;
					}
				}
				degree = angle * 180 / nyPI;

				return degree;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float offset = _Offset;
				fixed4 col = tex2D(_MainTex, i.uv);

				float2 upUV = i.uv + float2(0, offset);
				float2 downUV = i.uv + float2(0, -offset);
				float2 leftUV = i.uv + float2(-offset, 0);
				float2 rightUV = i.uv + float2(offset, 0);
				float2 upLeftUV = i.uv + float2(-offset, offset);
				float2 upRightUV = i.uv + float2(offset, offset);
				float2 downLeftUV = i.uv + float2(-offset, -offset);
				float2 downRightUV = i.uv + float2(offset, -offset);

				fixed4 upCol = tex2D(_MainTex, upUV);
				fixed4 downCol = tex2D(_MainTex, downUV);
				fixed4 leftCol = tex2D(_MainTex, leftUV);
				fixed4 rightCol = tex2D(_MainTex, rightUV);
				fixed4 upLeftCol = tex2D(_MainTex, upLeftUV);
				fixed4 upRightCol = tex2D(_MainTex, upRightUV);
				fixed4 downLeftCol = tex2D(_MainTex, downLeftUV);
				fixed4 downRightCol = tex2D(_MainTex, downRightUV);

				float2 upDir = upCol.a*(upUV - _Center);
				float2 downDir = downCol.a*(downUV- _Center);
				float2 leftDir = leftCol.a*(leftUV-_Center);
				float2 rightDir = rightCol.a*(rightUV-_Center);
				float2 upLeftDir = upLeftCol.a*(upLeftUV - _Center);
				float2 upRightDir = upRightCol.a*(upRightUV - _Center);
				float2 downLeftDir = downLeftCol.a*(downLeftUV - _Center);
				float2 downRightDir = downRightCol.a*(downRightUV-_Center);

				float2 outDir = (upDir + downDir + leftDir + rightDir + upLeftDir + upRightDir + downLeftDir + downRightDir) / 8;
				outDir = normalize(outDir)*_Offset;
				fixed a = upCol.a+downCol.a+leftCol.a+rightCol.a+ upLeftCol.a+ upRightCol.a+downLeftCol.a+downRightCol.a;
				
				float2 originDir = float2(0,0.5);
				float2 startDir=rotate(originDir,_StartAngle);
				float2 dir = i.uv - _Center;
				float angle = getRotateAngle(startDir,dir);

				col = tex2D(_OutlineTex, float2(angle / 360, 0.5));

				return col;
			}
			ENDCG
		}
	}
}
