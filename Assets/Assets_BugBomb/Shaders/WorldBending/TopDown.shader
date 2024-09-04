// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "WiedzowyWtorek/TopDown" {
	Properties{
		// Diffuse texture
		_Color("Main Color", Color) = (1,1,1,1)
		_MyPoint("Point", Vector) = (0,0,0)
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		// Degree of curvature
		_CurvatureX("CurvatureX", Float) = 0.001
		_CurvatureZ("CurvatureZ", Float) = 0.001
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
		// Surface shader function is called surf, and vertex preprocessor function is called vert
		// addshadow used to add shadow collector and caster passes following vertex modification
		#pragma surface surf Lambert vertex:vert addshadow

		// Access the shaderlab properties
		uniform fixed4 _Color;
		uniform sampler2D _MainTex;
		uniform fixed3 _MyPoint;
		uniform float _CurvatureX;
		uniform float _CurvatureZ;


		// Basic input structure to the shader function
		// requires only a single set of UV texture mapping coordinates
		struct Input {
			float2 uv_MainTex;
		};

		// This is where the curvature is applied
		void vert(inout appdata_full v)
		{
			// Transform the vertex coordinates from model space into world space
			float4 vv = mul(unity_ObjectToWorld, v.vertex);

			// Now adjust the coordinates to be relative to the camera position
			//vv.xyz -= _WorldSpaceCameraPos.xyz;

			// Reduce the y coordinate (i.e. lower the "height") of each vertex based
			// on the square of the distance from the camera in the z axis, multiplied
			// by the chosen curvature factor
			//vv = float4( 0.0f, (vv.z * vv.z) * - _CurvatureY, 0.0f, 0.0f );
			
			
			if (vv.y > 0.2)
			{
			
				float pointDistance = distance(_MyPoint.y, vv.y); // distance Y

				float dirX = sign(vv.x - _MyPoint.x);
				float dirZ = sign(vv.z - _MyPoint.z);

				//vv = float4(dirX *  _CurvatureX, 0.0f, dirZ  * -_CurvatureY, 0.0f);
				vv = float4(pointDistance * dirX *  _CurvatureX, 0.0f, pointDistance * dirZ *  _CurvatureZ, 0.0f);

				v.vertex += mul(unity_WorldToObject, vv);
			}
		}


		// This is just a default surface shader
		void surf(Input IN, inout SurfaceOutput o) {
			//half4 c = tex2D (_MainTex, IN.uv_MainTex);
			//o.Albedo = c.rgb;
			//o.Alpha = c.a;

			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
		// FallBack "Diffuse"
}