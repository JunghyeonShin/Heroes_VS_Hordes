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
    private ObjectPool _damageTextPool = new ObjectPool();
    private ObjectPool _experienceGemPool = new ObjectPool();

    private GameObject _rootObject;

    public GameObject RepositionArea { get; private set; }
    public GameObject MonsterSpawner { get; private set; }

    private const int DEFAULT_INSTANTIATE_MONSTER_COUNT = 50;
    private const int DEFAULT_INSTANTIATE_DAMAGE_TEXT_COUNT = 50;
    private const int DEFAULT_INSTANTIATE_EXPERIENCE_GEM_COUNT = 50;
    private const string NAME_ROOT_OBJECT = "[ROOT_OBJECT]";

    public void Init()
    {
        _rootObject = new GameObject(NAME_ROOT_OBJECT);

        Manager.Instance.Resource.Instantiate(Define.RESOURCE_REPOSITION_AREA, _rootObject.transform, (repositionArea) =>
        {
            RepositionArea = repositionArea;
            Utils.SetActive(RepositionArea, false);
        });

        Manager.Instance.Resource.Instantiate(Define.RESOURCE_MONSTER_SPAWNER, _rootObject.transform, (monsterSpawner) =>
        {
            MonsterSpawner = monsterSpawner;
            Utils.SetActive(MonsterSpawner, false);
        });

        _InitMonster(Define.RESOURCE_MONSTER_NORMAL_BAT, DEFAULT_INSTANTIATE_MONSTER_COUNT);

        Manager.Instance.Resource.LoadAsync<GameObject>(Define.RESROUCE_DAMAGE_TEXT, (damageText) =>
        {
            _damageTextPool.InitPool(damageText, _rootObject, DEFAULT_INSTANTIATE_DAMAGE_TEXT_COUNT);
        });

        Manager.Instance.Resource.LoadAsync<GameObject>(Define.RESOURCE_EXPERIENCE_GEM, (damageText) =>
        {
            _experienceGemPool.InitPool(damageText, _rootObject, DEFAULT_INSTANTIATE_EXPERIENCE_GEM_COUNT);
        });
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
        {
            callback?.Invoke(monsterPool.GetObject());
            return;
        }

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

    #region DamageText
    public GameObject GetDamageText()
    {
        return _damageTextPool.GetObject();
    }

    public void ReturnDamageText(GameObject damageText)
    {
        _damageTextPool.ReturnObject(damageText);
    }
    #endregion

    #region ExperienceGem
    public GameObject GetExperienceGem()
    {
        return _experienceGemPool.GetObject();
    }

    public void ReturnExperienceGem(GameObject experienceGem)
    {
        _experienceGemPool.ReturnObject(experienceGem);
    }
    #endregion
}
