Shader "Hidden/MegaWorld/HeightNoise"
{
    Properties { _HeightTex ("Texture", any) = "" {} }

    SubShader
    {
        ZTest Always Cull OFF ZWrite Off

        HLSLINCLUDE

        #include "UnityCG.cginc"
        #include "TerrainTool.cginc"

        sampler2D _HeightTex;
        sampler2D _BaseMaskTex;
        float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height
        
        sampler2D _NoiseTex;
        float4 _NoiseTex_TexelSize;

        int _BlendMode;

        float _MinHeight;
        float _MaxHeight;

        float _MaxRangeNoise;
        float _MinRangeNoise;

        float _ClampMinHeight;
        float _ClampMaxHeight;


        struct appdata_t
        {
            float4 vertex : POSITION;
            float2 pcUV : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 pcUV : TEXCOORD0;
        };

        v2f vert( appdata_t v )
        {
            v2f o;
            
            o.vertex = UnityObjectToClipPos( v.vertex );
            o.pcUV = v.pcUV;

            return o;
        }

        float Lerp(float v0, float v1, float t) 
        {
            return (1 - t) * v0 + t * v1;
        }

        float InverseLerp(float a, float b, float t)
        {
            return (t - a) / (b - a);
        }

        float GetSharpenHeightNoise(float fractalValue, float maxRangeNoise, float minRangeNoise, float height, float minHeight, float maxHeight)
        {
            fractalValue = clamp(fractalValue, 0, 1);

            if(height > maxHeight)
            {
                if(height < maxHeight)
                {
                    return 1;
                }

                float newMaxHeight = maxHeight + maxRangeNoise;

                fractalValue = clamp(fractalValue, 0, 1);

                float falloffHeightNoise = InverseLerp(maxHeight, newMaxHeight, height);

                float clampNoiseMin = falloffHeightNoise;
                float clampNoiseMax = falloffHeightNoise;
                
                if (fractalValue < clampNoiseMin) 
                {
                    return 0;
                }
                else if (fractalValue > clampNoiseMax) 
                {
                    return 1;
                }

                return fractalValue;
            }
            else if(height < minHeight)
            {
                if(height > maxHeight)
                {
                    return 1;
                }

                float newMinHeight = minHeight - minRangeNoise;

                fractalValue = clamp(fractalValue, 0, 1);

                float falloffHeightNoise = InverseLerp(minHeight, newMinHeight, height);
                
                float clampNoiseMin = falloffHeightNoise;
                float clampNoiseMax = falloffHeightNoise;
                
                if (fractalValue < clampNoiseMin) 
                {
                    return 0;
                }
                else if (fractalValue > clampNoiseMax) 
                {
                    return 1;
                }

                return fractalValue;
            }
            else
            {
                return 1;
            }
        }

        ENDHLSL

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4 frag( v2f i ) : SV_Target
            {
                float height = UnpackHeightmap(tex2D( _HeightTex, i.pcUV)) * 2;

                height = Lerp(_ClampMinHeight, _ClampMaxHeight, height);

                // need to adjust uvs due to "scaling" from brush rotation
                float2 pcUVRescale = float2( length( _PCUVToBrushUVScales.xy ), length( _PCUVToBrushUVScales.zw ) );

                // get noise mask value
                float2 noiseUV = ( i.pcUV - ( .5 ).xx ) * pcUVRescale + ( .5 ).xx + ( .5 ).xx * _NoiseTex_TexelSize.xy;
                float noiseValue = UnpackHeightmap( tex2D( _NoiseTex, i.pcUV ) );
                noiseValue = clamp(noiseValue, 0, 1);

                float blendHeightValue = 1;
                float result = 1;

                blendHeightValue = GetSharpenHeightNoise(noiseValue, _MaxRangeNoise, _MinRangeNoise, height, _MinHeight, _MaxHeight);
                blendHeightValue = clamp(blendHeightValue, 0, 1);

                switch (_BlendMode)
                {
                    case 0: //Multiply
                    {
                        result = UnpackHeightmap(tex2D(_BaseMaskTex, i.pcUV)) * blendHeightValue;

	            		break;
                    }
                    case 1: //Add
                    {
	            		result = UnpackHeightmap(tex2D(_BaseMaskTex, i.pcUV)) + blendHeightValue;

	            		break;
                    }
                    case 2: //Subtract
                    {
	            		result = UnpackHeightmap(tex2D(_BaseMaskTex, i.pcUV)) - blendHeightValue;

	            		break;
                    }
                }

                return clamp(result, 0, 1);
            }

            ENDHLSL
        }
    }
}
