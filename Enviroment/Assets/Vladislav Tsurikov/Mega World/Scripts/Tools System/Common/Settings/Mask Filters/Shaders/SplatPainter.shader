﻿Shader "Hidden/MegaWorld/SplatPainter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
		_Mask ("Mask", 2D) = "white" {}
		_Opacity ("Opacity", Float) = 1
		_ChannelIndex ("Channel Index", Int) = 0
    }

	CGINCLUDE
    #include "UnityCG.cginc"
	struct appdata
    {
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
    };

    struct v2f
    {
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float4 localPos : TEXCOORD1;
    };

    sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	sampler2D _Mask;
	float _Opacity;
	int _ChannelIndex;

	v2f vert (appdata v)
    {
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		o.localPos = v.vertex;
		return o;
    }

	fixed4 fragAdd (v2f i) : SV_Target
    {
		float4 maskColor = tex2D(_Mask, i.uv);
		float maskGray = maskColor.r;
		float strength = maskGray*_Opacity;
		float4 channel = float4(_ChannelIndex==0, _ChannelIndex==1, _ChannelIndex==2, _ChannelIndex==3);
		float4 currentColor = tex2D(_MainTex, i.localPos);

		return lerp(currentColor, channel, strength);
	}

	fixed4 fragSubSelective (v2f i) : SV_Target
    {
		float4 currentColor = tex2D(_MainTex, i.localPos);
		float4 maskColor = tex2D(_Mask, i.uv);
		float maskGray = maskColor.r;
		float strength = maskGray*_Opacity;
		float4 channel = float4(_ChannelIndex==0, _ChannelIndex==1, _ChannelIndex==2, _ChannelIndex==3);
		channel = (1 - channel)*currentColor;
		return lerp(currentColor, channel, strength);
	}

	fixed4 fragSub (v2f i) : SV_Target
    {
		float4 currentColor = tex2D(_MainTex, i.localPos);
		float4 maskColor = tex2D(_Mask, i.uv);
		float maskGray = maskColor.r;
		float strength = maskGray*_Opacity;
		float4 channel = 0;
		return lerp(currentColor, channel, strength);
	}
	ENDCG

    SubShader
    {
        Tags { "RenderType"="Transparent" }
		
        Pass
        {
			Name "Add"
			Blend One Zero
			BlendOp Add
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragAdd
            ENDCG
        }

		Pass
        {
			Name "Sub Selective"
			Blend One Zero
			BlendOp Add
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragSubSelective
            ENDCG
        }

		Pass
        {
			Name "Sub"
			Blend One Zero
			BlendOp Add
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragSub
            ENDCG
        }
    }
}