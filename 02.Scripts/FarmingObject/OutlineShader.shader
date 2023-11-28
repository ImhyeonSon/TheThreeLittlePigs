Shader"Custom/OutlineShader"
//{
//    Properties {
//		_MainTex ("Albedo (RGB)", 2D) = "white" {}
//        _OutLineColor("OutLine Color", Color) = (1,1,1,1)
//        _OutLineWidth("OutLine Width", Range(0.01, 0.1)) = 0.1
//	}
//    SubShader
//    {
//        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
//        LOD 200

//        cull front
//        zwrite off
//        CGPROGRAM
//        #pragma surface surf NoLight vertex:vert noshadow noambient
//        #pragma target 3.0

//        float4 _OutLineColor;
//        float _OutLineWidth;

//        void vert(inout appdata_full v)
//        {
//            v.vertex.xyz += v.normal.xyz * _OutLineWidth;
//        }

//        struct Input
//        {
//            float4 color;
//        };

//        void surf(Input IN, inout SurfaceOutput o)
//        {

//        }

//        float4 LightingNoLight(SurfaceOutput s, float3 lightDir, float atten)
//        {
//            return float4(_OutLineColor.rgb, 1);
//        }
//        ENDCG

//        cull back
//        zwrite on
//        CGPROGRAM
//        #pragma surface surf Lambert
//        #pragma target 3.0

//        sampler2D _MainTex;
//        struct Input
//        {
//            float2 uv_MainTex;
//        };

//        void surf(Input IN, inout SurfaceOutput o)
//        {
//            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
//            o.Albedo = c.rgb;
//            o.Alpha = c.a;
//        }
//        ENDCG
//    }
//    FallBack"Diffuse"
//}


//{
//	Properties
//	{
//		_Color("Main Color", Color) = (1,1,1,1)
//		_MainTex("Main Texture", 2D) = "white" {}
//		_Outline("Outline", Float) = 0.1
//		_OutlineColor("Outline Color", Color) = (1,1,1,1)
//	}

//	SubShader
//	{

//		// 외곽선 그리기
//		Pass
//		{
//		    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

//            Blend SrcAlpha
//            OneMinusSrcAlpha
//			cull front // 뒷면만 그리기
//			//zwrite off

//			CGPROGRAM

//			#pragma vertex vert
//			#pragma fragment frag

//            half _Outline;
//            half4 _OutlineColor;

//            struct vertexInput
//            {
//                float4 vertex : POSITION;
//            };

//            struct vertexOutput
//            {
//                float4 pos : SV_POSITION;
//            };

//            float4 CreateOutline(float4 vertPos, float Outline)
//            {
//			    // 행렬 중에 크기를 조절하는 부분만 값을 넣는다.
//			    // 밑의 부가 설명 사진 참고.
//                float4x4 scaleMat;
//                scaleMat[0][0] = 0.9f + Outline;
//                scaleMat[0][1] = 0.0f;
//                scaleMat[0][2] = 0.0f;
//                scaleMat[0][3] = 0.0f;
//                scaleMat[1][0] = 0.0f;
//                scaleMat[1][1] = 1.0f + Outline;
//                scaleMat[1][2] = 0.0f;
//                scaleMat[1][3] = 0.0f;
//                scaleMat[2][0] = 0.0f;
//                scaleMat[2][1] = 0.0f;
//                scaleMat[2][2] = 1.0f + Outline;
//                scaleMat[2][3] = 0.0f;
//                scaleMat[3][0] = 0.0f;
//                scaleMat[3][1] = 0.0f;
//                scaleMat[3][2] = 0.0f;
//                scaleMat[3][3] = 1.0f;
				
//                return mul(scaleMat, vertPos);
//            }

//            vertexOutput vert(vertexInput v)
//            {
//                vertexOutput o;

//                o.pos = UnityObjectToClipPos(CreateOutline(v.vertex, _Outline));

//                return o;
//            }

//            half4 frag(vertexOutput i) : COLOR
//            {
//                return _OutlineColor;
//            }

//			ENDCG
//		}

//		// 정상적으로 그리기
//		Pass
//		{
//            Blend SrcAlpha

//            OneMinusSrcAlpha

//            cull back
//            zwrite on
//			CGPROGRAM



//			#pragma vertex vert
//			#pragma fragment frag

//            half4 _Color;
//            sampler2D _MainTex;
//            float4 _MainTex_ST;

//            struct vertexInput
//            {
//                float4 vertex : POSITION;
//                float4 texcoord : TEXCOORD0;
//            };

//            struct vertexOutput
//            {
//                float4 pos : SV_POSITION;
//                float4 texcoord : TEXCOORD0;
//            };

//            vertexOutput vert(vertexInput v)
//            {
//                vertexOutput o;
//                o.pos = UnityObjectToClipPos(v.vertex);
//                o.texcoord.xy = (v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw);
//                return o;
//            }

//            half4 frag(vertexOutput i) : COLOR
//            {
//                return tex2D(_MainTex, i.texcoord) * _Color;
//            }

//			ENDCG
//		}
//	}
//}


{
	Properties {
			_OutlineColor ("Outline Color", Color) = (1,1,1,1)
			_Outline ("Outline width", Range (0, 1)) = .1
		}
 
	CGINCLUDE
	#include "UnityCG.cginc"
 
	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
 
	struct v2f {
		float4 pos : POSITION;
		float4 color : COLOR;
	};
 
	uniform float _Outline;
	uniform float4 _OutlineColor;
 
	v2f vert(appdata v) {
		v2f o;

		v.vertex *= ( 1 + _Outline);

		o.pos = UnityObjectToClipPos(v.vertex);
 
		o.color = _OutlineColor;
		return o;
	}
	ENDCG
 
	SubShader {
		Tags { "DisableBatching" = "True" }
		Pass {
			Name "OUTLINE"
			Tags {"LightMode" = "Always" }
			Cull Front
			ZWrite On
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			half4 frag(v2f i) :COLOR { return i.color; }
			ENDCG
		}
	}
 
	Fallback "Diffuse"
}