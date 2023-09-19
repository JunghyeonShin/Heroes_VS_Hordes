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

    private const float MIN_DAMAGE_TEXT_POSITION_X = -1f;
    private const float MAX_DAMAGE_TEXT_POSITION_X = 1f;
    private const float DAMAGE_TEXT_POSITION_Y = 1f;
    private const float TWO_MULTIPLES_VALUE = 2f;
    private const float DELAY_FADE_IN_TIME = 0.4f;
    private const float DELAY_FADE_OUT_TIME = 0.1f;
    private const float ALPHA_ZERO = 0f;
    private const float ALPHA_ONE = 1f;
    private const float FADE_IN_TIME = 5f;
    private const float DEVIDE_HALF = 0.5f;
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;

    private readonly Vector3 ADJUST_MID_POSITION_Y = new Vector3(0f, 0.5f, 0f);
    private readonly Color BOMB_ALLERT_SPRITE_COLOR = new Color(255f / 255f, 123f / 255f, 0f, ALPHA_ZERO);

    public void Init(Vector3 initPos, Vector3 targetPos, float speed, float attack, float effectRange)
    {
        transform.position = initPos;
        _targetPos = targetPos;
        _speed = speed;
        _attack = attack;
        _effectRange = effectRange;

        Utils.SetActive(_bombSprite.gameObject, true);
        _bombAllertSprite.transform.position = _targetPos;
        _bombAllertSprite.transform.localScale = new Vector3(_effectRange, _effectRange, 1f);
        _bombAllertSprite.color = BOMB_ALLERT_SPRITE_COLOR;
        _allertBomb = true;
        _AllertBombArea().Forget();
        _AttackRange().Forget();
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

        _bombAllertSprite.color = BOMB_ALLERT_SPRITE_COLOR;
        _AllertBombArea().Forget();
    }

    private async UniTaskVoid _AttackRange()
    {
        var startPos = transform.position;
        var midPos = (_targetPos + startPos) * DEVIDE_HALF + ADJUST_MID_POSITION_Y;
        var time = ZERO_SECOND;
        while (time < ONE_SECOND)
        {
            time += _speed * Time.fixedDeltaTime;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;

            transform.position = BezierCurve.QuadraticCurve(startPos, midPos, _targetPos, time);
            _bombAllertSprite.transform.position = _targetPos;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedDeltaTime), delayTiming: PlayerLoopTiming.FixedUpdate);
        }
        _allertBomb = false;

        var layerMask = 1 << LayerMask.NameToLayer(Define.LAYER_MONSTER);
        var monsters = Physics2D.OverlapCircleAll(_targetPos, _effectRange, layerMask);
        foreach (var monster in monsters)
        {
            var randomPos = new Vector3(UnityEngine.Random.Range(MIN_DAMAGE_TEXT_POSITION_X, MAX_DAMAGE_TEXT_POSITION_X), DAMAGE_TEXT_POSITION_Y, 0f);
            var initDamageTextPos = monster.transform.position + randomPos;
            var damageTextGO = Manager.Instance.Object.GetDamageText();
            var damageText = Utils.GetOrAddComponent<DamageText>(damageTextGO);
            var attack = _attack;
            var isCritical = _testHeroController.IsCritical();
            if (isCritical)
                attack = _attack * TWO_MULTIPLES_VALUE;
            damageText.FloatDamageText(initDamageTextPos, attack, isCritical);
            Utils.SetActive(damageTextGO, true);

            var testMonster = Utils.GetOrAddComponent<TestMonster>(monster.gameObject);
            testMonster.OnDamaged(attack);
        }
        Utils.SetActive(_bombSprite.gameObject, false);

        // Æø¹ß ÀÌÆåÆ® Ãß°¡

        FinishAttackHandler?.Invoke();
    }
}
