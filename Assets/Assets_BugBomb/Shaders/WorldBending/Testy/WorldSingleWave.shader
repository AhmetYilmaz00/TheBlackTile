// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/WorldSingleWave" {
	Properties
	{
		// Diffuse color
		_Color("Main Color", Color) = (1,1,1,1)
		_MyPoint("Point", Vector) = (0,0,0)
		// Degree of curvature
		_CurvatureX("CurvatureX", Float) = 0.001
		_CurvatureY("CurvatureY", Float) = 0.001

		_Multiplier("Mult", Float) = 1
		_MaxDist("MaxDist",Float) = 10
		_WavePos("WaveDist", Float) = 5
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        // Surface shader function is called surf, and vertex preprocessor function is called vert
        // addshadow used to add shadow collector and caster passes following vertex modification
        #pragma surface surf Lambert vertex:vert addshadow
 
        // Access the shaderlab properties
		uniform fixed4 _Color;
		uniform fixed3 _MyPoint;
        uniform float _CurvatureX;
		uniform float _CurvatureY;
		uniform float _Multiplier;
		uniform float _MaxDist;
		uniform float _WavePos;
 
        // Basic input structure to the shader function
        // requires only a single set of UV texture mapping coordinates
        struct Input {
            float dummy;
        };
 
        // This is where the curvature is applied
        void vert( inout appdata_full v)
        {
            // Transform the vertex coordinates from model space into world space
            float4 vv = mul( unity_ObjectToWorld, v.vertex );
 
            // Now adjust the coordinates to be relative to the point position
            //vv.xyz -= _MyPoint.xyz;
 
            // Reduce the y coordinate (i.e. lower the "height") of each vertex based
            // on the square of the distance from the point in the z axis, multiplied
            // by the chosen curvature factor
            //vv = float4((vv.x * vv.x) * -_CurvatureY, (vv.z * vv.z) * - _CurvatureX, 0.0f, 0.0f );
			//vv = float4((vv.x ) * -_CurvatureY, 0.0f, 0.0f, 0.0f);


			// move mesh
			//vv = float4(_MyPoint.x, _MyPoint.y, _MyPoint.z, 0.0f);
 

			// time
			float t = _Time * 20;
			//vv = float4(sin(t*_MyPoint.x), _MyPoint.y, _MyPoint.z, 0.0f);

			// radius
			float pointDistance = distance(_MyPoint.xz, vv.xz); // distance for radius
			float2 ballDisplacement = vv.xz - _MyPoint.xz; // position comparison

			//float yy = lerp(0, -1, pointDistance - _WavePos);
			//vv = float4(0.0f,sin( yy)*sin(t), 0.0f, 0.0f); // zanikaj¹ca fala
			//vv = float4(0.0f, sin(yy+t), 0.0f, 0.0f); // fala przesuwaj¹ca siê w czasie

			// fala przesuwaj¹ca siê od œrodka + zanikanie
			/*if(pointDistance < _MaxDist)
				vv = float4(0.0f, lerp(1,0, pointDistance / _MaxDist) * _Multiplier * sin(yy + t), 0.0f, 0.0f);
			else
				vv = float4(0.0f, 0.0f, 0.0f, 0.0f);*/


			// wersja 1
			/*float waveAndVVDist = pointDistance - _WavePos;
			float yy = lerp(0, 1, waveAndVVDist);

			if(waveAndVVDist < _MaxDist)
				vv = float4(0.0f,_Multiplier *  yy, 0.0f, 0.0f);
			else
				vv = float4(0.0f, 0.0f, 0.0f, 0.0f);*/

			// wersja 2
			float2 wavePos = (_MyPoint.xz - vv.xz) / (_WavePos/ pointDistance); // punkt fali w danym kierunku
			float vvWaveDist = distance(vv.xz, wavePos.xy); // odleg³oœæ werteksa od fali
			float yy = vvWaveDist * vvWaveDist;

			if (vvWaveDist < _MaxDist && vvWaveDist > 5)
				vv = float4(0.0f, _Multiplier *  yy, 0.0f, 0.0f);
			else
				vv = float4(0.0f, 0.0f, 0.0f, 0.0f);


            // Now apply the offset back to the vertices in model space
            v.vertex += mul(unity_WorldToObject, vv);
        }
 
        // This is just a default surface shader
        void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
        }
        ENDCG
    }
    // FallBack "Diffuse"
}