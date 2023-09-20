using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBombController : MonoBehaviour
{
    [SerializeField] private TestHeroController _testHeroController;
    [SerializeField] private GameObject _testBomb;

    private ObjectPool _testBombPool = new ObjectPool();
    private Queue<GameObject> _usedBombQueue = new Queue<GameObject>();

    private float _attack;
    private float _attackCooldown;
    private float _speed;
    private float _effectRange;
    private float _projectileCount;

    private int _weaponLevel;

    private bool _isAttack;
    private int _bombAttackCount;

    private float _finishAttackCount;
    private List<int> _targetMonsterIndexList = new List<int>();

    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const float DEFAULT_DETECT_BOX_ANGLE = 0f;
    private const float FINISH_ATTACK_COUNT = 0f;
    private const int CREATE_TEST_WEAPON_COUNT = 10;
    private const int ADJUST_WEAPON_LEVEL = 2;
    private const int INIT_WEAPON_LEVEL = 1;
    private const int MAX_WEAPON_LEVEL = 5;
    private const int INIT_BOMB_ATTACK_COUNT = 0;
    private const int MAX_BOMB_ATTACK_COUNT = 2;

    private readonly Vector2 OVERLAP_SIZE = new Vector2(22.5f, 40f);

    private void Update()
    {
        if (_isAttack)
            _Attack();
    }

    public void Init()
    {
        _testBombPool.InitPool(_testBomb, gameObject, CREATE_TEST_WEAPON_COUNT);
    }

    public void SetAbility()
    {
        var weaponAbility = Manager.Instance.Data.WeaponAbilityDic[Define.WEAPON_BOMB];
        var weaponLevelAbilityList = Manager.Instance.Data.WeaponLevelAbilityDic[Define.WEAPON_BOMB];

        var weaponAttack = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponAttack += weaponLevelAbilityList[ii].Attack;
        }
        _attack = (weaponAbility.Attack + _testHeroController.Attack) * (DEFAULT_ABILITY_VALUE + weaponAttack);

        _attackCooldown = weaponAbility.AttackCooldown;
        _speed = weaponAbility.Speed;

        var weaponEffectRange = 0f;
        if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
        {
            for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                weaponEffectRange += weaponLevelAbilityList[ii].EffectRange;
        }
        _effectRange = weaponAbility.EffectRange * (DEFAULT_ABILITY_VALUE + weaponEffectRange);

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

    public void StartBombAttack()
    {
        _bombAttackCount = INIT_BOMB_ATTACK_COUNT;
        _isAttack = true;

        var testSceneUI = Manager.Instance.UI.CurrentSceneUI as UI_TestScene;
        testSceneUI.SetWeaponName(Define.WEAPON_BOMB);
        testSceneUI.SetAbilityText(_attack, _attackCooldown, _speed, _effectRange, 0f, _projectileCount, 0f);
    }

    private void _Attack()
    {
        _isAttack = false;
        _finishAttackCount = _projectileCount;
        _targetMonsterIndexList.Clear();
        var layerMask = 1 << LayerMask.NameToLayer(Define.LAYER_MONSTER);
        var monsters = Physics2D.OverlapBoxAll(_testHeroController.transform.position, OVERLAP_SIZE, DEFAULT_DETECT_BOX_ANGLE, layerMask);
        if (monsters.Length > 0)
        {
            for (int ii = 0; ii < _projectileCount; ++ii)
                _targetMonsterIndexList.Add(_GetRandomTargetMonster(monsters.Length));

            for (int ii = 0; ii < _projectileCount; ++ii)
            {
                var testBombGO = _GetBomb();
                _usedBombQueue.Enqueue(testBombGO);
                var testBomb = Utils.GetOrAddComponent<TestBomb>(testBombGO);
                testBomb.Init(_testHeroController.transform.position, monsters[_targetMonsterIndexList[ii]].transform.position, _speed, _attack, _effectRange);
                testBomb.FinishAttackHandler -= _FinishAttack;
                testBomb.FinishAttackHandler += _FinishAttack;
                Utils.SetActive(testBombGO, true);
            }
            _ReturnBombAsync().Forget();
        }
    }

    private void _FinishAttack()
    {
        --_finishAttackCount;
    }

    private GameObject _GetBomb()
    {
        return _testBombPool.GetObject();
    }

    private void _ReturnBomb()
    {
        while (_usedBombQueue.Count > 0)
        {
            var bomb = _usedBombQueue.Dequeue();
            if (bomb.activeSelf)
                _testBombPool.ReturnObject(bomb);
        }
    }

    private int _GetRandomTargetMonster(int monsterIndices)
    {
        return UnityEngine.Random.Range(0, monsterIndices);
    }

    private async UniTaskVoid _ReturnBombAsync()
    {
        while (_finishAttackCount > FINISH_ATTACK_COUNT)
            await UniTask.Yield();

        _ReturnBomb();
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));

        ++_bombAttackCount;
        if (_bombAttackCount < MAX_BOMB_ATTACK_COUNT)
            _isAttack = true;
    }
}
