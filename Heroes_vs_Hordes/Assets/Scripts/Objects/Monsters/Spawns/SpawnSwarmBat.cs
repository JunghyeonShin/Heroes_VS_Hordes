using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSwarmBat : SpawnMonster
{
    private const int SPAWN_SWARM_BAT_COUNT = 10;
    private const float SPAWN_SWARM_BAT_TIME = 10f;

    public override void Spawn()
    {
        _Spawn().Forget();
    }

    private async UniTaskVoid _Spawn()
    {
        if (false == MonsterSpawner.SpawnMonster)
            return;

        _spawnMonsterCount = UnityEngine.Random.Range(MIN_SPAWN_MONSTER_COUNT, SPAWN_SWARM_BAT_COUNT);
        _delaySpawnMonsterTime = UnityEngine.Random.Range(MIN_SPAWN_MONSTER_TIME, SPAWN_SWARM_BAT_TIME);
        var randomPoint = UnityEngine.Random.Range(0, MonsterSpawner.SpawnPointTotalIndex);
        for (int ii = 0; ii < _spawnMonsterCount; ++ii)
            MonsterSpawner.RegistSpawnMonster(randomPoint, Define.RESOURCE_MONSTER_SWARM_BAT);
        await UniTask.Delay(TimeSpan.FromSeconds(_delaySpawnMonsterTime));

        _Spawn().Forget();
    }
}
