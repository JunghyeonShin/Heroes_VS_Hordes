using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNormalBat : SpawnMonster
{
    private const int SPAWN_NORMAL_BAT_COUNT = 10;
    private const float SPAWN_NORMAL_BAT_TIME = 10f;

    public override void Spawn()
    {
        _Spawn().Forget();
    }

    private async UniTaskVoid _Spawn()
    {
        if (false == MonsterSpawner.SpawnMonster)
            return;

        _spawnMonsterCount = UnityEngine.Random.Range(MIN_SPAWN_MONSTER_COUNT, SPAWN_NORMAL_BAT_COUNT);
        _delaySpawnMonsterTime = UnityEngine.Random.Range(MIN_SPAWN_MONSTER_TIME, SPAWN_NORMAL_BAT_TIME);
        for (int ii = 0; ii < _spawnMonsterCount; ++ii)
            MonsterSpawner.RegistSpawnMonster(UnityEngine.Random.Range(0, MonsterSpawner.SpawnPointTotalIndex), Define.RESOURCE_MONSTER_NORMAL_BAT);
        await UniTask.Delay(TimeSpan.FromSeconds(_delaySpawnMonsterTime));

        _Spawn().Forget();
    }
}
