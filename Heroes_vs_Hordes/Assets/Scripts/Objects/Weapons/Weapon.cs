using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected Collider2D _collider;
    protected Rigidbody2D _rigidbody;
    protected string _weaponName;

    protected Vector3 _targetPos;
    protected float _attack;
    protected float _critical;
    protected float _attackCooldown;
    protected float _speed;
    protected float _effectRange;
    protected float _effectTime;

    protected const float MIN_DAMAGE_TEXT_POSITION_X = -1f;
    protected const float MAX_DAMAGE_TEXT_POSITION_X = 1f;
    protected const float DAMAGE_TEXT_POSITION_Y = 1f;
    protected const float TWO_MULTIPLES_VALUE = 2f;
    protected const float ZERO_SECOND = 0f;
    protected const float ONE_SECOND = 1f;

    protected virtual void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_MONSTER))
        {
            var randomPos = new Vector3(UnityEngine.Random.Range(MIN_DAMAGE_TEXT_POSITION_X, MAX_DAMAGE_TEXT_POSITION_X), DAMAGE_TEXT_POSITION_Y, 0f);
            var initDamageTextPos = collision.transform.position + randomPos;
            var damageTextGO = Manager.Instance.Object.GetDamageText();
            var damageText = Utils.GetOrAddComponent<DamageText>(damageTextGO);
            var attack = _attack;
            var isCritical = HeroAbility.IsCritical(_critical);
            if (isCritical)
                attack = _attack * TWO_MULTIPLES_VALUE;
            damageText.FloatDamageText(initDamageTextPos, attack, isCritical);
            Utils.SetActive(damageTextGO, true);

            var monster = Utils.GetOrAddComponent<Monster>(collision.gameObject);
            monster.OnDamaged(attack);
        }
    }

    protected virtual void Update()
    {

    }

    protected virtual void LateUpdate()
    {

    }

    public virtual void Init(Vector3 initPos)
    {
        transform.position = initPos;

        var usedHero = Manager.Instance.Ingame.UsedHero;
        var weaponLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(_weaponName);
        _attack = WeaponAbility.GetWeaponAttack(usedHero.HeroName, _weaponName, weaponLevel);
        _critical = HeroAbility.GetHeroCritical(usedHero.HeroName);
        _attackCooldown = WeaponAbility.GetWeaponAttackCooldown(_weaponName, weaponLevel);
        _speed = WeaponAbility.GetWeaponSpeed(_weaponName, weaponLevel);
        _effectRange = WeaponAbility.GetWeaponEffectRange(_weaponName, weaponLevel);
        _effectTime = WeaponAbility.GetWeaponEffectTime(_weaponName, weaponLevel);
    }

    public virtual void Init(Vector3 initPos, Vector3 targetPos)
    {
        Init(initPos);
        _targetPos = targetPos;
    }
}
