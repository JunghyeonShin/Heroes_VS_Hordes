using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal_Goblin : Monster
{
    protected override void Awake()
    {
        base.Awake();
        _monsterName = Define.RESOURCE_MONSTER_NORMAL_GOBLIN;
    }
}
