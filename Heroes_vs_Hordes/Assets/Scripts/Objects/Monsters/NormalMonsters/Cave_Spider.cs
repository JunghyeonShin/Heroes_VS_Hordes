using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave_Spider : NormalMonster
{
    protected override void Awake()
    {
        base.Awake();

        _monsterName = Define.RESOURCE_MONSTER_CAVE_SPIDER;
    }
}
