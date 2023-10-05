using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpider_AttackState : BossMonsterState
{
    public BossSpider_AttackState(GameObject owner) : base(owner)
    {

    }

    public override void EnterState()
    {
        _bossMonster.ChangeState(EStateTypes.Idle);
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
}
