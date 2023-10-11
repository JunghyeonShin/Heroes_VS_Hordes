using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbility
{
    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const float MIN_CRITICAL_VALUE = 0f;
    private const float MAX_CRITICAL_VALUE = 1f;
    private const int ADJUST_BOOK_LEVEL = 1;

    public static float GetHeroHealth(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);

        var heartTalentLevel = Manager.Instance.SaveData.OwnedTalents[Define.INDEX_TALENT_HEART];
        var heartTalentAbility = Manager.Instance.Data.TalentInfoDataList[Define.INDEX_TALENT_HEART][heartTalentLevel].Ability;

        return heroCommonAbility.Health + heroIndividualAbility.Health + heartTalentAbility;
    }

    public static float GetHeroDeffence(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        return heroCommonAbility.Defense + heroIndividualAbility.Defense;
    }

    public static float GetHeroAttack(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);

        var swordTalentLevel = Manager.Instance.SaveData.OwnedTalents[Define.INDEX_TALENT_SWORD];
        var swordTalentAbility = Manager.Instance.Data.TalentInfoDataList[Define.INDEX_TALENT_SWORD][swordTalentLevel].Ability;

        return heroCommonAbility.Attack * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.Attack + swordTalentAbility);
    }

    public static float GetHeroAttackCooldown(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        var heroAttackCooldownValue = heroCommonAbility.AttackCooldown + heroIndividualAbility.AttackCooldown;

        var attackCooldownBookLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_COOLDOWN);
        if (attackCooldownBookLevel - ADJUST_BOOK_LEVEL >= 0)
        {
            var bookAbility = Manager.Instance.Data.BookAbilityDataDic[Define.BOOK_COOLDOWN];
            return heroAttackCooldownValue * (DEFAULT_ABILITY_VALUE + bookAbility[Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_COOLDOWN) - ADJUST_BOOK_LEVEL]);
        }
        else
            return heroAttackCooldownValue;
    }

    public static float GetHeroCritical(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        return heroCommonAbility.Critical + heroIndividualAbility.Critical;
    }

    public static float GetHeroMoveSpeed(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        var heroMoveSpeedValue = heroCommonAbility.MoveSpeed * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.MoveSpeed);

        var boostTalentLevel = Manager.Instance.SaveData.OwnedTalents[Define.INDEX_TALENT_BOOST];
        var boostTalentAbility = Manager.Instance.Data.TalentInfoDataList[Define.INDEX_TALENT_BOOST][boostTalentLevel].Ability;

        var heroMoveSpeedBookLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_HERO_MOVE_SPEED);
        if (heroMoveSpeedBookLevel - ADJUST_BOOK_LEVEL >= 0)
        {
            var bookAbility = Manager.Instance.Data.BookAbilityDataDic[Define.BOOK_HERO_MOVE_SPEED];
            return heroMoveSpeedValue * (DEFAULT_ABILITY_VALUE + bookAbility[Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_HERO_MOVE_SPEED) - ADJUST_BOOK_LEVEL] + boostTalentAbility);
        }
        else
            return heroMoveSpeedValue * (DEFAULT_ABILITY_VALUE + boostTalentAbility);
    }

    public static float GetHeroProjectileSpeed(string heroName)
    {
        (var heroCommonAbility, var heroIndividualAbility) = _GetHeroAbilityData(heroName);
        var heroProjectileValue = heroCommonAbility.ProjectileSpeed * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.ProjectileSpeed);

        var projectileSpeedBookLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_PROJECTILE_SPEED);
        if (projectileSpeedBookLevel - ADJUST_BOOK_LEVEL >= 0)
        {
            var bookAbility = Manager.Instance.Data.BookAbilityDataDic[Define.BOOK_PROJECTILE_SPEED];
            return heroProjectileValue * (DEFAULT_ABILITY_VALUE + bookAbility[Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_PROJECTILE_SPEED) - ADJUST_BOOK_LEVEL]);
        }
        else
            return heroProjectileValue;
    }

    public static float GetHeroRecovery()
    {
        var heroRecoveryBoolLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_HERO_RECOVERY);
        if (heroRecoveryBoolLevel - ADJUST_BOOK_LEVEL >= 0)
        {
            var bookAbility = Manager.Instance.Data.BookAbilityDataDic[Define.BOOK_HERO_RECOVERY];
            return bookAbility[Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_HERO_RECOVERY) - ADJUST_BOOK_LEVEL];
        }
        else
            return 0f;
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
