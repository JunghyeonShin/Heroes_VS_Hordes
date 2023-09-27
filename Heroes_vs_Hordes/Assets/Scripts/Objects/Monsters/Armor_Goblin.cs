using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Goblin : Monster
{
    protected override void Awake()
    {
        base.Awake();
        _monsterName = Define.RESOURCE_MONSTER_ARMOR_GOBLIN;
    }
}
