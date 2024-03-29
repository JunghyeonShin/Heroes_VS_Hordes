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

    public event Action OnLoadCompleteHandler;

    private TextMeshProUGUI _loadingText;
    private bool _loading;
    private bool _fadeOut;

    private const float ALPHA_ZERO = 0f;
    private const float ALPHA_ONE = 1f;
    private const float DELAY_TIME = 0.5f;
    private const float DELAY_LOAD_COMPLETE_TIME = 1f;
    private const float DELAY_LOAD_COMPLETE_EVENT_TIME = 0.2f;
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
        _loadingText.alpha = ALPHA_ZERO;
        Utils.SetActive(_loadingText.gameObject, true);
        _Loading().Forget();
    }

    public void CompleteLoading()
    {
        _loading = false;
        _LoadComplete().Forget();
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

    private async UniTaskVoid _LoadComplete()
    {
        Utils.SetActive(_loadingText.gameObject, false);
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_LOAD_COMPLETE_TIME));

        ClosePopupUI();
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_LOAD_COMPLETE_EVENT_TIME));

        OnLoadCompleteHandler?.Invoke();
        OnLoadCompleteHandler -= Manager.Instance.Ingame.StartIngame;
    }
}
