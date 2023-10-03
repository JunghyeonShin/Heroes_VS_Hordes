using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalMonster : Monster
{
    private const float REVERSE_ANGLE = -1f;
    private const float CHECK_DIRECTION = 0f;

    private const float DELAY_REATTACK_TIME = 2f;

    protected override void Awake()
    {
        base.Awake();

        _delayReattackTime = DELAY_REATTACK_TIME;
    }

    private void FixedUpdate()
    {
        if (null == Target)
            return;

        var monsterToHeroVec = Target.position - transform.position;
        var monsterToHeroNormalVec = new Vector2(monsterToHeroVec.x, monsterToHeroVec.y).normalized;
        _LookHero(monsterToHeroNormalVec);
        _ChaseHero(monsterToHeroNormalVec);
    }

    public override void OnDamage(float damage)
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

    public override void ReturnMonster()
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
}
