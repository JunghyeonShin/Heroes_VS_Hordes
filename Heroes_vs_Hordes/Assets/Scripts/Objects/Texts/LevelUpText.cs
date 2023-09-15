using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LevelUpText : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Transform _heroTransform;

    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;
    private const float PROGRESS_SPEED = 0.02f;
    private const float DISABLED_TIME = 0.5f;
    private const float FLOAT_SPEED = 1.5f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void FloatLevelUpText(Transform heroTransform)
    {
        _heroTransform = heroTransform;
        _rectTransform.anchoredPosition = _heroTransform.position + Vector3.up;
        _FloatingLevelUpText().Forget();
    }

    private async UniTaskVoid _FloatingLevelUpText()
    {
        var time = ZERO_SECOND;
        while (time < ONE_SECOND)
        {
            time += PROGRESS_SPEED;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;
            _rectTransform.anchoredPosition = _heroTransform.position + new Vector3(0f, FLOAT_SPEED * time, 0f);
            await UniTask.Delay(TimeSpan.FromSeconds(PROGRESS_SPEED), ignoreTimeScale: true, delayTiming: PlayerLoopTiming.LastUpdate);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(DISABLED_TIME), ignoreTimeScale: true);

        Utils.SetActive(gameObject, false);
    }
}
