Shader "TPGshadersAdvanced/18_WaveScroll1"
{
	Properties
	{
		_MainTex ("Diffuse", 2D) = "white" {}
		_TintColor ("Tint color", Color) = (1,1,1,1)
		_FoamTex ("Foam", 2D) = "white" {}

		_MainTexSpeed ("_MainTexSpeedMult", Range(0,10)) = 1
		_FoamTexSpeed ("_FaomTexSpeedMult", Range(0,10)) = 1
		_ScrollX ("ScrollX", Range(-5,5)) = 1
		_ScrollY ("ScrollY", Range(-5,5)) = 1

		_Freq("Frequency", Range(0,5)) = 3
		_Speed("Speed", Range(0,100)) = 10
		_Amp("Amplitude", Range(0,1)) = 0.5
	}
	SubShader
	{
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		struct Input
		{
			float2 uv_MainTex;
			float3 vertColor;
		};

		float4 _TintColor;
		float _Freq;
		float _Speed;
		float _Amp;

		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			float2 texcoord2 : TEXCOORD2;
		};

		void vert (inout appdata v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float t = _Time * _Speed;

			// advanced wave
			float waveHeight = sin(t + v.vertex.x * _Freq) * _Amp + sin(t*2 + v.vertex.x * _Freq*2) * _Amp;
			float waveHeightZ = sin(t + v.vertex.z * _Freq) * _Amp + sin(t*2 + v.vertex.z * _Freq*2) * _Amp;
			v.vertex.y = v.vertex.y + waveHeight + waveHeightZ;
			//---------------

			// simple wave
			//float waveHeight = sin(t + v.vertex.x * _Freq) * _Amp;
			//v.vertex.y = v.vertex.y + waveHeight;
			//---------------
			
			v.normal = normalize(float3(v.normal.x + waveHeight, v.normal.y, v.normal.z));
			o.vertColor = waveHeight + 2; // waveHeight [-1,1], kolor musi być od zera
		}
		
		sampler2D _MainTex;
		sampler2D _FoamTex;
		float _MainTexSpeed;
		float _FoamTexSpeed;
		float _ScrollX;
		float _ScrollY;
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			_ScrollX *= _Time.x;
			_ScrollY *= _Time.x * _FoamTexSpeed;
			
			float2 newUV = IN.uv_MainTex + float2(_ScrollX * _MainTexSpeed, _ScrollY * _MainTexSpeed);
			float2 newUV1 = IN.uv_MainTex + float2(_ScrollX * _FoamTexSpeed, _ScrollY * _FoamTexSpeed);

			fixed4 mainCol = tex2D (_MainTex, newUV);
			fixed4 foamCol = tex2D (_FoamTex, newUV1);

			o.Albedo = IN.vertColor.rgb *((mainCol + foamCol) / 2.0) * _TintColor; //mainCol * foamCol;
		}
		ENDCG
		
	}
}

