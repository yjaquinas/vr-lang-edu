Shader "YC/Example4" {		// 폴더 이름 설정

	Properties{									// 항목/변수/타입 정의: Range, Float, int
		[Header(Textures)]
		_MainTex("Texture", 2D) = "white" {}				//
		_AlphaCut("AlphaCut", Range(0, 1)) = 0.5

	}

		SubShader{
			Tags { "RenderType" = "AlphaTest" "Queue" = "Transparent"}
			CGPROGRAM
			#pragma surface surf Lambert alpha keepalpha			// 램버트는 조명 각도만
			//#pragma surface surf Lambert			// 램버트는 조명 각도만
			//#pragma surface surf BlinnPhong		// 블린퐁은 조명각도와 카메라 각도도
			#pragma surface surf Unlit noambient	// noambient 앰비언트 라이팅도 무시하기
			half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
			{
				return fixed4(s.Albedo, s.Alpha);
			}

			struct Input {
				float2 uv_MainTex;
			};

			sampler2D _MainTex;
			float _AlphaCut;


			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 alpha = tex2D(_MainTex, IN.uv_MainTex);

				o.Albedo = albedo;

				o.Alpha = alpha.r * _AlphaCut * abs(cos(_Time.y));

				}

				ENDCG
		}
			//Fallback "Diffuse"			// 만든 쉐이더가 망했을때 Diffuse로 oh shit!
					Fallback "VertexLit"			// 만든 쉐이더가 망했을때 Diffuse로 oh shit!
}