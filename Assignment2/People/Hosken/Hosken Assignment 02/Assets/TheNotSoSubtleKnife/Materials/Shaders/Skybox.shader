Shader "Custom/GasGiant"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_NoiseScale("Noise Scale", Range(0.0001, 100)) = 1.0
		_Octaves("Octaves", Range(1,10)) = 1
		_Lacunarity("Lacunarity", Range(1,2)) = 1.5
		_Persistence("Persistence", Range(0,1)) = 0.5
		_Power("Power", Range(0,20)) = 1
		


	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			//THIS IS WHAT ALLOWS STENCILLING
			Stencil {
				Ref 1
				Comp Equal
			}



			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard vertex:vert 

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			#include "Noise.hlsl"

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
				float3 localPos;
			};

			half _Glossiness;
			half _Metallic;
			half _NoiseScale;

			int _Octaves;
			half _Lacunarity;
			half _Persistence;
			half _Power;
			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			//UNITY_INSTANCING_BUFFER_START(Props)
			//	// put more per-instance properties here
			//UNITY_INSTANCING_BUFFER_END(Props)

			void vert(inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.localPos = v.vertex.xyz;
			}

			half oct_noise(fixed3 pos, half scl, int oct, half lac, half per) {

				half noise_val = 0;
				half amp = 1;

				
				for (int i = 0; i < oct; i++) {

					noise_val += snoise(pos * scl) * amp;
					scl *= lac;
					amp *= per;

				}

				noise_val /= oct;
				noise_val = (noise_val + 1) / 2;

				return noise_val;
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{

				fixed3 sample_pos = IN.localPos ;
					
				half turbulence = oct_noise(IN.localPos, 10, 4, 1.5, 0.9);
				sample_pos += pow(turbulence, 2) * 0.01;

				half noise_val = oct_noise(sample_pos, _NoiseScale, _Octaves, _Lacunarity, _Persistence);
				

				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * noise_val;
				//o.Albedo = float3(0, 0, 0);
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
				o.Emission = pow(c.rgb, _Power);

			}
			ENDCG
		}
			FallBack "Diffuse"
}
