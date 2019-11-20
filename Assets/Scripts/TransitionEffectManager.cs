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

public class TransitionEffectManager : MonoBehaviour {

    public static TransitionEffectManager Instance {
        get;
        private set;
    }

    //[SerializeField]
    //private Camera mainCamera;
    [SerializeField]
    private Texture2D blendTexture;
    [SerializeField]
    private float blendFuzzyRadius;

    private PostProcessVolume volume;
    private CaptureTransitionPPEffect effect;
    private RenderTexture renderTexture;
    private float startTime, period;
    private float loadTimer;

    private void Awake() {
        if(Instance == null) {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else {
            GameObject.DestroyImmediate(gameObject);
        }
    }

    void Start() {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 1);

        effect = ScriptableObject.CreateInstance<CaptureTransitionPPEffect>();
        effect.enabled.Override(true);
        effect.captureTex.Override(renderTexture);
        effect.blendTex.Override(blendTexture);
        effect.blendFuzzyRadius.Override(blendFuzzyRadius);
        DontDestroyOnLoad(effect);
    }

    public void CaptureAndStartTransition(float effectPeriod, bool isNewScene=false) {
        Camera mainCamera = Camera.main;
        mainCamera.targetTexture = renderTexture;
        mainCamera.Render();
        mainCamera.targetTexture = null;

        if(volume == null) {
            effect.enabled.Override(true);

            volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, effect);
            DontDestroyOnLoad(volume);
        }

        startTime = isNewScene ? 0 : Time.timeSinceLevelLoad;
        period = effectPeriod;
        effect.startTime.Override(startTime);
        effect.period.Override(period);
        loadTimer = 0.1f;
    }

    void Update() {
        if(volume != null) {
            loadTimer -= Time.deltaTime;
            float blendPercentage = (Time.timeSinceLevelLoad - startTime) / period;
            if(blendPercentage >= 1 && loadTimer <= 0) {
                CleanUp();
            }
        }
    }

    private void CleanUp() {
        RuntimeUtilities.DestroyVolume(volume, false, true);
        effect.enabled.Override(false);
    }

    void OnDestroy() {
        if(volume != null) {
            CleanUp();
        }
        if(renderTexture != null && renderTexture.IsCreated()) {
            renderTexture.Release();
        }
        if(effect != null) {
            GameObject.Destroy(effect);
        }
    }
}