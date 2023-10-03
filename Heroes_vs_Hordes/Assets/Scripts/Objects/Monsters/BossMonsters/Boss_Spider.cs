using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Spider : BossMonster
{
    protected override void Awake()
    {
        base.Awake();

        _monsterName = Define.RESOURCE_MONSTER_BOSS_SPIDER;
    }
}
