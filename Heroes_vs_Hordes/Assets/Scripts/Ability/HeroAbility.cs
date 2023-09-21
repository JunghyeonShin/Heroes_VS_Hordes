using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbility
{
    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const float MIN_CRITICAL_VALUE = 0f;
    private const float MAX_CRITICAL_VALUE = 1f;

    public static float GetHeroHealth(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        return heroCommonAbility.Health + heroIndividualAbility.Health;
    }

    public static float GetHeroDeffence(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        return heroCommonAbility.Defense + heroIndividualAbility.Defense;
    }

    public static float GetHeroAttack(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        return heroCommonAbility.Attack * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.Attack);
    }

    public static float GetHeroAttackCooldown(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        return heroCommonAbility.AttackCooldown + heroIndividualAbility.AttackCooldown;
    }

    public static float GetHeroCritical(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        return heroCommonAbility.Critical + heroIndividualAbility.Critical;
    }

    public static float GetHeroMoveSpeed(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        return heroCommonAbility.MoveSpeed * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.MoveSpeed);
    }

    public static float GetHeroProjectileSpeed(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        return heroCommonAbility.ProjectileSpeed * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.ProjectileSpeed);
    }

    public static bool IsCritical(float critical)
    {
        var randomValue = Random.Range(MIN_CRITICAL_VALUE, MAX_CRITICAL_VALUE);
        if (critical >= randomValue)
            return true;
        return false;
    }

    private static (HeroAbilityData, HeroAbilityData) _GetHeroAbilityData(string heroName)
    {
        return (Manager.Instance.Data.HeroCommonAbilityData, Manager.Instance.Data.HeroIndividualAbilityDataDic[heroName]);
    }
}
