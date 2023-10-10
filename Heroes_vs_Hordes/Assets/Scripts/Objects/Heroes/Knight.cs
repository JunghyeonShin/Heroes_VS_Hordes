using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Hero
{
    private float _effectRange;

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
    }

    protected override void _DetectMonster()
    {

    }

    protected override void _AttackMonster()
    {

    }
}
