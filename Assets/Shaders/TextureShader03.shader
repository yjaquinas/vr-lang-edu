	Shader "YC/Example3" {		// 폴더 이름 설정
		Properties{
		_MainTex("Texture", 2D) = "white" {}
		[NoScaleOffset][Normal] _BumpMap("Normal Texture", 2D) = "white" {}
	}
		SubShader{
		  Tags { "RenderType" = "Opaque" }
		  CGPROGRAM
		  #pragma surface surf Lambert
		  struct Input {
			  float2 uv_MainTex;
		  };
		  sampler2D _MainTex;
		  sampler2D _BumpMap;
		  void surf(Input IN, inout SurfaceOutput o) {
			  o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			  o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		  }
		  ENDCG
	}
		Fallback "Diffuse"
}