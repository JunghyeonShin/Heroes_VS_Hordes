using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDivineAuraController : MonoBehaviour
{
    [SerializeField] private TestHeroController _testHeroController;
    [SerializeField] private GameObject _testDivineArua;

    private ObjectPool _testDivineAuraPool = new ObjectPool();
    private Queue<GameObject> _usedDivineQueue = new Queue<GameObject>();

    private float _attack;
    private float _attackCooldown;
    private float _effectRange;
    private float _effectTime;
    private int _weaponLevel;

    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const int CREATE_TEST_WEAPON_COUNT = 1;
    private const int ADJUST_WEAPON_LEVEL = 2;
    private const int INIT_WEAPON_LEVEL = 1;
    private const int MAX_WEAPON_LEVEL = 5;

    public void Init()
    {
        _testDivineAuraPool.InitPool(_testDivineArua, gameObject, CREATE_TEST_WEAPON_COUNT);
    }

    public void SetAbility()
    {
        var weaponAbility = Manager.Instance.Data.WeaponAbilityDic[Define.WEAPON_DIVINE_AURA];
        var weaponLevelAbilityList = Manager.Instance.Data.WeaponLevelAbilityDic[Define.WEAPON_DIVINE_AURA];

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

        var weaponEffectRange = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponEffectRange += weaponLevelAbilityList[ii].EffectRange;
        }
        _effectRange = weaponAbility.EffectRange * (DEFAULT_ABILITY_VALUE + weaponEffectRange);
        _effectTime = weaponAbility.EffectTime;
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

    public void StartDivineAuraAttack()
    {
        var testSceneUI = Manager.Instance.UI.CurrentSceneUI as UI_TestScene;
        testSceneUI.SetWeaponName(Define.WEAPON_DIVINE_AURA);
        testSceneUI.SetAbilityText(_attack, _attackCooldown, 0f, _effectRange, _effectTime, 0f, 0f);

        _Attack();
    }

    private void _Attack()
    {
        GameObject testDivineAuraGO;
        if (_usedDivineQueue.Count > 0)
            testDivineAuraGO = _usedDivineQueue.Peek();
        else
        {
            testDivineAuraGO = _GetDivineAura();
            _usedDivineQueue.Enqueue(testDivineAuraGO);
        }
        var testDivineAura = Utils.GetOrAddComponent<TestDivineAura>(testDivineAuraGO);
        testDivineAura.Init(_testHeroController.transform.position, _attack, _attackCooldown, _effectRange, _effectTime);
        Utils.SetActive(testDivineAuraGO, true);
    }

    private GameObject _GetDivineAura()
    {
        return _testDivineAuraPool.GetObject();
    }

    private void _ReturnDivineAura()
    {
        while (_usedDivineQueue.Count > 0)
        {
            var divineAura = _usedDivineQueue.Dequeue();
            if (divineAura.activeSelf)
                _testDivineAuraPool.ReturnObject(divineAura);
        }
    }
}
