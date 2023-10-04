using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    protected string _monsterName;
    protected float _totalHealth;
    protected float _health;
    protected float _moveSpeed;
    protected float _attack;

    protected Rigidbody2D _rigidbody;
    protected float _delayReattackTime;

    private bool _isAttack;

    public Transform Target { get; set; }

    protected const float ZERO_HEALTH = 0f;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isAttack)
            return;

        if (collision.CompareTag(Define.TAG_HERO))
        {
            var hero = Utils.GetOrAddComponent<Hero>(collision.gameObject);
            hero.OnDamage(_attack);
            _isAttack = true;
            _Reattack().Forget();
        }
    }

    public abstract void OnDamage(float damage);

    public abstract void ReturnMonster();

    public void InitMonsterAbilities()
    {
        var monsterInfo = Manager.Instance.Data.MonsterInfoDic[_monsterName];
        _totalHealth = monsterInfo.Health;
        _health = _totalHealth;
        _moveSpeed = monsterInfo.MoveSpeed;
        _attack = monsterInfo.Attack;
    }

    private async UniTaskVoid _Reattack()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_delayReattackTime));

        _isAttack = false;
    }
}
