using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneMage_Projectile : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private string _weaponName;

    private Vector3 _targetPos;
    private Vector2 _moveVec;
    private float _attack;
    private float _critical;
    private float _moveSpeed;
    private float _penetraitCount;
    private Action<GameObject> _returnObjectHandler;

    private const float MIN_DAMAGE_TEXT_POSITION_X = -1f;
    private const float MAX_DAMAGE_TEXT_POSITION_X = 1f;
    private const float DAMAGE_TEXT_POSITION_Y = 1f;
    private const float TWO_MULTIPLES_VALUE = 2f;
    private const float END_LIFE = 0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _weaponName = Define.WEAPON_ARCANE_MAGE_WAND;
    }

    private void OnEnable()
    {
        var aimingVec = _targetPos - transform.position;
        var aimingNormalVec = new Vector2(aimingVec.x, aimingVec.y).normalized;
        _moveVec = aimingNormalVec * _moveSpeed * Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _moveVec);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_MONSTER))
        {
            --_penetraitCount;
            if (_penetraitCount <= END_LIFE)
                _returnObjectHandler?.Invoke(gameObject);

            var randomPos = new Vector3(UnityEngine.Random.Range(MIN_DAMAGE_TEXT_POSITION_X, MAX_DAMAGE_TEXT_POSITION_X), DAMAGE_TEXT_POSITION_Y, 0f);
            var initDamageTextPos = collision.transform.position + randomPos;
            var damageTextGO = Manager.Instance.Object.GetDamageText();
            var damageText = Utils.GetOrAddComponent<DamageText>(damageTextGO);
            var isCritical = HeroAbility.IsCritical(_critical);
            var attack = _attack;
            if (isCritical)
                attack = _attack * TWO_MULTIPLES_VALUE;
            damageText.FloatDamageText(initDamageTextPos, attack, isCritical);
            Utils.SetActive(damageTextGO, true);

            var monster = Utils.GetOrAddComponent<Monster>(collision.gameObject);
            monster.OnDamage(_attack);
        }
    }

    public void Init(Vector3 initPos, Vector3 targetPos, Action<GameObject> returnObjectCallback)
    {
        transform.position = initPos;
        _targetPos = targetPos;

        var usedHero = Manager.Instance.Ingame.UsedHero;
        var weaponLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(_weaponName);
        _attack = WeaponAbility.GetWeaponAttack(usedHero.HeroName, _weaponName, weaponLevel);
        _critical = HeroAbility.GetHeroCritical(usedHero.HeroName);
        _moveSpeed = HeroAbility.GetHeroProjectileSpeed(usedHero.HeroName);
        _penetraitCount = WeaponAbility.GetWeaponPenetraitCount(_weaponName, weaponLevel);

        _returnObjectHandler -= returnObjectCallback;
        _returnObjectHandler += returnObjectCallback;
    }
}
