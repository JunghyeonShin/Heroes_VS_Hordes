using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    private Dictionary<string, GameObject> _mapDic = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _heroDic = new Dictionary<string, GameObject>();

    private GameObject _root;

    private const string NAME_ROOT_OBJECT = "[ROOT_OBJECT]";

    public void Init()
    {
        _root = new GameObject(NAME_ROOT_OBJECT);
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
        Manager.Instance.Resource.Instantiate(key, _root.transform, (map) =>
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
        Manager.Instance.Resource.Instantiate(key, _root.transform, (hero) =>
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
}
