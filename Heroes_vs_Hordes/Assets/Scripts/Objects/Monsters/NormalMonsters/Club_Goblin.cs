using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club_Goblin : NormalMonster
{
    protected override void Awake()
    {
        base.Awake();

        _monsterName = Define.RESOURCE_MONSTER_CLUB_GOBLIN;
    }
}
