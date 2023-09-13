using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DamageText : MonoBehaviour
{
    private RectTransform _rectTransform;
    private TextMeshPro _damageText;

    private const float ALPHA_ZERO = 0f;
    private const float ALPHA_ONE = 1f;
    private const float PROGRESS_SPEED = 0.02f;
    private const float FADE_IN_SPEED = 8f;
    private const float FADE_OUT_SPEED = 4f;
    private const float WAIT_TIME = 0.05f;

    private readonly Color CRITICAL_COLOR = new Color(255f, 127f, 0f);

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _damageText = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        _FloatingDamageText().Forget();
    }

    public void Init(Vector3 initPos, float value, bool isCritical)
    {
        _rectTransform.anchoredPosition = initPos;

        if (isCritical)
            _SetDamageText($"{value}!", CRITICAL_COLOR);
        else
            _SetDamageText($"{value}", Color.white);
    }

    private async UniTaskVoid _FloatingDamageText()
    {
        _damageText.alpha = ALPHA_ZERO;
        await UniTask.Delay(TimeSpan.FromSeconds(WAIT_TIME));

        // Fade-In
        var alpha = ALPHA_ZERO;
        while (alpha < ALPHA_ONE)
        {
            alpha += PROGRESS_SPEED * FADE_IN_SPEED;
            if (alpha >= ALPHA_ONE)
                alpha = ALPHA_ONE;

            _damageText.alpha = Mathf.Lerp(ALPHA_ZERO, ALPHA_ONE, alpha);
            await UniTask.Delay(TimeSpan.FromSeconds(PROGRESS_SPEED));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(WAIT_TIME));

        // Fade-Out
        alpha = ALPHA_ONE;
        while (alpha > ALPHA_ZERO)
        {
            alpha -= PROGRESS_SPEED * FADE_OUT_SPEED;
            if (alpha <= ALPHA_ZERO)
                alpha = ALPHA_ZERO;

            _damageText.alpha = Mathf.Lerp(ALPHA_ZERO, ALPHA_ONE, alpha);
            await UniTask.Delay(TimeSpan.FromSeconds(PROGRESS_SPEED));
        }

        Manager.Instance.Object.ReturnDamageText(gameObject);
    }

    private void _SetDamageText(string text, Color color)
    {
        _damageText.text = text;
        _damageText.color = color;
    }
}
