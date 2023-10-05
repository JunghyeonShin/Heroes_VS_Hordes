using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Spider : BossMonster
{
    private const float DELAY_REATTACK_TIME = 1f;

    protected override void Awake()
    {
        base.Awake();

        _monsterName = Define.RESOURCE_MONSTER_BOSS_SPIDER;
        _delayReattackTime = DELAY_REATTACK_TIME;

        _bossMonsterFSM = Utils.GetOrAddComponent<FiniteStateMachine>(gameObject);
        _bossMonsterFSM.AddState(EStateTypes.Idle, new BossSpider_IdleState(gameObject));
        _bossMonsterFSM.AddState(EStateTypes.Move, new BossSpider_MoveState(gameObject));
        _bossMonsterFSM.AddState(EStateTypes.Attack, new BossSpider_AttackState(gameObject));
        _bossMonsterFSM.AddState(EStateTypes.Dead, new BossSpider_DeadState(gameObject));
    }

    protected override void OnEnable()
    {
        if (null == Target)
            return;

        base.OnEnable();

        ChangeState(EStateTypes.Idle);
    }
}
