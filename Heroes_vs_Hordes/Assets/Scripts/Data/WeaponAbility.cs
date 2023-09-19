using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAbility
{
    private float _attack;
    private float _attackCooldown;
    private float _speed;
    private float _effectRange;
    private float _effectTime;
    private float _projectileCount;
    private float _penetrateCount;

    public float Attack { get { return _attack; } }
    public float AttackCooldown { get { return _attackCooldown; } }
    public float Speed { get { return _speed; } }
    public float EffectRange { get { return _effectRange; } }
    public float EffectTime { get { return _effectTime; } }
    public float ProjectileCount { get { return _projectileCount; } }
    public float PenetrateCount { get { return _penetrateCount; } }

    private const int INDEX_WEAPON_ABILITY_ATTACK = 1;
    private const int INDEX_WEAPON_ABILITY_ATTACK_COOLDOWN = 2;
    private const int INDEX_WEAPON_ABILITY_SPEED = 3;
    private const int INDEX_WEAPON_ABILITY_EFFFECT_RANGE = 4;
    private const int INDEX_WEAPON_ABILITY_EFFECT_TIME = 5;
    private const int INDEX_WEAPON_ABILITY_PROJECTILE_COUNT = 6;
    private const int INDEX_WEAPON_ABILITY_PENETRATE_COUNT = 7;

    public WeaponAbility(string[] splitData)
    {
        float.TryParse(splitData[INDEX_WEAPON_ABILITY_ATTACK].TrimEnd(), out _attack);
        float.TryParse(splitData[INDEX_WEAPON_ABILITY_ATTACK_COOLDOWN].TrimEnd(), out _attackCooldown);
        float.TryParse(splitData[INDEX_WEAPON_ABILITY_SPEED].TrimEnd(), out _speed);
        float.TryParse(splitData[INDEX_WEAPON_ABILITY_EFFFECT_RANGE].TrimEnd(), out _effectRange);
        float.TryParse(splitData[INDEX_WEAPON_ABILITY_EFFECT_TIME].TrimEnd(), out _effectTime);
        float.TryParse(splitData[INDEX_WEAPON_ABILITY_PROJECTILE_COUNT].TrimEnd(), out _projectileCount);
        float.TryParse(splitData[INDEX_WEAPON_ABILITY_PENETRATE_COUNT].TrimEnd(), out _penetrateCount);
    }
}
