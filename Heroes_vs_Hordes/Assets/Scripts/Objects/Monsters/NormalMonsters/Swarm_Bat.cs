using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarm_Bat : NormalMonster
{
    protected override void Awake()
    {
        base.Awake();

        _monsterName = Define.RESOURCE_MONSTER_SWARM_BAT;
    }
}
