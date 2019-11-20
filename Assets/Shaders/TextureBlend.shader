//    Copyright (C) 2019 Timothy Ned Atton

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

Shader "Hidden/Custom/TextureBlend" {
	HLSLINCLUDE

		#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		TEXTURE2D_SAMPLER2D(_CaptureTex, sampler_CaptureTex);
		TEXTURE2D_SAMPLER2D(_BlendTex, sampler_BlendTex);
		float _BlendFuzzyRadius;
		float _StartTime;
		float _Period;

		float4 Frag(VaryingsDefault i) : SV_Target {
			float4 camera = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
			float4 captured = SAMPLE_TEXTURE2D(_CaptureTex, sampler_CaptureTex, i.texcoord);
			float blendCutoff = SAMPLE_TEXTURE2D(_BlendTex, sampler_BlendTex, i.texcoord);

			float blendValue = saturate((_Time.y - _StartTime) / _Period);
			float lerpValue = smoothstep(blendCutoff - _BlendFuzzyRadius, blendCutoff + _BlendFuzzyRadius, blendValue);
			return lerp(captured, camera, lerpValue);
		}

	ENDHLSL

	SubShader {
		Cull Off ZWrite Off ZTest Always

		Pass {
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag

			ENDHLSL
		}
	}
}
