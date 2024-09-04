Shader "TPGshadersAdvanced/18_UVScroll" {
	Properties {
		_MainTex ("Water", 2D) = "white" {}
		_FoamTex ("Foam", 2D) = "white" {}
		_MainTexSpeed ("_MainTexSpeedMult", Range(0,10)) = 1
		_FoamTexSpeed ("_FaomTexSpeedMult", Range(0,10)) = 1
		_ScrollX ("ScrollX", Range(-5,5)) = 1
		_ScrollY ("ScrollY", Range(-5,5)) = 1
	}
	SubShader {
		
		CGPROGRAM
		
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _FoamTex;
		float _MainTexSpeed;
		float _FoamTexSpeed;
		float _ScrollX;
		float _ScrollY;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			_ScrollX *= _Time.x;
			_ScrollY *= _Time.x * _FoamTexSpeed;
			
			float2 newUV = IN.uv_MainTex + float2(_ScrollX * _MainTexSpeed, _ScrollY * _MainTexSpeed);
			float2 newUV1 = IN.uv_MainTex + float2(_ScrollX * _FoamTexSpeed, _ScrollY * _FoamTexSpeed);

			fixed4 mainCol = tex2D (_MainTex, newUV);
			fixed4 foamCol = tex2D (_FoamTex, newUV1);

			o.Albedo = (mainCol + foamCol) / 2.0; //mainCol * foamCol;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
