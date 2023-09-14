using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbility
{
    private float _health;
    private float _defense;
    private float _attack;
    private float _attackCooldown;
    private float _critical;
    private float _moveSpeed;
    private float _projectileSpeed;

    public float Health { get { return _health; } }
    public float Defense { get { return _defense; } }
    public float Attack { get { return _attack; } }
    public float AttackCooldown { get { return _attackCooldown; } }
    public float Critical { get { return _critical; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float ProjectileSpeed { get { return _projectileSpeed; } }

    private const int INDEX_HERO_ABILITY_HEALTH = 1;
    private const int INDEX_HERO_ABILITY_DEFENCE = 2;
    private const int INDEX_HERO_ABILITY_ATTACK = 3;
    private const int INDEX_HERO_ABILITY_ATTACK_COOLDOWN = 4;
    private const int INDEX_HERO_ABILITY_CRITICAL = 5;
    private const int INDEX_HERO_ABILITY_MOVE_SPEED = 6;
    private const int INDEX_HERO_ABILITY_PROJECTILE_SPEED = 7;

    public HeroAbility(string[] splitData)
    {
        float.TryParse(splitData[INDEX_HERO_ABILITY_HEALTH].TrimEnd(), out _health);
        float.TryParse(splitData[INDEX_HERO_ABILITY_DEFENCE].TrimEnd(), out _defense);
        float.TryParse(splitData[INDEX_HERO_ABILITY_ATTACK].TrimEnd(), out _attack);
        float.TryParse(splitData[INDEX_HERO_ABILITY_ATTACK_COOLDOWN].TrimEnd(), out _attackCooldown);
        float.TryParse(splitData[INDEX_HERO_ABILITY_CRITICAL].TrimEnd(), out _critical);
        float.TryParse(splitData[INDEX_HERO_ABILITY_MOVE_SPEED].TrimEnd(), out _moveSpeed);
        float.TryParse(splitData[INDEX_HERO_ABILITY_PROJECTILE_SPEED].TrimEnd(), out _projectileSpeed);
    }
}
