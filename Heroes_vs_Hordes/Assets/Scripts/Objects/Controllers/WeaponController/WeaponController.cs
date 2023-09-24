using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponController : MonoBehaviour, IAbilityController
{
    protected string _weaponName;
    protected int _initWeaponCount;

    protected Queue<GameObject> _usedWeaponQueue = new Queue<GameObject>();

    protected float _attack;
    protected float _attackCooldown;
    protected float _speed;
    protected float _effectRange;
    protected float _effectTime;
    protected float _projectileCount;

    [SerializeField] private GameObject _weaponPrefab;
    private ObjectPool _weaponPool = new ObjectPool();

    private bool _startAttack;

    private const float DELAY_START_ATTACK_TIME = 0.3f;

    private void Awake()
    {
        _Init();
    }

    protected virtual void _Init()
    {
        _weaponPool.InitPool(_weaponPrefab, gameObject, _initWeaponCount);
        Utils.SetActive(_weaponPrefab, false);
    }

    protected virtual bool _Attack()
    {
        if (false == _startAttack)
            return false;
        return true;
    }

    public void SetAbilities()
    {
        var usedHero = Manager.Instance.Ingame.UsedHero;
        var weaponLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(_weaponName);
        _attack = WeaponAbility.GetWeaponAttack(usedHero.HeroName, _weaponName, weaponLevel);
        _attackCooldown = WeaponAbility.GetWeaponAttackCooldown(_weaponName, weaponLevel);
        _speed = WeaponAbility.GetWeaponSpeed(_weaponName, weaponLevel);
        _effectRange = WeaponAbility.GetWeaponEffectRange(_weaponName, weaponLevel);
        _effectTime = WeaponAbility.GetWeaponEffectTime(_weaponName, weaponLevel);
        _projectileCount = WeaponAbility.GetWeaponProjectileCount(_weaponName, weaponLevel);

        if (false == _startAttack)
            _StartAttack().Forget();
    }

    public void ReturnAbilities()
    {
        _startAttack = false;
        _ReturnWeapon();
    }

    protected GameObject _GetWeapon()
    {
        return _weaponPool.GetObject();
    }

    protected void _ReturnWeapon()
    {
        while (_usedWeaponQueue.Count > 0)
        {
            var weapon = _usedWeaponQueue.Dequeue();
            if (weapon.activeSelf)
                _weaponPool.ReturnObject(weapon);
        }
    }

    private async UniTaskVoid _StartAttack()
    {
        _startAttack = true;
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_START_ATTACK_TIME));

        _Attack();
    }
}
