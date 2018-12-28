Shader "Custom/Fish_Shadow"
{	
	Properties
	{
		_MainTex ("Diffuse (RGB)", 2D) = "grey" {}
		_MaskTex ("MaskTex", 2D) = "white" {}
		_LightTex("LightTex (RGB)", 2D) = "grey" {}
		_LightScale("LightScale", range(0.0,2.0)) = 0.35



		_NoiseTex ("EffectTex", 2D) = "white" {}
		_Scroll2X ("EffectOffsetX", Float) = 1.0
		_Scroll2Y ("EffectOffsetY", Float) = 0.0
		_NoiseColor("EffectColor", Color) = (1,1,1,1)
		_MMultiplier ("EffectPower", Float) = 2.0
		[Toggle]_HurtSwitch("HurtSwitch", Float) = 0

		[Toggle]_FrozenSwitch("FrozenSwith",Float) = 0
		_AlphaFish("AlphaFish", Range(0,1)) = 1

		_Blend("Blend",Range(0,1)) = 0.1
		_FrozenTex("FrozenTex",2D)= "white"{}
		_FrozenLight("FrozenLightTex",2D) = "white"{}
		_FrozenLightBlend("FrozenBlend",Range(0,1))= 0.6


		_ShadowPlane("Shadow Plane", vector) = (0,1,0,0)		
		_ShadowProjDir("ShadowProjDir", vector) = (0,0,0,0)
		_ShadowFadeParams("ShadowFadeParams", vector) = (0,0,0,0)
		_ShadowInvLen("ShadowInvLen", float) = 0.2
		


	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		// LOD 500
		Fog { Mode Off }
		Cull Back

		Pass
		{
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha 

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile _DUMMY _HURT_EFFECT_ON

				#include "Assets/Shaders/CG_FishShader.cginc"
				
			ENDCG
		}
		
		// Pass
		// {
		// 	Stencil
		// 	{
		// 		Ref 0
		// 		Comp equal
		// 		Pass incrWrap
		// 		Fail keep
		// 		ZFail keep
		// 		
		// 	}
		// 	Blend SrcAlpha OneMinusSrcAlpha
		// 	ColorMask RGB
		// 	ZWrite off
		// 	Cull Back
		// 
		// 	CGPROGRAM
		// 		#pragma vertex vert
		// 		#pragma fragment frag
		// 		#include "Assets/Shaders/CG_FakeShadow.cginc"
		// 	ENDCG
		// }

	}

	// SubShader
	// {
	// 
	// Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	// 	LOD 100
	// 	Fog { Mode Off }
	// 	Cull Back
	// 
	// 	Pass
	// 	{
	// 		Cull Off
	// 		CGPROGRAM
	// 			#pragma vertex vert
	// 			#pragma fragment frag
	// 			#pragma multi_compile _DUMMY _HURT_EFFECT_ON
	// 			#include "Assets/Shaders/CG_FishShader.cginc"
	// 			
	// 		ENDCG
	// 	}
	// 
	// }


}

