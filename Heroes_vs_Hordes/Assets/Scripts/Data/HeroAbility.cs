using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbility
{
    public float Health { get; private set; }
    public float Defense { get; private set; }
    public float Attack { get; private set; }
    public float AttackCooldown { get; private set; }
    public float Critical { get; private set; }
    public float MoveSpeed { get; private set; }
    public float ProjectileSpeed { get; private set; }

    public HeroAbility(float health, float defense, float attack, float attackCooldown, float critical, float moveSpeed, float projectileSpeed)
    {
        Health = health;
        Defense = defense;
        Attack = attack;
        AttackCooldown = attackCooldown;
        Critical = critical;
        MoveSpeed = moveSpeed;
        ProjectileSpeed = projectileSpeed;
    }
}
