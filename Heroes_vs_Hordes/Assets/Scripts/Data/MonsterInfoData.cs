using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfoData
{
    private float _health;
    private float _moveSpeed;
    private float _attack;

    public float Health { get { return _health; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float Attack { get { return _attack; } }

    private const int INDEX_MONSTER_INFO_HEALTH = 1;
    private const int INDEX_MONSTER_INFO_MOVE_SPEED = 2;
    private const int INDEX_MONSTER_INFO_ATTACK = 3;

    public MonsterInfoData(string[] splitData)
    {
        float.TryParse(splitData[INDEX_MONSTER_INFO_HEALTH], out _health);
        float.TryParse(splitData[INDEX_MONSTER_INFO_MOVE_SPEED], out _moveSpeed);
        float.TryParse(splitData[INDEX_MONSTER_INFO_ATTACK], out _attack);
    }
}
