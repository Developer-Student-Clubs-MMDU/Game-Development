Shader "Hidden/MegaWorld/PaintMaterialBrushPreview"
{
    SubShader
    {
        ZTest Always Cull Back ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGINCLUDE
            // Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
            #pragma exclude_renderers gles

            #include "UnityCG.cginc"
            #include "TerrainPreview.cginc"

            sampler2D _BrushTex;
            float4 _Color;
            int _EnableBrushStripe;
            int _ColorSpace;
            int _AlphaVisualisationType;
            float _Alpha;

        ENDCG

        Pass    // 0
        {
            Name "PaintMaterialTerrainPreview"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct v2f {
                float4 clipPosition : SV_POSITION;
                float3 positionWorld : TEXCOORD0;
                float3 positionWorldOrig : TEXCOORD1;
                float2 pcPixels : TEXCOORD2;
                float2 brushUV : TEXCOORD3;
            };

            v2f vert(uint vid : SV_VertexID)
            {
                // build a quad mesh, with one vertex per paint context pixel (pcPixel)
                float2 pcPixels = BuildProceduralQuadMeshVertex(vid);

                // compute heightmap UV and sample heightmap
                float2 heightmapUV = PaintContextPixelsToHeightmapUV(pcPixels);
                float heightmapSample = UnpackHeightmap(tex2Dlod(_Heightmap, float4(heightmapUV, 0, 0)));

                // compute brush UV
                float2 brushUV = PaintContextPixelsToBrushUV(pcPixels);

                // compute object position (in terrain space) and world position
                float3 positionObject = PaintContextPixelsToObjectPosition(pcPixels, heightmapSample);
                float3 positionWorld = TerrainObjectToWorldPosition(positionObject);

                v2f o;
                o.pcPixels = pcPixels;
                o.positionWorld = positionWorld;
                o.positionWorldOrig = positionWorld;
                o.clipPosition = UnityWorldToClipPos(positionWorld);
                o.brushUV = brushUV;
                return o;
            }

            float Lerp(float v0, float v1, float t) 
            {
                return (1 - t) * v0 + t * v1;
            }

            float4 ColorLerp(float4 colorA, float4 colorB, float t)
            {
                float r = Lerp(colorA.r, colorB.r, t);
                float g = Lerp(colorA.g, colorB.g, t);
                float b = Lerp(colorA.b, colorB.b, t);

                return float4(r, g, b, 1.0f);
            }

            float GetAlpha(float4 color, float brushSample)
            {
                if(_AlphaVisualisationType == 0)
                {
                    color.a = 1.0f * saturate(brushSample * 5.0f);
                }
                else if(_AlphaVisualisationType == 1)
                {
                    color.a = brushSample;
                }

                color.a = Lerp(0, color.a, _Alpha);

                return color.a;
            }

            float GetAlphaForCustomColor(float4 color, float brushSample)
            {
                if(_AlphaVisualisationType == 0)
                {
                    color.a = 1.0f * saturate(brushSample * 5.0f);
                }
                else if(_AlphaVisualisationType == 1)
                {
                    color.a = brushSample;
                }
                else
                {
                    color.a = _Alpha;
                }

                return color.a;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = float4(1.0f, 0.6f, 0.05f, 1.0f);

                float brushSample = UnpackHeightmap(tex2D(_BrushTex, i.brushUV));

                brushSample = clamp(brushSample, 0, 1);

                if(_ColorSpace == 0) //CustomColor
                {
                    // out of bounds multiplier
                    float oob = all(saturate(i.brushUV) == i.brushUV) ? 1.0f : 0.0f;

                    float brushStripe = 0;

                    if(_EnableBrushStripe == 1)
                    {
                        // brush outline stripe
                        float stripeWidth = 0.0f;       // pixels
                        float stripeLocation = 0.2f;    // at 20% alpha
                        brushStripe = Stripe(brushSample, stripeLocation, stripeWidth);
                    }

                    color = _Color * saturate(2.0f * brushStripe + 1.0f * brushSample);

                    color.a = GetAlphaForCustomColor(color, brushSample);

                    //color.a = 1.0f * saturate(brushSample * 5.0f);
                    color *= oob;
                }
                else if(_ColorSpace == 1) //Colorful
                {
                    // out of bounds multiplier
                    float oob = all(saturate(i.brushUV) == i.brushUV) ? 1.0f : 0.0f;    

                    float4 colorRed = float4(1, 0, 0, 1);
                    float4 colorYellow = float4(1, 1, 0, 1);
                    float4 colorGreen = float4(0, 1, 0, 1);

                    if(brushSample < 0.5)
                    {
                        float difference = brushSample / 0.5f;
                        color = ColorLerp(colorRed, colorYellow, difference);
                    }
                    else
                    {
                        float difference = (brushSample - 0.5f) / 0.5f;
                        color = ColorLerp(colorYellow, colorGreen, difference);
                    }
    
                    color.a = GetAlpha(color, brushSample);

                    color *= oob;
                }
                else if(_ColorSpace == 2) //Heightmap
                {
                    // out of bounds multiplier
                    float oob = all(saturate(i.brushUV) == i.brushUV) ? 1.0f : 0.0f;

                    color = float4(brushSample, brushSample, brushSample, 1.0f); 

                    color.a = GetAlpha(color, brushSample);
                    
                    color *= oob;
                }
                
				return color;
            }
            ENDCG
        }
    }
    Fallback Off
}