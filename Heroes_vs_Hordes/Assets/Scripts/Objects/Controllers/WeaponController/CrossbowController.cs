using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowController : WeaponController
{
    private const float RANDON_POS_VALUE = 1f;
    private const int INIT_CROSSBOW_COUNT = 10;

    protected override void _Init()
    {
        _weaponName = Define.WEAPON_CROSSBOW;
        _initWeaponCount = INIT_CROSSBOW_COUNT;
        base._Init();
    }

    protected override void _Attack()
    {
        for (int ii = 0; ii < _projectileCount; ++ii)
        {
            var crossbowGO = _GetWeapon();
            _usedWeaponQueue.Enqueue(crossbowGO);
            var crossbow = Utils.GetOrAddComponent<Crossbow>(crossbowGO);
            crossbow.Init(Manager.Instance.Ingame.UsedHero.transform.position, _GetRandomPos());
            Utils.SetActive(crossbowGO, true);
        }
        _ReAttack().Forget();
    }

    private Vector3 _GetRandomPos()
    {
        var posX = UnityEngine.Random.Range(-RANDON_POS_VALUE, RANDON_POS_VALUE);
        var posY = UnityEngine.Random.Range(-RANDON_POS_VALUE, RANDON_POS_VALUE);
        return new Vector3(posX, posY).normalized;
    }

    private async UniTaskVoid _ReAttack()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_effectTime));

        _ReturnWeapon();
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));

        _Attack();
    }
}
