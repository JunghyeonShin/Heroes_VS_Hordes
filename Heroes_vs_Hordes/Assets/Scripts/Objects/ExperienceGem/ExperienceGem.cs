using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    private Collider2D _collider;

    private const float DIVIDE_HALF_VALUE = 0.5f;
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;
    private const float PROGRESS_TIME = 0.02f;
    private const float EXPERIENCE_DISTANCE = 0.3f;
    private const float LINEAR_CURVE_DISTANCE = 1f;
    private const float INCREASE_EXP_VALUE = 1;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        _collider.enabled = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_HERO))
        {
            _collider.enabled = false;
            _GiveExperience(collision.GetComponent<Hero>()).Forget();
        }
    }

    public void Init(Vector3 initPos)
    {
        transform.position = initPos;
    }

    private async UniTaskVoid _GiveExperience(Hero targetHero)
    {
        var tempPos = Vector3.zero;
        var startPos = transform.position;
        var distance = Vector3.Distance(startPos, targetHero.transform.position);
        if (distance <= LINEAR_CURVE_DISTANCE)
            tempPos = (startPos + targetHero.transform.position) * DIVIDE_HALF_VALUE;
        else
            tempPos = startPos + Vector3.Slerp(transform.up, (targetHero.transform.position - startPos).normalized, 0.3f);

        var time = ZERO_SECOND;
        while (time < ONE_SECOND)
        {
            time += PROGRESS_TIME;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;

            distance = Vector3.Distance(transform.position, targetHero.transform.position);
            if (distance <= EXPERIENCE_DISTANCE)
                break;
            else if (EXPERIENCE_DISTANCE < distance && distance <= LINEAR_CURVE_DISTANCE)
                transform.position = BezierCurve.LinearCurve(startPos, targetHero.transform.position, time);
            else
                transform.position = BezierCurve.QuadraticCurve(startPos, tempPos, targetHero.transform.position, time);
            await UniTask.Delay(TimeSpan.FromSeconds(PROGRESS_TIME), ignoreTimeScale: true);
        }

        targetHero.GetExp(INCREASE_EXP_VALUE);
        Manager.Instance.Object.ReturnExperienceGem(gameObject);
    }
}
