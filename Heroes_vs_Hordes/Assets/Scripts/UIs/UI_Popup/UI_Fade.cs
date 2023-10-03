using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Fade : UI_Popup
{
    private enum EImages
    {
        FadePanel
    }

    private Image _fadeImage;

    private bool _fadeOut;
    private bool _fadeIn;

    private const float ALPHA_ZERO = 0f;
    private const float ALPHA_ONE = 1f;

    protected override void _Init()
    {
        _BindImage(typeof(EImages));

        _fadeImage = _GetImage((int)EImages.FadePanel);
    }

    public void FadeOut(float endTime, Action fadeAction)
    {
        _FadeOut(endTime, fadeAction).Forget();
    }

    public void FadeIn(float endTime, Action fadeAction)
    {
        _FadeIn(endTime, fadeAction).Forget();
    }

    private async UniTaskVoid _FadeOut(float endTime, Action fadeAction)
    {
        while (_fadeIn)
            await UniTask.Yield();

        _fadeOut = true;
        fadeAction -= () => { _fadeOut = false; };
        fadeAction += () => { _fadeOut = false; };
        _Fade(ALPHA_ZERO, ALPHA_ONE, endTime, fadeAction).Forget();
    }

    private async UniTaskVoid _FadeIn(float endTime, Action fadeAction)
    {
        while (_fadeOut)
            await UniTask.Yield();

        _fadeIn = true;
        fadeAction -= () => { _fadeIn = false; };
        fadeAction += () => { _fadeIn = false; };
        _Fade(ALPHA_ONE, ALPHA_ZERO, endTime, fadeAction).Forget();
    }

    private async UniTaskVoid _Fade(float startAlpha, float endAlpha, float endTime, Action fadeAction)
    {
        var time = 0f;
        while (time < endTime)
        {
            time += Time.deltaTime;
            if (time >= endTime)
                time = endTime;

            var alpha = Mathf.Lerp(startAlpha, endAlpha, time / endTime);
            _fadeImage.color = new Color(0f, 0f, 0f, alpha);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }
        fadeAction?.Invoke();
    }
}
