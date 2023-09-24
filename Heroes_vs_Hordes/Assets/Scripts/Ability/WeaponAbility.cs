using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAbility
{
    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const int ADJUST_WEAPON_LEVEL = 2;
    private const int ADJUST_BOOK_LEVEL = 1;

    public static float GetWeaponAttack(string heroName, string weaponName, int weaponLevel)
    {
        (var weaponAbility, var weaponLevelAbilityList) = _GetWeaponAbilityData(weaponName);
        var weaponAdditionalAttack = 0f;
        if (weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAdditionalAttack += weaponLevelAbilityList[ii].Attack;
        }
        return (weaponAbility.Attack + HeroAbility.GetHeroAttack(heroName)) * (DEFAULT_ABILITY_VALUE + weaponAdditionalAttack);
    }

    public static float GetWeaponAttackCooldown(string weaponName, int weaponLevel)
    {
        (var weaponAbility, var weaponLevelAbilityList) = _GetWeaponAbilityData(weaponName);
        var weaponAdditionalAttackCooldown = 0f;
        if (weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAdditionalAttackCooldown += weaponLevelAbilityList[ii].AttackCooldown;
        }
        var weaponAttackCooldownValue = weaponAbility.AttackCooldown + weaponAdditionalAttackCooldown;

        var attackCooldownBookLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_COOLDOWN);
        if (attackCooldownBookLevel - ADJUST_BOOK_LEVEL >= 0)
        {
            var bookAbility = Manager.Instance.Data.BookAbilityDataDic[Define.BOOK_COOLDOWN];
            return weaponAttackCooldownValue * (DEFAULT_ABILITY_VALUE + bookAbility[Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_COOLDOWN) - ADJUST_BOOK_LEVEL]);
        }
        else
            return weaponAttackCooldownValue;
    }

    public static float GetWeaponSpeed(string weaponName, int weaponLevel)
    {
        (var weaponAbility, var weaponLevelAbilityList) = _GetWeaponAbilityData(weaponName);
        var weaponAdditionalSpeed = 0f;
        if (weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAdditionalSpeed += weaponLevelAbilityList[ii].Speed;
        }
        var weaponSpeedValue = weaponAbility.Speed * (DEFAULT_ABILITY_VALUE + weaponAdditionalSpeed);

        var speedBookLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_PROJECTILE_SPEED);
        if (speedBookLevel - ADJUST_BOOK_LEVEL >= 0)
        {
            var bookAbility = Manager.Instance.Data.BookAbilityDataDic[Define.BOOK_PROJECTILE_SPEED];
            return weaponSpeedValue * (DEFAULT_ABILITY_VALUE + bookAbility[Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_PROJECTILE_SPEED) - ADJUST_BOOK_LEVEL]);
        }
        else
            return weaponSpeedValue;
    }

    public static float GetWeaponEffectRange(string weaponName, int weaponLevel)
    {
        (var weaponAbility, var weaponLevelAbilityList) = _GetWeaponAbilityData(weaponName);
        var weaponAdditionalEffectRange = 0f;
        if (weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAdditionalEffectRange += weaponLevelAbilityList[ii].EffectRange;
        }
        var weaponEffectRangeValue = weaponAbility.EffectRange * (DEFAULT_ABILITY_VALUE + weaponAdditionalEffectRange);

        var effectRangeBookLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_RANGE);
        if (effectRangeBookLevel - ADJUST_BOOK_LEVEL >= 0)
        {
            var bookAbility = Manager.Instance.Data.BookAbilityDataDic[Define.BOOK_RANGE];
            return weaponEffectRangeValue * (DEFAULT_ABILITY_VALUE + bookAbility[Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_RANGE) - ADJUST_BOOK_LEVEL]);
        }
        else
            return weaponEffectRangeValue;
    }

    public static float GetWeaponEffectTime(string weaponName, int weaponLevel)
    {
        (var weaponAbility, var weaponLevelAbilityList) = _GetWeaponAbilityData(weaponName);
        var weaponEffectTime = 0f;
        if (weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponEffectTime += weaponLevelAbilityList[ii].EffectTime;
        }
        return weaponAbility.EffectTime + weaponEffectTime;
    }

    public static float GetWeaponProjectileCount(string weaponName, int weaponLevel)
    {
        (var weaponAbility, var weaponLevelAbilityList) = _GetWeaponAbilityData(weaponName);
        var weaponAdditionalProjectileCount = 0f;
        if (weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAdditionalProjectileCount += weaponLevelAbilityList[ii].ProjectileCount;
        }
        var weaponProjectileCountValue = weaponAbility.ProjectileCount + weaponAdditionalProjectileCount;

        var projectileCountBookLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_PROJECTILE_COPY);
        if (projectileCountBookLevel - ADJUST_BOOK_LEVEL >= 0)
        {
            var bookAbility = Manager.Instance.Data.BookAbilityDataDic[Define.BOOK_PROJECTILE_COPY];
            return weaponProjectileCountValue + bookAbility[Manager.Instance.Ingame.GetOwnedAbilityLevel(Define.BOOK_PROJECTILE_COPY) - ADJUST_BOOK_LEVEL];
        }
        else
            return weaponProjectileCountValue;
    }

    public static float GetWeaponPenetraitCount(string weaponName, int weaponLevel)
    {
        (var weaponAbility, var weaponLevelAbilityList) = _GetWeaponAbilityData(weaponName);
        var weaponAdditionalPenetraitCount = 0f;
        if (weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAdditionalPenetraitCount += weaponLevelAbilityList[ii].PenetrateCount;
        }
        return weaponAbility.PenetrateCount + weaponAdditionalPenetraitCount;
    }

    private static (WeaponAbilityData, List<WeaponAbilityData>) _GetWeaponAbilityData(string weaponName)
    {
        var weaponAbility = Manager.Instance.Data.WeaponAbilityDataDic[weaponName];
        var weaponLevelAbilityList = Manager.Instance.Data.WeaponLevelAbilityDataDic[weaponName];

        return (weaponAbility, weaponLevelAbilityList);
    }
}
