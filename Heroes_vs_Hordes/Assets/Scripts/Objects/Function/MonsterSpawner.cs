using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;

    private Dictionary<string, SpawnMonster> _allSpawnMonsterDic = new Dictionary<string, SpawnMonster>();
    private List<SpawnMonster> _spawnMonsterList = new List<SpawnMonster>();
    private Queue<string>[] _spawnMonsterQueues;
    private bool[] _spawnMonsters;

    public HeroController HeroController { get; set; }
    public bool SpawnMonster { get; private set; }
    public int SpawnPointTotalIndex { get { return _spawnPoints.Length - 1; } }

    private const float DELAY_RESPAWN_TIME = 0.3f;
    private const int EMPTY_VALUE = 0;

    private void Awake()
    {
        _spawnMonsterQueues = new Queue<string>[_spawnPoints.Length];
        for (int ii = 0; ii < _spawnMonsterQueues.Length; ++ii)
            _spawnMonsterQueues[ii] = new Queue<string>();

        _spawnMonsters = new bool[_spawnPoints.Length];
        for (int ii = 0; ii < _spawnMonsters.Length; ++ii)
            _spawnMonsters[ii] = false;
    }

    private void Update()
    {
        if (SpawnMonster)
            _SpawnMonster();
    }

    public void InitSpawnMonster()
    {
        StopSpawnMonster();

        _spawnMonsterList.Clear();
        var monsterTypes = Manager.Instance.Data.ChapterInfoDataList[Manager.Instance.Ingame.CurrentChapterIndex].MonsterTypes;
        foreach (var monsterType in monsterTypes)
        {
            switch (monsterType)
            {
                case Define.RESOURCE_MONSTER_NORMAL_BAT:
                    _InitSpawnMonster<SpawnNormalBat>(monsterType);
                    break;
                case Define.RESOURCE_MONSTER_SWARM_BAT:
                    _InitSpawnMonster<SpawnSwarmBat>(monsterType);
                    break;
                case Define.RESOURCE_MONSTER_NORMAL_GOBLIN:
                    _InitSpawnMonster<SpawnNormalGoblin>(monsterType);
                    break;
                case Define.RESOURCE_MONSTER_CLUB_GOBLIN:
                    _InitSpawnMonster<SpawnClubGoblin>(monsterType);
                    break;
                case Define.RESOURCE_MONSTER_ARMOR_GOBLIN:
                    _InitSpawnMonster<SpawnArmorGoblin>(monsterType);
                    break;
                case Define.RESOURCE_MONSTER_NORMAL_SKELETON:
                    _InitSpawnMonster<SpawnNormalSkeleton>(monsterType);
                    break;
                case Define.RESOURCE_MONSTER_ARMOR_SKELETON:
                    _InitSpawnMonster<SpawnArmorSkeleton>(monsterType);
                    break;
            }
        }
    }

    public void RegistSpawnMonster(int index, string monster)
    {
        _spawnMonsterQueues[index].Enqueue(monster);
    }

    public void StartSpawnMonster()
    {
        SpawnMonster = true;

        foreach (var spawnMonster in _spawnMonsterList)
            spawnMonster.Spawn();
    }

    public void StopSpawnMonster()
    {
        SpawnMonster = false;

        for (int ii = 0; ii < _spawnMonsterQueues.Length; ++ii)
            _spawnMonsterQueues[ii].Clear();

        for (int ii = 0; ii < _spawnMonsters.Length; ++ii)
            _spawnMonsters[ii] = false;
    }

    private void _InitSpawnMonster<T>(string monsterType) where T : SpawnMonster, new()
    {
        if (false == _allSpawnMonsterDic.TryGetValue(monsterType, out var spawnMonster))
        {
            spawnMonster = new T();
            spawnMonster.MonsterSpawner = this;
        }
        _spawnMonsterList.Add(spawnMonster);
    }

    private void _SpawnMonster()
    {
        for (int ii = 0; ii < _spawnMonsterQueues.Length; ++ii)
        {
            if (_spawnMonsters[ii])
                continue;

            if (_spawnMonsterQueues[ii].Count <= EMPTY_VALUE)
                continue;

            var monsterType = _spawnMonsterQueues[ii].Dequeue();
            Manager.Instance.Object.GetMonster(monsterType, (normalMonsterGO) =>
            {
                var normalMonster = Utils.GetOrAddComponent<NormalMonster>(normalMonsterGO);
                normalMonster.Target = HeroController.transform;
                normalMonster.InitMonsterAbilities();

                var randomPoint = _spawnPoints[ii];
                normalMonster.transform.position = randomPoint.position;

                var repositionMonster = Utils.GetOrAddComponent<RepositionMonster>(normalMonsterGO);
                repositionMonster.HeroController = HeroController;

                Manager.Instance.Ingame.EnqueueUsedMonster(normalMonster);

                Utils.SetActive(normalMonsterGO, true);

                _ReadyToRespawn(ii).Forget();
            });
        }
    }

    private async UniTaskVoid _ReadyToRespawn(int index)
    {
        _spawnMonsters[index] = true;
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_RESPAWN_TIME));

        if (_spawnMonsters[index])
            _spawnMonsters[index] = false;
    }
}
