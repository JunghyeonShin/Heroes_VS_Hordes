using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : MonoBehaviour
{
    protected float _health;
    protected float _defense;
    protected float _attack;
    protected float _attackCooldown;
    protected float _critical;

    protected Animator _animator;

    protected bool _detectMonster;
    protected bool _attackMonster;

    private event Action<float> _changeExpHandler;
    private event Action<int> _changeLevelHandler;
    private float _exp;
    private int _level;
    private bool _levelUp;
    private bool _levelUpPostProcessing;

    public float MoveSpeed { get; private set; }
    public float ProjectileSpeed { get; private set; }

    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const float MIN_CRITICAL_VALUE = 0f;
    private const float MAX_CRITICAL_VALUE = 1f;
    private const float INIT_EXP = 0;
    private const int INIT_LEVEL = 1;
    private const int ADJUST_LEVEL = 1;
    private const int INCREASE_LEVEL_VALUE = 1;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();

        var ingame = Manager.Instance.Ingame;
        _changeExpHandler -= ingame.ChangeHeroExp;
        _changeExpHandler += ingame.ChangeHeroExp;
        _changeLevelHandler -= ingame.ChangeHeroLevel;
        _changeLevelHandler += ingame.ChangeHeroLevel;
        ingame.ChangeHeroLevelPostProcessingHandler -= _LevelUpPostProcessing;
        ingame.ChangeHeroLevelPostProcessingHandler += _LevelUpPostProcessing;
    }

    private void OnEnable()
    {
        _exp = INIT_EXP;
        _level = INIT_LEVEL;
        _changeExpHandler?.Invoke(INIT_EXP);
        _changeLevelHandler?.Invoke(INIT_LEVEL);
    }

    private void Update()
    {
        _DetectMonster();
    }

    protected abstract void _DetectMonster();

    protected abstract void _AttackMonster(Vector3 targetPos);

    public void SetHeroAbilities()
    {
        var heroCommonAbility = Manager.Instance.Data.HeroCommonAbility;
        var heroIndividualAbility = Manager.Instance.Data.HeroIndividualAbilityDic[Define.RESOURCE_HERO_ARCANE_MAGE];

        _health = heroCommonAbility.Health + heroIndividualAbility.Health;
        _defense = heroCommonAbility.Defense + heroIndividualAbility.Defense;
        _attack = heroCommonAbility.Attack * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.Attack);
        _attackCooldown = heroCommonAbility.AttackCooldown + heroIndividualAbility.AttackCooldown;
        _critical = heroCommonAbility.Critical + heroIndividualAbility.Critical;
        MoveSpeed = heroCommonAbility.MoveSpeed * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.MoveSpeed);
        ProjectileSpeed = heroCommonAbility.ProjectileSpeed * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.ProjectileSpeed);
    }

    public void GetExp(float exp)
    {
        _exp += exp;
        if (_levelUp)
            return;

        _GetExp();
    }

    protected bool _IsCritical()
    {
        var randomValue = UnityEngine.Random.Range(MIN_CRITICAL_VALUE, MAX_CRITICAL_VALUE);
        if (randomValue <= _critical)
            return true;
        return false;
    }

    private void _GetExp()
    {
        var expToNextLevel = Manager.Instance.Data.RequiredExpList[_level - ADJUST_LEVEL];
        var value = _exp / expToNextLevel;
        _changeExpHandler?.Invoke(value);
        if (_exp >= expToNextLevel)
            _LevelUp(expToNextLevel).Forget();
    }

    private async UniTaskVoid _LevelUp(float expToNextLevel)
    {
        _levelUp = true;

        var levelUpTextGO = Manager.Instance.Object.LevelUpText;
        var levelUpText = Utils.GetOrAddComponent<LevelUpText>(levelUpTextGO);
        levelUpText.FloatLevelUpText(transform);
        Utils.SetActive(levelUpTextGO, true);

        _level += INCREASE_LEVEL_VALUE;
        _changeLevelHandler?.Invoke(_level);
        await UniTask.WaitUntil(() => _levelUpPostProcessing);

        _levelUp = false;
        _levelUpPostProcessing = false;

        _exp -= expToNextLevel;
        _GetExp();
    }

    private void _LevelUpPostProcessing()
    {
        _levelUpPostProcessing = true;
    }
}
