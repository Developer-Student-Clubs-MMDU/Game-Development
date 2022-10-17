Shader "Hidden/MegaWorld/Slope"
{
    Properties 
    { 

    }

    SubShader
    {
        ZTest Always Cull OFF ZWrite Off

        HLSLINCLUDE

        #include "UnityCG.cginc"
        #include "TerrainTool.cginc"

        sampler2D _NormalTex;
        sampler2D _BaseMaskTex;

        int _BlendMode;

        float _MinSlope;
        float _MaxSlope;
        int _SlopeFalloffType;
        float _MinAddSlopeFalloff;
        float _MaxAddSlopeFalloff;

        struct appdata_t
        {
            float4 vertex : POSITION;
            float2 pcUV : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 pcUV : TEXCOORD0;
            float4 normalizePos : TEXCOORD1;
        };

        v2f vert( appdata_t v )
        {
            v2f o;
            
            o.vertex = UnityObjectToClipPos( v.vertex );
            o.pcUV = v.pcUV;
            o.normalizePos = v.vertex;

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
        
        float GetSharpenSlope(float slope, float minSlope, float maxSlope)
        {
            if (slope >= minSlope && slope <= maxSlope)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        
        float GetFalloffAddSlope(float slope, float minSlope, float maxSlope, float localMinAddSlopeFalloff, float localMaxAddSlopeFalloff)
        {
            if(slope > maxSlope)
            {
                float newMaxSlope = maxSlope + (localMaxAddSlopeFalloff);
        
                return InverseLerp(newMaxSlope, maxSlope, slope);
            }
            else if(slope < minSlope)
            {
                float newMinSlope = minSlope - (localMinAddSlopeFalloff);
        
                return InverseLerp(newMinSlope, minSlope, slope);
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
                float4 normalMapColor = tex2D(_NormalTex, i.normalizePos);
				float3 normalVector = normalMapColor * 2 - float3(1,1,1);
				float slopeAngle = acos(normalVector.y);
                float parameter = InverseLerp(0, 1.57, slopeAngle);
                slopeAngle = Lerp(0, 90, parameter);

                float slopeValue = 1;
                float result = 1;

                switch (_SlopeFalloffType)
                {
                    case 1: //FalloffAdd
                    {
	                	slopeValue = GetFalloffAddSlope(slopeAngle, _MinSlope, _MaxSlope, _MinAddSlopeFalloff, _MaxAddSlopeFalloff);
	                	break;
                    }
                    default:
                    {
	                	slopeValue = GetSharpenSlope(slopeAngle, _MinSlope, _MaxSlope);
	                	break;
                    }
                }

                slopeValue = clamp(slopeValue, 0, 1);

                switch (_BlendMode)
                {
                    case 0: //Multiply
                    {
                        result = UnpackHeightmap(tex2D(_BaseMaskTex, i.pcUV)) * slopeValue;

                		break;
                    }
                    case 1: //Add
                    {
                		result = UnpackHeightmap(tex2D(_BaseMaskTex, i.pcUV)) + slopeValue;

                		break;
                    }
                    case 2: //Subtract
                    {
                		result = UnpackHeightmap(tex2D(_BaseMaskTex, i.pcUV)) - slopeValue;

                		break;
                    }
                }

                return clamp(result, 0, 1);
            }

            ENDHLSL
        }
    }
}
