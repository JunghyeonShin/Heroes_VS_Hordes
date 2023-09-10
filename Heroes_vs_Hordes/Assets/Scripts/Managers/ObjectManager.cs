using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObjectManager
{
    private Dictionary<string, GameObject> _mapDic = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _heroDic = new Dictionary<string, GameObject>();
    private Dictionary<string, ObjectPool> _monsterPoolDic = new Dictionary<string, ObjectPool>();

    private GameObject _rootObject;

    public GameObject MapCollisionArea { get; private set; }

    private const int DEFAULT_INSTANTIATE_MONSTER_COUNT = 50;
    private const string NAME_ROOT_OBJECT = "[ROOT_OBJECT]";

    public void Init()
    {
        _rootObject = new GameObject(NAME_ROOT_OBJECT);

        Manager.Instance.Resource.Instantiate(Define.RESOURCE_MAP_COLLISION_AREA, _rootObject.transform, (mapCollisionArea) =>
        {
            MapCollisionArea = mapCollisionArea;
            Utils.SetActive(MapCollisionArea, false);
        });
        _InitMonster(Define.RESOURCE_MONSTER_NORMAL_BAT, DEFAULT_INSTANTIATE_MONSTER_COUNT);
    }

    #region Map
    public void GetMap(string key, Action<GameObject> callback)
    {
        // 캐시 확인
        if (_mapDic.TryGetValue(key, out var map))
        {
            callback?.Invoke(map);
            return;
        }

        // 맵 오브젝트 생성 후 캐싱
        Manager.Instance.Resource.Instantiate(key, _rootObject.transform, (map) =>
        {
            _mapDic.Add(key, map);
            callback?.Invoke(map);
        });
    }

    public void ReturnMap(string key)
    {
        Utils.SetActive(_mapDic[key], false);
    }
    #endregion

    #region Hero
    public void GetHero(string key, Action<GameObject> callback)
    {
        // 캐시 확인
        if (_heroDic.TryGetValue(key, out var hero))
        {
            callback?.Invoke(hero);
            return;
        }

        // 영웅 오브젝트 생성 후 캐싱
        Manager.Instance.Resource.Instantiate(key, _rootObject.transform, (hero) =>
        {
            _heroDic.Add(key, hero);
            callback?.Invoke(hero);
        });
    }

    public void ReturnHero(string key)
    {
        Utils.SetActive(_heroDic[key], false);
    }
    #endregion

    #region Monster
    public void GetMonster(string key, Action<GameObject> callback)
    {
        // 캐시 확인
        if (_monsterPoolDic.TryGetValue(key, out var monsterPool))
            callback?.Invoke(monsterPool.GetObject());

        // 몬스터 오브젝트 생성 후 캐싱
        _InitMonster(key, DEFAULT_INSTANTIATE_MONSTER_COUNT, callback);
    }

    public void ReturnMonster(string key, GameObject monster)
    {
        _monsterPoolDic[key].ReturnObject(monster);
    }

    private void _InitMonster(string key, int count, Action<GameObject> callback = null)
    {
        var objectPool = new ObjectPool();

        Manager.Instance.Resource.LoadAsync<GameObject>(key, (monster) =>
        {
            objectPool.InitPool(monster, _rootObject, count);
            _monsterPoolDic.Add(key, objectPool);
            callback?.Invoke(objectPool.GetObject());
        });
    }
    #endregion
}
