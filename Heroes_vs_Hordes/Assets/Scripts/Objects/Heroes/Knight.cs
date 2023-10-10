using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Hero
{
    [SerializeField] private KnightSlash _knightSlash;

    private float _effectRange;

    private const float INIT_KNIGHT_SLASH_RANGE = 1.5f;

    private readonly Vector2 OVERLAP_SIZE = new Vector2(10f, 10f);

    protected override void Awake()
    {
        base.Awake();
        _heroName = Define.RESOURCE_HERO_KNIGHT;
        _heroWeaponName = Define.WEAPON_KNIGHT_SWORD;
    }

    public override void SetAbilities()
    {
        base.SetAbilities();
        var weaponLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(_heroWeaponName);
        _effectRange = WeaponAbility.GetWeaponEffectRange(_heroWeaponName, weaponLevel);
        _knightSlash.transform.localScale = new Vector3(INIT_KNIGHT_SLASH_RANGE * _effectRange, INIT_KNIGHT_SLASH_RANGE * _effectRange, 1f);
    }

    protected override void _DetectMonster()
    {
        if (false == _detectMonster)
            _DetectMonsterAsync().Forget();
    }

    protected override void _AttackMonster()
    {
        _attackMonster = true;
        _knightSlash.SetAbilities();
        _animator.SetTrigger(Define.ANIMATOR_TRIGGER_ATTACK);
    }

    private async UniTaskVoid _DetectMonsterAsync()
    {
        _detectMonster = true;
        if (_attackMonster)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));
            _attackMonster = false;
        }

        var monsters = Physics2D.OverlapBoxAll(transform.position, OVERLAP_SIZE, DEFAULT_DETECT_BOX_ANGLE, Define.LAYER_MASK_MONSTER);
        if (monsters.Length > 0)
            _AttackMonster();
        _detectMonster = false;
    }
}
