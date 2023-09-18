using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCrossbowController : MonoBehaviour
{
    [SerializeField] TestHeroController _testHeroController;
    [SerializeField] GameObject _testCrossbow;

    private ObjectPool _testCrossbowPool = new ObjectPool();
    private Queue<GameObject> _usedCrossbowQueue = new Queue<GameObject>();

    private float _attack;
    private float _attackCooldown;
    private float _speed;
    private float _effectTime;
    private float _projectileCount;
    private int _weaponLevel;

    private bool _isAttack;
    private int _crossbowAttackCount;

    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const float RANDON_POS_VALUE = 1f;
    private const int CREATE_TEST_WEAPON_COUNT = 10;
    private const int ADJUST_WEAPON_LEVEL = 2;
    private const int INIT_WEAPON_LEVEL = 1;
    private const int MAX_WEAPON_LEVEL = 5;
    private const int INIT_CROSSBOW_ATTACK_COUNT = 0;
    private const int MAX_CROSSBOW_ATTACK_COUNT = 2;

    private void Update()
    {
        if (_isAttack)
            _Attack();
    }

    public void Init()
    {
        _testCrossbowPool.InitPool(_testCrossbow, gameObject, CREATE_TEST_WEAPON_COUNT);
    }

    public void SetAbility()
    {
        var weaponAbility = Manager.Instance.Data.WeaponAbilityDic[Define.WEAPON_CROSSBOW];
        var weaponLevelAbilityList = Manager.Instance.Data.WeaponLevelAbilityDic[Define.WEAPON_CROSSBOW];
        var weaponAttack = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAttack += weaponLevelAbilityList[ii].Attack;
        }
        _attack = (weaponAbility.Attack + _testHeroController.Attack) * (DEFAULT_ABILITY_VALUE + weaponAttack);
        _attackCooldown = weaponAbility.AttackCooldown;
        _speed = weaponAbility.Speed;

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

    public void StartCrossbowAttack()
    {
        _crossbowAttackCount = INIT_CROSSBOW_ATTACK_COUNT;
        _isAttack = true;

        var testSceneUI = Manager.Instance.UI.CurrentSceneUI as UI_TestScene;
        testSceneUI.SetWeaponName(Define.WEAPON_CROSSBOW);
        testSceneUI.SetAttack(_attack);
        testSceneUI.SetAttackCooldown(_attackCooldown);
        testSceneUI.SetSpeed(_speed);
        testSceneUI.SeEffectTime(_effectTime);
        testSceneUI.SetProjectileCount(_projectileCount);
    }

    private void _Attack()
    {
        _isAttack = false;
        for (int ii = 0; ii < _projectileCount; ++ii)
        {
            var testCrossbowGO = _GetCrossbow();
            _usedCrossbowQueue.Enqueue(testCrossbowGO);
            var testCrossbow = Utils.GetOrAddComponent<TestCrossbow>(testCrossbowGO);
            testCrossbow.Init(_testHeroController.transform.position, _GetRandomPos(), _speed, _attack);
            Utils.SetActive(testCrossbowGO, true);
        }
        _ReturnCrossbowAsync().Forget();
    }

    private GameObject _GetCrossbow()
    {
        return _testCrossbowPool.GetObject();
    }

    private void _ReturnCrossbow()
    {
        while (_usedCrossbowQueue.Count > 0)
        {
            var crossbow = _usedCrossbowQueue.Dequeue();
            _testCrossbowPool.ReturnObject(crossbow);
        }
    }

    private Vector3 _GetRandomPos()
    {
        var posX = UnityEngine.Random.Range(-RANDON_POS_VALUE, RANDON_POS_VALUE);
        var posY = UnityEngine.Random.Range(-RANDON_POS_VALUE, RANDON_POS_VALUE);
        return new Vector3(posX, posY).normalized;
    }

    private async UniTaskVoid _ReturnCrossbowAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_effectTime));

        _ReturnCrossbow();
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));

        ++_crossbowAttackCount;
        if (_crossbowAttackCount < MAX_CROSSBOW_ATTACK_COUNT)
            _isAttack = true;
    }
}
