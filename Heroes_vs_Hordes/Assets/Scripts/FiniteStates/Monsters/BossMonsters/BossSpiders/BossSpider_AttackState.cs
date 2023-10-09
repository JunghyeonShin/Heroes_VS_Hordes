using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpider_AttackState : BossMonsterState
{
    private Dictionary<EBossSpiderAttackTypes, IBossSpiderAttack> _bossSpiderAttackDic = new Dictionary<EBossSpiderAttackTypes, IBossSpiderAttack>();

    private const float DELAY_IDLE_STATE = 1f;
    private const float DELAY_INDIVIDUAL_HATCH_SPIDER = 0.5f;
    private const float DELAY_SPRAY_WEB = 2f;
    private const float DELAY_INDIVIDUAL_SPRAY_WEB = 1f;
    private const float CHECK_SPRAY_WEB = 0.65f;
    private const int MIN_HATCH_SPIDER_COUNT = 3;
    private const int MAX_HATCH_SPIDER_COUNT = 5;
    private const int MIN_SPRAY_WEB_COUNT = 2;
    private const int MAX_SPRAY_WEB_COUNT = 5;

    public BossSpider_AttackState(GameObject owner) : base(owner)
    {

    }

    public override void InitState()
    {
        base.InitState();

        _bossSpiderAttackDic.Add(EBossSpiderAttackTypes.HatchSpider, new BossSpiderAttack_HatchSpider());
        _bossSpiderAttackDic.Add(EBossSpiderAttackTypes.SprayWeb, new BossSpiderAttack_SprayWeb());

        foreach (var bossSpiderAttack in _bossSpiderAttackDic)
            bossSpiderAttack.Value.Init(_owner);
    }

    public override void EnterState()
    {
        if (false == _owner.activeSelf)
            return;

        _Attack().Forget();
    }

    public override void ExitState()
    {

    }

    public override void FixedUpdateState()
    {

    }

    public override void UpdateState()
    {

    }

    public override void ReturnObject()
    {
        foreach (var bossSpiderAttack in _bossSpiderAttackDic.Values)
            bossSpiderAttack.ReturnObject();
    }

    private async UniTaskVoid _Attack()
    {
        var hatchSpiderCount = UnityEngine.Random.Range(MIN_HATCH_SPIDER_COUNT, MAX_HATCH_SPIDER_COUNT);
        for (int ii = 0; ii < hatchSpiderCount; ++ii)
        {
            if (false == _owner.activeSelf)
                return;

            _bossSpiderAttackDic[EBossSpiderAttackTypes.HatchSpider].Attack(_bossMonster.GetRandomPosition());
            await UniTask.Delay(TimeSpan.FromSeconds(DELAY_INDIVIDUAL_HATCH_SPIDER));
        }

        var sprayWebChance = UnityEngine.Random.value;
        if (_CheckSprayWeb(sprayWebChance))
        {
            await UniTask.Delay(TimeSpan.FromSeconds(DELAY_SPRAY_WEB));

            var spraySpiderCount = UnityEngine.Random.Range(MIN_SPRAY_WEB_COUNT, MAX_SPRAY_WEB_COUNT);
            for (int ii = 0; ii < spraySpiderCount; ++ii)
            {
                if (false == _owner.activeSelf)
                    return;

                _bossSpiderAttackDic[EBossSpiderAttackTypes.SprayWeb].Attack(_bossMonster.GetRandomPosition());
                await UniTask.Delay(TimeSpan.FromSeconds(DELAY_INDIVIDUAL_SPRAY_WEB));
            }
        }
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_IDLE_STATE));

        _bossMonster.ChangeState(EStateTypes.Idle);
    }

    private bool _CheckSprayWeb(float value)
    {
        return value <= CHECK_SPRAY_WEB;
    }
}
