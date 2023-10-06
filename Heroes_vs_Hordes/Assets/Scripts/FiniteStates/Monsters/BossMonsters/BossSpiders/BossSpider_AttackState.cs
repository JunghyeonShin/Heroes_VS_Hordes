using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpider_AttackState : BossMonsterState
{
    private Dictionary<EBossSpiderAttackTypes, IBossSpiderAttack> _bossSpiderAttackDic = new Dictionary<EBossSpiderAttackTypes, IBossSpiderAttack>();

    private const float DELAY_IDLE_STATE = 1f;

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

    private async UniTaskVoid _Attack()
    {
        _bossSpiderAttackDic[EBossSpiderAttackTypes.HatchSpider].Attack(_bossMonster.GetRandomPosition());
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_IDLE_STATE));

        _bossMonster.ChangeState(EStateTypes.Idle);
    }
}
