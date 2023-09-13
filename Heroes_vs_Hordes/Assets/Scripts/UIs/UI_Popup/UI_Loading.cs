using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Loading : UI_Popup
{
    private enum ETexts
    {
        LoadingText
    }

    private TextMeshProUGUI _loadingText;
    private bool _loading;
    private bool _fadeOut;

    private const float INIT_ALPHA = 0.01f;
    private const float ALPHA_ZERO = 0f;
    private const float ALPHA_ONE = 1f;
    private const float DELAY_TIME = 0.5f;
    private const float PROGRESS_TIME = 0.02f;
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;

    protected override void _Init()
    {
        _BindText(typeof(ETexts));

        _loadingText = _GetText((int)ETexts.LoadingText);
    }

    public void StartLoading()
    {
        _loading = true;
        _fadeOut = true;
        _loadingText.alpha = INIT_ALPHA;
        _Loading().Forget();
    }

    public void CompleteLoading()
    {
        _loading = false;
    }

    private async UniTaskVoid _Loading()
    {
        var time = ZERO_SECOND;
        while (_loading)
        {
            if (ALPHA_ZERO == _loadingText.alpha)
            {
                _fadeOut = true;
                time = ZERO_SECOND;
                await UniTask.Delay(TimeSpan.FromSeconds(DELAY_TIME));
            }
            else if (ALPHA_ONE == _loadingText.alpha)
            {
                _fadeOut = false;
                time = ZERO_SECOND;
                await UniTask.Delay(TimeSpan.FromSeconds(DELAY_TIME));
            }

            time += PROGRESS_TIME;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;

            if (_fadeOut)
                _Fading(ALPHA_ZERO, ALPHA_ONE, time);
            else
                _Fading(ALPHA_ONE, ALPHA_ZERO, time);
            await UniTask.Delay(TimeSpan.FromSeconds(PROGRESS_TIME));
        }
    }

    private void _Fading(float minAlpha, float maxAlpha, float time)
    {
        _loadingText.alpha = Mathf.Lerp(minAlpha, maxAlpha, time);
    }
}
