using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDivineAura : MonoBehaviour
{
    [SerializeField] private TestHeroController _testHeroController;
    [SerializeField] private SpriteRenderer _divineArua;
    [SerializeField] private GameObject _divineAruaBorder;

    private Collider2D _collider;
    private float _attack;
    private float _attackCooldown;
    private float _effectRange;
    private float _effectTime;

    private bool _actTest;

    private const float MIN_DAMAGE_TEXT_POSITION_X = -1f;
    private const float MAX_DAMAGE_TEXT_POSITION_X = 1f;
    private const float DAMAGE_TEXT_POSITION_Y = 1f;
    private const float TWO_MULTIPLES_VALUE = 2f;
    private const float ZERO_EFFECT_RANGE = 0f;
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;

    private readonly Vector3 ROTATE_DIVINE_AURA = new Vector3(0f, 0f, 30f);
    private readonly Color INACTIVE_DIVINE_AURA_BORDER_SPRITE_COLOR = new Color(253f / 255f, 209f / 255f, 44f / 255f, 105f / 255f);
    private readonly Color ACTIVE_DIVINE_AURA_BORDER_SPRITE_COLOR = new Color(253f / 255f, 209f / 255f, 44f / 255f);

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    private void OnEnable()
    {
        _divineArua.transform.Rotate(Vector3.zero);
    }

    private void FixedUpdate()
    {
        if (_actTest)
            _divineArua.transform.Rotate(ROTATE_DIVINE_AURA * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_MONSTER))
        {
            var randomPos = new Vector3(UnityEngine.Random.Range(MIN_DAMAGE_TEXT_POSITION_X, MAX_DAMAGE_TEXT_POSITION_X), DAMAGE_TEXT_POSITION_Y, 0f);
            var initDamageTextPos = collision.transform.position + randomPos;
            var damageTextGO = Manager.Instance.Object.GetDamageText();
            var damageText = Utils.GetOrAddComponent<DamageText>(damageTextGO);
            var attack = _attack;
            var isCritical = _testHeroController.IsCritical();
            if (isCritical)
                attack = _attack * TWO_MULTIPLES_VALUE;
            damageText.FloatDamageText(initDamageTextPos, attack, isCritical);
            Utils.SetActive(damageTextGO, true);

            var testMonster = Utils.GetOrAddComponent<TestMonster>(collision.gameObject);
            testMonster.OnDamaged(attack);
        }
    }

    private void LateUpdate()
    {
        if (_actTest)
            transform.position = _testHeroController.transform.position;
    }

    public void Init(Vector3 initPPos, float attack, float attackCooldown, float effectRange, float effectTime)
    {
        _actTest = true;

        transform.position = initPPos;
        _attack = attack;
        _attackCooldown = attackCooldown;
        _effectRange = effectRange;
        _effectTime = effectTime;

        var collider = _collider as CircleCollider2D;
        collider.radius = _effectRange * TWO_MULTIPLES_VALUE;
        _divineArua.transform.localScale = new Vector3(_effectRange, _effectRange, 1f);
        if (false == gameObject.activeSelf)
        {
            _divineAruaBorder.transform.localScale = Vector3.zero;
            _FadeDivineAura().Forget();
        }
    }

    private async UniTaskVoid _FadeDivineAura()
    {
        var time = ZERO_SECOND;
        var effectRange = ZERO_EFFECT_RANGE;
        while (time < ONE_SECOND)
        {
            time += Time.fixedDeltaTime;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;

            effectRange += _effectRange * TWO_MULTIPLES_VALUE * Time.fixedDeltaTime;
            _divineAruaBorder.transform.localScale = new Vector3(effectRange, effectRange, 1f);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedDeltaTime), delayTiming: PlayerLoopTiming.FixedUpdate);
        }
        _divineArua.color = ACTIVE_DIVINE_AURA_BORDER_SPRITE_COLOR;
        _collider.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(_effectTime));

        _divineArua.color = INACTIVE_DIVINE_AURA_BORDER_SPRITE_COLOR;
        _collider.enabled = false;
        time = ONE_SECOND;
        effectRange = _effectRange * TWO_MULTIPLES_VALUE;
        while (time > ZERO_SECOND)
        {
            time -= Time.fixedDeltaTime;
            if (time <= ZERO_SECOND)
                time = ZERO_SECOND;

            effectRange -= _effectRange * TWO_MULTIPLES_VALUE * Time.fixedDeltaTime;
            _divineAruaBorder.transform.localScale = new Vector3(effectRange, effectRange, 1f);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedDeltaTime), delayTiming: PlayerLoopTiming.FixedUpdate);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));
        _FadeDivineAura().Forget();
    }
}
