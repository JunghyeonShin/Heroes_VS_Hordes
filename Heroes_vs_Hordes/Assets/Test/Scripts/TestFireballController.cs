using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFireballController : MonoBehaviour
{
    [SerializeField] private TestHeroController _testHeroController;
    [SerializeField] private GameObject _testFireball;

    private ObjectPool _testFireballPool = new ObjectPool();
    private Queue<GameObject> _usedFireballQueue = new Queue<GameObject>();

    private float _attack;
    private float _attackCooldown;
    private float _speed;
    private float _effectRange;
    private float _effectTime;
    private float _projectileCount;
    private int _weaponLevel;

    private bool _isAttack;
    private int _fireballAttackCount;

    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const float ANGLE_360 = 360f;
    private const float DELAY_FADE_TIME = 2f;
    private const int CREATE_TEST_WEAPON_COUNT = 10;
    private const int ADJUST_WEAPON_LEVEL = 2;
    private const int INIT_WEAPON_LEVEL = 1;
    private const int MAX_WEAPON_LEVEL = 5;
    private const int INIT_FIREBALL_ATTACK_COUNT = 0;
    private const int MAX_FIREBALL_ATTACK_COUNT = 2;

    private void Update()
    {
        if (_isAttack)
            _Attack();
    }

    public void Init()
    {
        _testFireballPool.InitPool(_testFireball, gameObject, CREATE_TEST_WEAPON_COUNT);
    }

    public void SetAbility()
    {
        var weaponAbility = Manager.Instance.Data.WeaponAbilityDic[Define.WEAPON_FIREBALL];
        var weaponLevelAbilityList = Manager.Instance.Data.WeaponLevelAbilityDic[Define.WEAPON_FIREBALL];

        var weaponAttack = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAttack += weaponLevelAbilityList[ii].Attack;
        }
        _attack = (weaponAbility.Attack + _testHeroController.Attack) * (DEFAULT_ABILITY_VALUE + weaponAttack);
        _attackCooldown = weaponAbility.AttackCooldown;

        var weaponSpeed = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAttack += weaponLevelAbilityList[ii].Speed;
        }
        _speed = weaponAbility.Speed + weaponSpeed;

        var weaponEffectRange = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponEffectRange += weaponLevelAbilityList[ii].EffectRange;
        }
        _effectRange = weaponAbility.EffectRange * (DEFAULT_ABILITY_VALUE + weaponEffectRange);

        var weaponEffectTime = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponEffectTime += weaponLevelAbilityList[ii].EffectTime;
        }
        _effectTime = weaponAbility.EffectTime + weaponEffectTime;

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

    public void StartFireballAttack()
    {
        _fireballAttackCount = INIT_FIREBALL_ATTACK_COUNT;
        _isAttack = true;

        var testSceneUI = Manager.Instance.UI.CurrentSceneUI as UI_TestScene;
        testSceneUI.SetWeaponName(Define.WEAPON_FIREBALL);
        testSceneUI.SetAbilityText(_attack, _attackCooldown, _speed, _effectRange, _effectTime, _projectileCount, 0f);
    }

    private void _Attack()
    {
        _isAttack = false;

        for (int ii = 0; ii < _projectileCount; ++ii)
        {
            var testFireballGO = _GetFireball();
            _usedFireballQueue.Enqueue(testFireballGO);
            var testFireball = Utils.GetOrAddComponent<TestFireball>(testFireballGO);
            testFireball.Init(_testHeroController.transform.position, _GetTargetPos(ii), _speed, _attack, _effectRange, _effectTime);
            Utils.SetActive(testFireballGO, true);
        }
        _ReturnFireballAsync().Forget();
    }

    private GameObject _GetFireball()
    {
        return _testFireballPool.GetObject();
    }

    private void _ReturnFireball()
    {
        while (_usedFireballQueue.Count > 0)
        {
            var fireball = _usedFireballQueue.Dequeue();
            if (fireball.activeSelf)
                _testFireballPool.ReturnObject(fireball);
        }
    }

    private Vector3 _GetTargetPos(int index)
    {
        var targetPos = Vector3.up;
        if (0 == index)
            return targetPos * _effectRange;

        var angle = (ANGLE_360 / _projectileCount) * index;
        targetPos = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
        return targetPos.normalized * _effectRange;
    }

    private async UniTaskVoid _ReturnFireballAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_effectTime + DELAY_FADE_TIME));

        _ReturnFireball();
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));

        ++_fireballAttackCount;
        if (_fireballAttackCount < MAX_FIREBALL_ATTACK_COUNT)
            _isAttack = true;
    }
}
