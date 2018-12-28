// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


#pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile _DUMMY _NOISETEX_ON


#include "UnityCG.cginc"

		sampler2D _NoiseTex;
		sampler2D _MaskTex;
		sampler2D _FrozenTex;

		float4 _MainTex_ST;
		float4 _NoiseTex_ST;
		float4 _FrozenTex_ST;

		float _Blend;
		float _FrozenLightBlend;
		half3 _NoiseColor;
		//half3 _HurtColor;
		half  _HurtSwitch;
		half _FrozenSwitch;
		half _AlphaFish; // fish alpha
		half _Scroll2X;
		half _Scroll2Y;
		half _MMultiplier;

struct v2f
{
	float4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
	half3 normalVS: TEXCOORD2;
	half2 uv2 :TEXCOORD1;
	float4 posWorld : TEXCOORD4;
	float3 normalWorld : TEXCOORD5;



};

v2f vert(appdata_base v)
{
	v2f o;
	UNITY_INITIALIZE_OUTPUT(v2f, o);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv  =TRANSFORM_TEX(v.texcoord, _MainTex);
	o.uv2 =TRANSFORM_TEX(v.texcoord,_FrozenTex);
	o.normalVS = mul(UNITY_MATRIX_IT_MV, float4(v.normal,0)).xyz;

	return o;
}

		sampler2D _MainTex;
		sampler2D _LightTex;
		sampler2D _FrozenLight;
		half _LightScale;



// half4 _RimColor;

fixed4 frag(v2f i) : COLOR0
{
    half2 litTexUV = normalize(i.normalVS).xy * 0.5 + 0.5;
    half2 litTexUV2 = normalize(i.normalVS).xy * 0.5 + 0.5;
	fixed4 color = tex2D(_MainTex, i.uv);

	fixed4 Light = tex2D(_LightTex, litTexUV);
	fixed4 FrozenLight = tex2D(_FrozenLight,litTexUV2);

	//if(_FrozenSwitch) 
	//{
	// fixed Light = tex2D(_FrozenTex, litTexUV);
	//}

	float tmpSwitch = step(0.1, _FrozenSwitch);
	float tmpSwitchReverse = 1 - tmpSwitch;

	fixed4 LightFroze = tex2D(_FrozenTex, litTexUV);
	Light.r = Light.r * tmpSwitchReverse + LightFroze.r * tmpSwitch;
	Light.g = Light.g * tmpSwitchReverse + LightFroze.g * tmpSwitch;
	Light.b = Light.b * tmpSwitchReverse + LightFroze.b * tmpSwitch;


	fixed4 Base = tex2D(_MainTex, i.uv.xy);
	fixed4 Mask = tex2D(_MaskTex, i.uv.xy);
	half3 albedo = (Base.rgb+0.15)* Light.rgb ;
	albedo += Base.rgb;

	half2 flowUV = TRANSFORM_TEX(i.uv, _NoiseTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time.x);
	half3 noise = tex2D(_NoiseTex, flowUV.xy).rgb;
	noise *= _NoiseColor.rgb;
	noise *= Mask.g * _MMultiplier;
	half2 uv = normalize(i.normalVS).xy * 0.5 + 0.5;
	fixed4 light = tex2D(_LightTex,uv);
	color.rgb += light.rgb * _LightScale + noise;


	// if (_FrozenSwitch)
//	{
	 fixed4 frozen = tex2D(_FrozenTex, i.uv2); 
	 //color.rgb = lerp(color, frozen*2.3,_Blend );
	 color.rgb = color.rgb + frozen * _Blend * tmpSwitch;
//	 fixed4 frozenLig = lerp(0, FrozenLight,_FrozenLightBlend);
//	 color +=frozenLig;
	 color.rgb = lerp(color, FrozenLight.rgb, FrozenLight.a*_FrozenLightBlend*tmpSwitch);
//	}


	// hurt effect
    tmpSwitch = step(0.1, _HurtSwitch);
	// if(_HurtSwitch)
//	{ 
		//float gray = 0.2126*color.r + 0.7152*color.g + 0.0722*color.b;
		
		float witLevel = 0.3 * tmpSwitch;
		
		color.r = saturate(color.r + witLevel);
		color.g = saturate(color.g + witLevel);
		color.b = saturate(color.b + witLevel);

//	}
	

	color.a = _AlphaFish;

	
	return color;
}
