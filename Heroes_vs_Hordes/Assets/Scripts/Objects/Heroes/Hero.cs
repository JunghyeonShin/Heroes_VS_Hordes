using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : MonoBehaviour, IAbilityController
{
    public event Action<float> ChangeHealthHandler;

    protected Animator _animator;
    protected string _heroName;
    protected string _heroWeaponName;

    protected float _health;
    protected float _defense;
    protected float _attack;
    protected float _attackCooldown;
    protected float _critical;
    protected float _recovery;

    protected bool _detectMonster;
    protected bool _attackMonster;

    private float _totalHealth;

    private float _moveSpeed;

    private event Action _levelUpAbilityHandler;
    private event Action<float> _changeExpHandler;
    private event Action<int> _changeLevelHandler;
    private float _exp;
    private int _level;
    private bool _levelUp;

    public string HeroName { get { return _heroName; } }
    public string HeroWeaponName { get { return _heroWeaponName; } }
    public float MoveSpeed
    {
        get
        {
            if (IsSlow)
                return _moveSpeed * SLOW_VALUE;
            return _moveSpeed;
        }
    }
    public bool IsDead { get; private set; }
    public bool IsSlow { get; set; }

    private const float DELAY_LEVEL_UP = 0.2f;
    private const float DELAY_ENHANCE_ABILITY = 1f;
    private const float DELAY_RECOVERY = 6f;
    private const float INIT_EXP = 0f;
    private const float ZERO_HEALTH = 0f;
    private const float ADJUST_RECOVERY_CYCLE = 0.1f;
    private const float SLOW_VALUE = 0.65f;
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
        _levelUpAbilityHandler -= ingame.LevelUpHeroAbility;
        _levelUpAbilityHandler += ingame.LevelUpHeroAbility;
        ingame.ChangeHeroLevelUpPostProcessingHandler -= _LevelUpPostProcessing;
        ingame.ChangeHeroLevelUpPostProcessingHandler += _LevelUpPostProcessing;
    }

    private void OnEnable()
    {
        IsDead = false;

        _exp = INIT_EXP;
        _level = INIT_LEVEL;
        _changeExpHandler?.Invoke(INIT_EXP);
        _changeLevelHandler?.Invoke(INIT_LEVEL);
    }

    private void Update()
    {
        if (false == IsDead)
            _DetectMonster();
    }

    public virtual void SetAbilities()
    {
        _attackCooldown = HeroAbility.GetHeroAttackCooldown(_heroName);
        _moveSpeed = HeroAbility.GetHeroMoveSpeed(_heroName);
        _recovery = HeroAbility.GetHeroRecovery();
    }

    public virtual void ReturnAbilities()
    {

    }

    protected abstract void _DetectMonster();

    protected abstract void _AttackMonster();

    public void InitHeroAbilities()
    {
        _totalHealth = HeroAbility.GetHeroHealth(_heroName);
        _health = _totalHealth;
        _defense = HeroAbility.GetHeroDeffence(_heroName);
        SetAbilities();
        _Recovery().Forget();
    }

    public void OnDamage(float damage)
    {
        if (_health <= ZERO_HEALTH)
            return;

        _health -= damage;
        if (_health <= ZERO_HEALTH)
            _health = ZERO_HEALTH;
        ChangeHealthHandler?.Invoke(_health / _totalHealth);
        if (_health <= ZERO_HEALTH)
        {
            IsDead = true;

            var heroDeath = Manager.Instance.Object.HeroDeath;
            var floatHeroDeath = Utils.GetOrAddComponent<FloatHeroDeath>(heroDeath);
            floatHeroDeath.SetTransform(transform.position);
            Utils.SetActive(heroDeath, true);

            Utils.SetActive(gameObject, false);

            Manager.Instance.Ingame.CurrentWave.OnDeadHero();
        }
    }

    public void GetExp(float exp)
    {
        if (IsDead)
            return;

        _exp += exp;
        if (_levelUp)
            return;

        _GetExp();
    }

    private void _GetExp()
    {
        var needExpToLevelUp = Manager.Instance.Data.RequiredExpDataList[_level - ADJUST_LEVEL];
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

        var needExpToLevelUp = Manager.Instance.Data.RequiredExpDataList[_level - ADJUST_LEVEL];
        _level += INCREASE_LEVEL_VALUE;
        _changeLevelHandler?.Invoke(_level);

        _exp -= needExpToLevelUp;
        needExpToLevelUp = Manager.Instance.Data.RequiredExpDataList[_level - ADJUST_LEVEL];
        if (_exp >= needExpToLevelUp)
            _LevelUp().Forget();
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(DELAY_ENHANCE_ABILITY), ignoreTimeScale: true);
            _levelUpAbilityHandler?.Invoke();
        }
    }

    private void _LevelUpPostProcessing()
    {
        _levelUp = false;
        _GetExp();
    }

    private async UniTaskVoid _Recovery()
    {
        if (_health >= _totalHealth)
        {
            await UniTask.Yield();
            _Recovery().Forget();
            return;
        }
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_RECOVERY));

        var recoveryValue = _totalHealth * _recovery * ADJUST_RECOVERY_CYCLE;
        _health += recoveryValue;
        if (_health >= _totalHealth)
            _health = _totalHealth;
        ChangeHealthHandler?.Invoke(_health / _totalHealth);
        _Recovery().Forget();
    }
}
