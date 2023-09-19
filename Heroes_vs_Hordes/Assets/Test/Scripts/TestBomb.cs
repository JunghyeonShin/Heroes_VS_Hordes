using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBomb : MonoBehaviour
{
    public event Action FinishAttackHandler;

    [SerializeField] private TestHeroController _testHeroController;
    [SerializeField] private SpriteRenderer _bombSprite;
    [SerializeField] private SpriteRenderer _bombAllertSprite;

    private Vector3 _targetPos;
    private float _speed;
    private float _attack;
    private float _effectRange;

    [SerializeField] private bool _allertBomb;

    private const float DELAY_FADE_IN_TIME = 0.3f;
    private const float DELAY_FADE_OUT_TIME = 1f;
    private const float ALPHA_ZERO = 0f;
    private const float ALPHA_ONE = 1f;
    private const float FADE_IN_TIME = 3f;
    private const float FADE_OUT_TIME = 5f;

    private readonly Color BOMB_ALLERT_SPRITE_COLOR = new Color(255f / 255f, 123f / 255f, 0f, ALPHA_ZERO);

    public void Init(Vector3 initPos, Vector3 targetPos, float speed, float attack, float effectRange)
    {
        transform.position = initPos;
        _targetPos = targetPos;
        _speed = speed;
        _attack = attack;
        _effectRange = effectRange;

        _bombAllertSprite.transform.position = _targetPos;
        _bombAllertSprite.transform.localScale = new Vector3(_effectRange, _effectRange, 1f);
        _bombAllertSprite.color = BOMB_ALLERT_SPRITE_COLOR;
        _allertBomb = true;
        _AllertBombArea().Forget();
    }

    private async UniTaskVoid _AllertBombArea()
    {
        if (false == _allertBomb)
            return;

        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_FADE_IN_TIME));

        // Fade-In
        var alpha = ALPHA_ZERO;
        while (alpha < ALPHA_ONE)
        {
            alpha += FADE_IN_TIME * Time.deltaTime;
            if (alpha >= ALPHA_ONE)
                alpha = ALPHA_ONE;

            _bombAllertSprite.color = new Color(BOMB_ALLERT_SPRITE_COLOR.r, BOMB_ALLERT_SPRITE_COLOR.g, BOMB_ALLERT_SPRITE_COLOR.b, alpha);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_FADE_OUT_TIME));

        // Fade-Out
        alpha = ALPHA_ONE;
        while (alpha > ALPHA_ZERO)
        {
            alpha -= FADE_OUT_TIME * Time.deltaTime;
            if (alpha <= ALPHA_ZERO)
                alpha = ALPHA_ZERO;

            _bombAllertSprite.color = new Color(BOMB_ALLERT_SPRITE_COLOR.r, BOMB_ALLERT_SPRITE_COLOR.g, BOMB_ALLERT_SPRITE_COLOR.b, alpha);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }

        _AllertBombArea().Forget();
    }

}
