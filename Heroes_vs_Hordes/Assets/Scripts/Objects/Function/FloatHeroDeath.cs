using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatHeroDeath : MonoBehaviour
{
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;
    private const float DELAY_DISABLE_HERO_DEATH = 1f;

    public void SetTransform(Vector3 initPos)
    {
        transform.position = initPos;
        _Float().Forget();
    }

    private async UniTaskVoid _Float()
    {
        while (false == gameObject.activeSelf)
            await UniTask.Yield();

        var time = ZERO_SECOND;
        var startPos = transform.position;
        var endPos = transform.position + Vector3.up;
        while (time < ONE_SECOND)
        {
            time += Time.deltaTime;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;
            transform.position = Vector3.Lerp(startPos, endPos, time);
            transform.localScale = new Vector3(time, time, time);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_DISABLE_HERO_DEATH));

        Utils.SetActive(gameObject, false);
    }
}
