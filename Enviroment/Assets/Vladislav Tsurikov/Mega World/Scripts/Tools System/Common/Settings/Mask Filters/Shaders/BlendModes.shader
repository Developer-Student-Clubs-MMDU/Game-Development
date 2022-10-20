Shader "Hidden/MegaWorld/BlendModes"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
    }

    SubShader
    {
        ZTest Always Cull Off ZWrite Off

        HLSLINCLUDE

        #include "UnityCG.cginc"
		#include "TerrainTool.cginc"

        sampler2D _MainTex;
		sampler2D _BlendTex;

        int _BlendMode;

        struct appdata_t 
        {
            float4 vertex : POSITION;
            float2 pcUV : TEXCOORD0;
        };

        struct v2f {
            float4 vertex : SV_POSITION;
            float2 pcUV : TEXCOORD0;
        };

        v2f vert(appdata_t v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.pcUV = v.pcUV;
            return o;
        }

        ENDHLSL

        Pass // 0 - Multiply
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment Blend

            float4 Blend(v2f i) : SV_Target
            {
                float result = 1;

                float mainValue = UnpackHeightmap(tex2D(_MainTex, i.pcUV));
                float blendValue = UnpackHeightmap(tex2D(_BlendTex, i.pcUV));

                switch (_BlendMode)
                {
                    case 0: //Multiply
                    {
                        result = mainValue * blendValue;

	            		break;
                    }
                    case 1: //Add
                    {
	            		result = mainValue + blendValue;

	            		break;
                    }
                    case 2: //Subtract
                    {
	            		result = mainValue - blendValue;

	            		break;
                    }
                }

                return clamp(result, 0, 1);
            }

            ENDHLSL
        }
    }
}