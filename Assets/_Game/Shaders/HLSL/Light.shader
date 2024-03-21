Shader "Custom/Light"
{
    Properties
    { 
		_MainTex("Texture", 2D) = "" {}
		_UVLength ("Length", float) = 0
		[HDR] _EmissiveColor ("Albedo", Color) = (1,1,1,1)
	}

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
		
		Blend SrcAlpha OneMinusSrcAlpha
		
        Pass
        {
            HLSLPROGRAM
			
            #pragma vertex vert
            #pragma fragment frag
			
			#pragma exclude_renderers d3d11_9x

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            struct Attributes
            {
                float4 positionOS   : POSITION;   
				float2 uv			: TEXCOORD;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
				float2 uv			: TEXCOORD;
            };

			sampler2D _MainTex;
			
			CBUFFER_START(UnityPerMaterial)
			float _UVLength;
			float _UVPositions[256];
			
			half4 _EmissiveColor;
			CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
				return OUT;
            }
			
            half4 frag(Varyings IN) : SV_Target
            {
				int index = 0;
				
				[unroll(256)]
				for(int i = 0; i < _UVLength; i++)
				{
					if((i / _UVLength) < IN.uv.x)
						index = i;
					else
						break;
				}
				
				float uvPosition = _UVPositions[(int)index];
				
                half4 color = tex2D(_MainTex, IN.uv);
				
				if(IN.uv.y >= (uvPosition))
				{
					color.a = _EmissiveColor.a;
					color.rgb = color.rgb * _EmissiveColor.rgb;
				}
				else 
					color.a =  0.0;
					
                return color;
            }
            ENDHLSL
        }
    }
}