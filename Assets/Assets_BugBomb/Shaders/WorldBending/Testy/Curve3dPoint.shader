// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Curved3dPoint" {
    Properties {
        // Diffuse texture
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_MyPoint("Point", Vector) = (0,0,0)
        // Degree of curvature
		_CurvatureX("CurvatureX", Float) = 0.001
		_CurvatureY("CurvatureY", Float) = 0.001
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        // Surface shader function is called surf, and vertex preprocessor function is called vert
        // addshadow used to add shadow collector and caster passes following vertex modification
        #pragma surface surf Lambert vertex:vert addshadow
 
        // Access the shaderlab properties
        uniform sampler2D _MainTex;
		uniform fixed3 _MyPoint;
		uniform float _CurvatureX;
		uniform float _CurvatureY;
 
        // Basic input structure to the shader function
        // requires only a single set of UV texture mapping coordinates
        struct Input {
            float2 uv_MainTex;
        };
 
        // This is where the curvature is applied
        void vert( inout appdata_full v)
        {
            // Transform the vertex coordinates from model space into world space
            float4 vv = mul( unity_ObjectToWorld, v.vertex );
            vv.xyz -= _WorldSpaceCameraPos.xyz;


			float pointDistance = distance(_MyPoint.xz, vv.xz); // distance for radius
			vv = float4(pointDistance * pointDistance *  -_CurvatureX, pointDistance * pointDistance *  -_CurvatureY, 0.0f, 0.0f);
 

            v.vertex += mul(unity_WorldToObject, vv);
        }
 
        // This is just a default surface shader
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    // FallBack "Diffuse"
}