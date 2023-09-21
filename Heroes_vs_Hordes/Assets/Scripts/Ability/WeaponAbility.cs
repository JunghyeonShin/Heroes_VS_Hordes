using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAbility
{
    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const int ADJUST_WEAPON_LEVEL = 2;

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
        return weaponAbility.AttackCooldown + weaponAdditionalAttackCooldown;
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
        return weaponAbility.Speed * (DEFAULT_ABILITY_VALUE + weaponAdditionalSpeed);
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
        return weaponAbility.EffectRange * (DEFAULT_ABILITY_VALUE + weaponAdditionalEffectRange);
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
        return weaponAbility.ProjectileCount + weaponAdditionalProjectileCount;
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
