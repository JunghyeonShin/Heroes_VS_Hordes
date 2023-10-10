using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnClubGoblin : SpawnMonster
{
    private const int SPAWN_NORMAL_GOBLIN_COUNT = 10;
    private const float SPAWN_NORMAL_GOBLIN_TIME = 10f;

    public override void Spawn()
    {
        _stopSpawnMonster = false;
        _Spawn().Forget();
    }

    private async UniTaskVoid _Spawn()
    {
        if (_stopSpawnMonster)
            return;

        _spawnMonsterCount = UnityEngine.Random.Range(MIN_SPAWN_MONSTER_COUNT, SPAWN_NORMAL_GOBLIN_COUNT);
        _delaySpawnMonsterTime = UnityEngine.Random.Range(MIN_SPAWN_MONSTER_TIME, SPAWN_NORMAL_GOBLIN_TIME);
        for (int ii = 0; ii < _spawnMonsterCount; ++ii)
            MonsterSpawner.RegistSpawnMonster(UnityEngine.Random.Range(0, MonsterSpawner.SpawnPointTotalIndex), Define.RESOURCE_MONSTER_CLUB_GOBLIN);
        await UniTask.Delay(TimeSpan.FromSeconds(_delaySpawnMonsterTime));

        _Spawn().Forget();
    }
}
