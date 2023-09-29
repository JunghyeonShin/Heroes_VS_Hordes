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
    private Action _dieHandler;

    public Transform Target { get; set; }

    private const float REVERSE_ANGLE = -1f;
    private const float CHECK_DIRECTION = 0f;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _dieHandler -= Manager.Instance.Ingame.OnDeadMonster;
        _dieHandler += Manager.Instance.Ingame.OnDeadMonster;
    }

    private void OnDisable()
    {
        _dieHandler -= Manager.Instance.Ingame.OnDeadMonster;
    }

    private void FixedUpdate()
    {
        var monsterToHeroVec = Target.position - transform.position;
        var monsterToHeroNormalVec = new Vector2(monsterToHeroVec.x, monsterToHeroVec.y).normalized;
        _LookHero(monsterToHeroNormalVec);
        _ChaseHero(monsterToHeroNormalVec);
    }

    public virtual void OnDamaged(float damage)
    {
        var waveIndex = Manager.Instance.Data.ChapterInfoDataList[Define.CURRENT_CHAPTER_INDEX].WaveIndex[Manager.Instance.Ingame.CurrentWaveIndex];
        if (Define.INDEX_GOLD_RUSH_WAVE == waveIndex)
            _ShowDropItem<Gold>(Define.RESOURCE_GOLD);
        else
            _ShowDropItem<ExpGem>(Define.RESOURCE_EXP_GEM);
    }

    public void InitMonsterAbilities()
    {
        var monsterInfo = Manager.Instance.Data.MonsterInfoDic[_monsterName];
        _health = monsterInfo.Health;
        _moveSpeed = monsterInfo.MoveSpeed;
        _health = monsterInfo.Health;
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

            _dieHandler?.Invoke();
            ReturnMonster();
        });
    }

    public void ReturnMonster()
    {
        Manager.Instance.Object.ReturnMonster(_monsterName, gameObject);
    }
}
