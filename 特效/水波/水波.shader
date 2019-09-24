Shader "Unlit/水波"
{
	Properties {
		_MainTex ("Main Tex", 2D) = "white" {}
		_F("F",Range(0,1))=0.1
	}
	SubShader {
		Pass
		{	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _WaveTex;
			float _F;
			struct v2f
			{
				float4 pos:POSITION;
				float2 uv:TEXCOORD0;
			};


			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv=v.texcoord.xy;
				return o;
			}
			
			fixed4 frag (v2f IN):COLOR
			{
				float2 uv=tex2D(_WaveTex,IN.uv).xy;
				uv=uv*2-1;
				uv*=_F;
				IN.uv+=uv;
				fixed4 color=tex2D(_MainTex,IN.uv);
				return color;
			}
			ENDCG
		}
	} 
}
