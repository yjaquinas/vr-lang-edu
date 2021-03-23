Shader "YC/Example1" {		// 폴더 이름 설정

	Properties{									// 항목/변수/타입 정의: Range, Float, int
		[Header(Textures)]
		_TintColor("Tint Color", Color) = (0.2, 0, 0, 0.5)	//
		_MainTex("Texture", 2D) = "white" {}				//
		_MainTex02("Texture", 2D) = "white" {}				//
		_MainTex03("Texture", 2D) = "white" {}				//
		_EmissiveColor("Emissive Color", color) = (1, 0, 0, 1)

		[NoScaleOffset] _SubTex("Sub Texture", 2D) = "white" {}
		[NoScaleOffset] [Normal] _BumpMap("Normal Texture", 2D) = "white" {}
		//[NoScaleOffset] [Normal] _NormalTexture("Normal Texture", 2D) = "white" {}
		[NoScaleOffset] [HDL] _HDLTex("HDL Texture", 2D) = "white" {}

		[Space(20)][Header(Values)][Space(20)]
		//[IntRange] _Inten("Intensity", Range(0, 100)) = 1
		_Inten("Intensity", Range(0, 1)) = 1
		_Int("Integer", int) = 5
		_Float("Float", float) = 19.2
		[Toggle] _Toggle("Toggle Float?", float) = 0
		_Color("Color", Color) = (1, 1, 1)
		_Vector("Vector", Vector) = (1, 1, 1)
	}

	SubShader{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}
		CGPROGRAM
		//#pragma surface surf Lambert			// 램버트는 조명 각도만
		//#pragma surface surf BlinnPhong		// 블린퐁은 조명각도와 카메라 각도도
		#pragma surface surf Unlit noambient	// noambient 앰비언트 라이팅도 무시하기
		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			return fixed4(s.Albedo, 1);
		}

		struct Input {
			float2 uv_MainTex;
			float2 uv_MainTex02;
			float2 uv_MainTex03;
		};

		sampler2D _MainTex;
		sampler2D _MainTex02;
		sampler2D _MainTex03;
	
		fixed4 _TintColor;
		fixed4 _EmissiveColor;
		half _Inten;

		void surf(Input IN, inout SurfaceOutput o) {
			fixed3 c = tex2D(_MainTex, IN.uv_MainTex).rgb;
			fixed3 e = tex2D(_MainTex02, IN.uv_MainTex02).rgb;
			fixed3 l = tex2D(_MainTex03, IN.uv_MainTex03).rgb;


			//o.Albedo = c.rgb * _TintColor;

			fixed l1 = l;
			fixed l2 = l2;
			
			fixed3 final = lerp(c, e, sin(_Time.y));

			o.Albedo = (final.r + final.g + final.b)*0.33;

		//	o.Emission = c * _EmissiveColor * _Inten*0.01;
		}

		ENDCG
	}
	//Fallback "Diffuse"			// 만든 쉐이더가 망했을때 Diffuse로 oh shit!
	Fallback "VertexLit"			// 만든 쉐이더가 망했을때 Diffuse로 oh shit!
}	