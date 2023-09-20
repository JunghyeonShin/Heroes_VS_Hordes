using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestBoomerangController : MonoBehaviour
{
    [SerializeField] private TestHeroController _testHeroController;
    [SerializeField] private GameObject _testBoomerang;

    private ObjectPool _testBoomerangPool = new ObjectPool();
    private Queue<GameObject> _usedBoomerangQueue = new Queue<GameObject>();

    private float _attack;
    private float _attackCooldown;
    private float _speed;
    private float _effectRange;
    private float _effectTime;
    private float _projectileCount;
    private int _weaponLevel;

    private bool _isAttack;
    private int _boomerangAttackCount;

    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const int CREATE_TEST_WEAPON_COUNT = 10;
    private const int ADJUST_WEAPON_LEVEL = 2;
    private const int INIT_WEAPON_LEVEL = 1;
    private const int MAX_WEAPON_LEVEL = 5;
    private const int INIT_BOOMERANG_ATTACK_COUNT = 0;
    private const int MAX_BOOMERANG_ATTACK_COUNT = 2;

    private void Update()
    {
        if (_isAttack)
            _Attack();
    }

    public void Init()
    {
        _testBoomerangPool.InitPool(_testBoomerang, gameObject, CREATE_TEST_WEAPON_COUNT);
    }

    public void SetAbility()
    {
        var weaponAbility = Manager.Instance.Data.WeaponAbilityDic[Define.WEAPON_BOOMERANG];
        var weaponLevelAbilityList = Manager.Instance.Data.WeaponLevelAbilityDic[Define.WEAPON_BOOMERANG];

        var weaponAttack = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAttack += weaponLevelAbilityList[ii].Attack;
        }
        _attack = (weaponAbility.Attack + _testHeroController.Attack) * (DEFAULT_ABILITY_VALUE + weaponAttack);

        var weaponAttackCooldown = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAttackCooldown += weaponLevelAbilityList[ii].AttackCooldown;
        }
        _attackCooldown = weaponAbility.AttackCooldown + weaponAttackCooldown;

        var weaponSpeed = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponSpeed += weaponLevelAbilityList[ii].Speed;
        }
        _speed = weaponAbility.Speed * (DEFAULT_ABILITY_VALUE + weaponSpeed);

        var weaponEffectRange = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponEffectRange += weaponLevelAbilityList[ii].EffectRange;
        }
        _effectRange = weaponAbility.EffectRange * (DEFAULT_ABILITY_VALUE + weaponEffectRange);
        _effectTime = weaponAbility.EffectTime;

        var weaponProjectileCount = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponProjectileCount += weaponLevelAbilityList[ii].ProjectileCount;
        }
        _projectileCount = weaponAbility.ProjectileCount + weaponProjectileCount;
    }

    public void SetLevelUp()
    {
        ++_weaponLevel;
        if (_weaponLevel > MAX_WEAPON_LEVEL)
            _weaponLevel = MAX_WEAPON_LEVEL;
        SetAbility();
    }

    public void SetLevelDown()
    {
        --_weaponLevel;
        if (_weaponLevel < INIT_WEAPON_LEVEL)
            _weaponLevel = INIT_WEAPON_LEVEL;
        SetAbility();
    }

    public void StartBoomerangAttack()
    {
        _boomerangAttackCount = INIT_BOOMERANG_ATTACK_COUNT;
        _isAttack = true;

        var testSceneUI = Manager.Instance.UI.CurrentSceneUI as UI_TestScene;
        testSceneUI.SetWeaponName(Define.WEAPON_BOOMERANG);
        testSceneUI.SetAbilityText(_attack, _attackCooldown, _speed, _effectRange, _effectTime, _projectileCount, 0f);
    }

    private void _Attack()
    {
        _isAttack = false;

        for (int ii = 0; ii < _projectileCount; ++ii)
        {
            var testBoomerangGO = _GetBoomerang();
            _usedBoomerangQueue.Enqueue(testBoomerangGO);
            var testBoomerang = Utils.GetOrAddComponent<TestBoomerang>(testBoomerangGO);
            testBoomerang.Init(_testHeroController.transform.position, Vector3.zero, _speed, _attack, _effectRange, _effectTime);
            Utils.SetActive(testBoomerangGO, true);
        }
        _ReturnBoomerangAsync().Forget();
    }

    private GameObject _GetBoomerang()
    {
        return _testBoomerangPool.GetObject();
    }

    private void _ReturnBoomerang()
    {
        while (_usedBoomerangQueue.Count > 0)
        {
            var boomerang = _usedBoomerangQueue.Dequeue();
            if (boomerang.activeSelf)
                _testBoomerangPool.ReturnObject(boomerang);
        }
    }

    private async UniTaskVoid _ReturnBoomerangAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_effectTime));

        _ReturnBoomerang();
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));

        ++_boomerangAttackCount;
        if (_boomerangAttackCount < MAX_BOOMERANG_ATTACK_COUNT)
            _isAttack = true;
    }
}
