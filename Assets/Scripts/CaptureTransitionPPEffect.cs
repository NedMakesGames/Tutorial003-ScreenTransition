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

using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(CaptureTransitionRenderer), PostProcessEvent.AfterStack, "Custom/CaptureTransition")]
public sealed class CaptureTransitionPPEffect : PostProcessEffectSettings {
    public TextureParameter captureTex = new TextureParameter() { defaultState = TextureParameterDefault.Black };
    public TextureParameter blendTex = new TextureParameter() { defaultState = TextureParameterDefault.White };
    [Range(0f, 0.5f), Tooltip("Transition fuzzy radius")]
    public FloatParameter blendFuzzyRadius = new FloatParameter() { value = 0.05f };
    public FloatParameter startTime = new FloatParameter();
    public FloatParameter period = new FloatParameter() { value = 1 };
}

public sealed class CaptureTransitionRenderer : PostProcessEffectRenderer<CaptureTransitionPPEffect> {
    public override void Render(PostProcessRenderContext context) {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/TextureBlend"));
        sheet.properties.SetTexture("_CaptureTex", settings.captureTex);
        sheet.properties.SetTexture("_BlendTex", settings.blendTex);
        sheet.properties.SetFloat("_BlendFuzzyRadius", settings.blendFuzzyRadius);
        sheet.properties.SetFloat("_StartTime", settings.startTime);
        sheet.properties.SetFloat("_Period", settings.period);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
