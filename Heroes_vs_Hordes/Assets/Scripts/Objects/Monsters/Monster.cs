using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    protected string _monsterName;
    protected float _health;
    protected float _moveSpeed;
    protected float _attack;

    private Rigidbody2D _rigidbody;

    private bool _isAttack;

    public Transform Target { get; set; }

    private const float REVERSE_ANGLE = -1f;
    private const float CHECK_DIRECTION = 0f;
    private const float ZERO_HEALTH = 0f;
    private const float DELAY_REATTACK_TIME = 2f;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var monsterToHeroVec = Target.position - transform.position;
        var monsterToHeroNormalVec = new Vector2(monsterToHeroVec.x, monsterToHeroVec.y).normalized;
        _LookHero(monsterToHeroNormalVec);
        _ChaseHero(monsterToHeroNormalVec);
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

    public void InitMonsterAbilities()
    {
        var monsterInfo = Manager.Instance.Data.MonsterInfoDic[_monsterName];
        _health = monsterInfo.Health;
        _moveSpeed = monsterInfo.MoveSpeed;
        _attack = monsterInfo.Attack;
    }

    public virtual void OnDamage(float damage)
    {
        if (_health <= ZERO_HEALTH)
            return;

        _health -= damage;
        if (_health <= ZERO_HEALTH)
        {
            var waveIndex = Manager.Instance.Data.ChapterInfoDataList[Define.CURRENT_CHAPTER_INDEX].WaveIndex[Manager.Instance.Ingame.CurrentWaveIndex];
            if (Define.INDEX_GOLD_RUSH_WAVE == waveIndex)
                _ShowDropItem<Gold>(Define.RESOURCE_GOLD);
            else
                _ShowDropItem<ExpGem>(Define.RESOURCE_EXP_GEM);
        }
    }

    public void ReturnMonster()
    {
        Manager.Instance.Object.ReturnMonster(_monsterName, gameObject);
    }

    private void _LookHero(Vector2 monsterToHeroNormalVec)
    {
        var lookAngle = Vector2.Angle(Vector2.up, monsterToHeroNormalVec);
        if (_IsLocatedTargetRightSide(monsterToHeroNormalVec.x))
            lookAngle *= REVERSE_ANGLE;
        _rigidbody.rotation = lookAngle;
    }

    private bool _IsLocatedTargetRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }

    private void _ChaseHero(Vector2 monsterToHeroNormalVec)
    {
        var moveVec = monsterToHeroNormalVec * _moveSpeed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + moveVec);
    }

    private void _ShowDropItem<T>(string dropItemKey) where T : DropItem
    {
        Manager.Instance.Object.GetDropItem(dropItemKey, (dropItemGO) =>
        {
            var dropItem = Utils.GetOrAddComponent<T>(dropItemGO);
            dropItem.InitTransform(transform.position);
            Utils.SetActive(dropItemGO, true);

            Manager.Instance.Ingame.OnDeadMonster();
            ReturnMonster();
        });
    }

    private async UniTaskVoid _Reattack()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_REATTACK_TIME));

        _isAttack = false;
    }
}
