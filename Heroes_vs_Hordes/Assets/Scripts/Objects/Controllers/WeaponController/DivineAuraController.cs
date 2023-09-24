using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineAuraController : WeaponController
{
    private const int INIT_DIVINE_AURA_COUNT = 2;
    protected override void _Init()
    {
        _weaponName = Define.WEAPON_BOOMERANG;
        _initWeaponCount = INIT_DIVINE_AURA_COUNT;

        base._Init();
    }

    protected override bool _Attack()
    {
        if (false == base._Attack())
            return false;

        var divineAuraGO = _GetWeapon();
        _usedWeaponQueue.Enqueue(divineAuraGO);
        var divineAura = Utils.GetOrAddComponent<DivineAura>(divineAuraGO);
        divineAura.Init(Manager.Instance.Ingame.UsedHero.transform.position);
        divineAura.FinishAttackHandler -= _FinishAttack;
        divineAura.FinishAttackHandler += _FinishAttack;
        Utils.SetActive(divineAuraGO, true);
        return true;
    }

    private void _FinishAttack()
    {
        _Reattack().Forget();
    }

    private async UniTaskVoid _Reattack()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));

        _ReturnWeapon();
        _Attack();
    }
}
