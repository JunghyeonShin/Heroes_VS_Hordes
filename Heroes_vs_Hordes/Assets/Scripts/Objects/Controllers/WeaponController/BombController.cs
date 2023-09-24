using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : WeaponController
{
    private float _finishAttackCount;
    private List<int> _targetMonsterIndexList = new List<int>();

    private const float DEFAULT_DETECT_BOX_ANGLE = 0f;
    private const float FINISH_ATTACK_COUNT = 0f;
    private const int INIT_BOMB_COUNT = 10;

    private readonly Vector2 OVERLAP_SIZE = new Vector2(22.5f, 40f);

    protected override void _Init()
    {
        _weaponName = Define.WEAPON_BOMB;
        _initWeaponCount = INIT_BOMB_COUNT;

        base._Init();
    }

    protected override bool _Attack()
    {
        if (base._Attack())
            return false;

        _finishAttackCount = _projectileCount;
        _targetMonsterIndexList.Clear();
        var layerMask = 1 << LayerMask.NameToLayer(Define.LAYER_MONSTER);
        var monsters = Physics2D.OverlapBoxAll(Manager.Instance.Ingame.UsedHero.transform.position, OVERLAP_SIZE, DEFAULT_DETECT_BOX_ANGLE, layerMask);
        if (monsters.Length > 0)
        {
            for (int ii = 0; ii < _projectileCount; ++ii)
                _targetMonsterIndexList.Add(_GetRandomTargetMonster(monsters.Length));

            for (int ii = 0; ii < _projectileCount; ++ii)
            {
                var bombGO = _GetWeapon();
                _usedWeaponQueue.Enqueue(bombGO);
                var bomb = Utils.GetOrAddComponent<Bomb>(bombGO);
                bomb.Init(Manager.Instance.Ingame.UsedHero.transform.position, monsters[_targetMonsterIndexList[ii]].transform.position);
                bomb.FinishAttackHandler -= _FinishAttack;
                bomb.FinishAttackHandler += _FinishAttack;
                Utils.SetActive(bombGO, true);
            }
            _Reattack().Forget();
        }
        return true;
    }

    private int _GetRandomTargetMonster(int monsterIndices)
    {
        return UnityEngine.Random.Range(0, monsterIndices);
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