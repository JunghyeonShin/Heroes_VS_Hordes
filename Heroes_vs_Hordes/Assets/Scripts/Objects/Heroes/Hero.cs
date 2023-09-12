using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : MonoBehaviour
{
    protected float _health;
    protected float _defense;
    protected float _attack;
    protected float _attackCooldown;
    protected float _critical;

    public float MoveSpeed { get; private set; }
    public float ProjectileSpeed { get; private set; }

    private const float DEFAULT_ABILITY_VALUE = 1f;

    public void SetHeroAbilities()
    {
        var heroCommonAbility = Manager.Instance.Data.HeroCommonAbility;
        var heroIndividualAbility = Manager.Instance.Data.HeroIndividualAbility[Define.RESOURCE_HERO_ARCANE_MAGE];

        _health = heroCommonAbility.Health + heroIndividualAbility.Health;
        _defense = heroCommonAbility.Defense + heroIndividualAbility.Defense;
        _attack = heroCommonAbility.Attack * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.Attack);
        _attackCooldown = heroCommonAbility.AttackCooldown + heroIndividualAbility.AttackCooldown;
        _critical = heroCommonAbility.Critical + heroIndividualAbility.Critical;
        MoveSpeed = heroCommonAbility.MoveSpeed * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.MoveSpeed);
        ProjectileSpeed = heroCommonAbility.ProjectileSpeed * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.ProjectileSpeed);
    }
}
