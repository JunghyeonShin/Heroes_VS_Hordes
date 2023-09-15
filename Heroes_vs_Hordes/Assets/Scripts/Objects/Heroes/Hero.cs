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

    private event Action _enhanceAbilityHandler;
    private event Action<float> _changeExpHandler;
    private event Action<int> _changeLevelHandler;
    private float _exp;
    private int _level;
    private bool _levelUp;

    public bool IsDead { get { return _health <= HP_ZERO; } }
    public float MoveSpeed { get; private set; }
    public float ProjectileSpeed { get; private set; }

    private const float DELAY_LEVEL_UP = 0.2f;
    private const float DELAY_ENHANCE_ABILITY = 1f;
    private const float DEFAULT_ABILITY_VALUE = 1f;
    private const float MIN_CRITICAL_VALUE = 0f;
    private const float MAX_CRITICAL_VALUE = 1f;
    private const float INIT_EXP = 0;
    private const float HP_ZERO = 0f;
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
        _enhanceAbilityHandler -= ingame.EnhanceHeroAbility;
        _enhanceAbilityHandler += ingame.EnhanceHeroAbility;
        ingame.ChangeHeroLevelUpPostProcessingHandler -= _LevelUpPostProcessing;
        ingame.ChangeHeroLevelUpPostProcessingHandler += _LevelUpPostProcessing;
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

    #region TEST
    public void SetDead()
    {
        _health = HP_ZERO;
        Manager.Instance.Ingame.ClearIngame();
    }
    #endregion

    protected bool _IsCritical()
    {
        var randomValue = UnityEngine.Random.Range(MIN_CRITICAL_VALUE, MAX_CRITICAL_VALUE);
        if (_critical >= randomValue)
            return true;
        return false;
    }

    private void _GetExp()
    {
        var needExpToLevelUp = Manager.Instance.Data.RequiredExpList[_level - ADJUST_LEVEL];
        var value = _exp / needExpToLevelUp;
        _changeExpHandler?.Invoke(value);
        if (_exp >= needExpToLevelUp)
        {
            _FloatLevelUpText();
            _LevelUp().Forget();
        }
    }

    private void _FloatLevelUpText()
    {
        var levelUpTextGO = Manager.Instance.Object.LevelUpText;
        var levelUpText = Utils.GetOrAddComponent<LevelUpText>(levelUpTextGO);
        levelUpText.FloatLevelUpText(transform);
        Utils.SetActive(levelUpTextGO, true);
    }

    private async UniTaskVoid _LevelUp()
    {
        if (_levelUp)
            await UniTask.Delay(TimeSpan.FromSeconds(DELAY_LEVEL_UP), ignoreTimeScale: true);
        _levelUp = true;

        var needExpToLevelUp = Manager.Instance.Data.RequiredExpList[_level - ADJUST_LEVEL];
        _level += INCREASE_LEVEL_VALUE;
        _changeLevelHandler?.Invoke(_level);

        _exp -= needExpToLevelUp;
        needExpToLevelUp = Manager.Instance.Data.RequiredExpList[_level - ADJUST_LEVEL];
        if (_exp >= needExpToLevelUp)
            _LevelUp().Forget();
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(DELAY_ENHANCE_ABILITY), ignoreTimeScale: true);
            _enhanceAbilityHandler?.Invoke();
        }
    }

    private void _LevelUpPostProcessing()
    {
        _levelUp = false;
        _GetExp();
    }
}
