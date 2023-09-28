using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnMonster
{
    protected float _delaySpawnMonsterTime;
    protected int _spawnMonsterCount;

    public MonsterSpawner MonsterSpawner { get; set; }

    protected const int MIN_SPAWN_MONSTER_COUNT = 5;
    protected const float MIN_SPAWN_MONSTER_TIME = 5f;

    public abstract void Spawn();
}
