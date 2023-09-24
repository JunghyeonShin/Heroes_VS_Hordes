using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineAura : Weapon
{
    public event Action FinishAttackHandler;

    [SerializeField] private SpriteRenderer _divineArua;
    [SerializeField] private GameObject _divineAruaBorder;

    private const float ZERO_EFFECT_RANGE = 0f;

    private readonly Vector3 ROTATE_DIVINE_AURA = new Vector3(0f, 0f, 30f);
    private readonly Color INACTIVE_DIVINE_AURA_BORDER_SPRITE_COLOR = new Color(253f / 255f, 209f / 255f, 44f / 255f, 105f / 255f);
    private readonly Color ACTIVE_DIVINE_AURA_BORDER_SPRITE_COLOR = new Color(253f / 255f, 209f / 255f, 44f / 255f);

    protected override void Awake()
    {
        base.Awake();

        _weaponName = Define.WEAPON_DIVINE_AURA;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        _divineArua.transform.Rotate(ROTATE_DIVINE_AURA * Time.fixedDeltaTime);
    }

    protected override void LateUpdate()
    {
        transform.position = Manager.Instance.Ingame.UsedHero.transform.position;
    }

    public override void Init(Vector3 initPos)
    {
        base.Init(initPos);

        _divineArua.transform.Rotate(Vector3.zero);
        var collider = _collider as CircleCollider2D;
        collider.radius = _effectRange * TWO_MULTIPLES_VALUE;
        _divineArua.transform.localScale = new Vector3(_effectRange, _effectRange, 1f);
        if (false == gameObject.activeSelf)
        {
            _divineAruaBorder.transform.localScale = Vector3.zero;
            collider.enabled = false;
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
        FinishAttackHandler?.Invoke();
    }
}
