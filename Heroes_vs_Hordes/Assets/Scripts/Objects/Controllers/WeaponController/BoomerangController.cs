using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : WeaponController
{
    private float _rotateAngle;
    private float _finishAttackCount;

    private const float ROTATE_ANGLE = 45f;
    private const float ANGLE_360 = 360f;
    private const float FINISH_ATTACK_COUNT = 0f;
    private const int INIT_BOOMERANG_COUNT = 10;

    protected override void _Init()
    {
        _weaponName = Define.WEAPON_BOOMERANG;
        _initWeaponCount = INIT_BOOMERANG_COUNT;

        base._Init();
    }

    protected override bool _Attack()
    {
        if (false == base._Attack())
            return false;

        _finishAttackCount = _projectileCount;
        for (int ii = 0; ii < _projectileCount; ++ii)
        {
            var boomerangGO = _GetWeapon();
            _usedWeaponQueue.Enqueue(boomerangGO);
            var boomerang = Utils.GetOrAddComponent<Boomerang>(boomerangGO);
            boomerang.Init(Manager.Instance.Ingame.UsedHero.transform.position, _GetTargetPos(ii));
            boomerang.FinishAttackHandler -= _FinishAttack;
            boomerang.FinishAttackHandler += _FinishAttack;
            Utils.SetActive(boomerangGO, true);
        }
        _rotateAngle -= ROTATE_ANGLE;
        if (_rotateAngle <= ANGLE_360)
            _rotateAngle += ANGLE_360;
        _Reattack().Forget();
        return true;
    }

    private Vector3 _GetTargetPos(int index)
    {
        var angle = (ANGLE_360 / _projectileCount) * index + _rotateAngle;
        var targetPos = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
        return targetPos.normalized * _effectRange;
    }

    private void _FinishAttack()
    {
        --_finishAttackCount;
    }

    private async UniTaskVoid _Reattack()
    {
        while (_finishAttackCount > FINISH_ATTACK_COUNT)
            await UniTask.Yield();

        _ReturnWeapon();
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));

        _Attack();
    }
}
