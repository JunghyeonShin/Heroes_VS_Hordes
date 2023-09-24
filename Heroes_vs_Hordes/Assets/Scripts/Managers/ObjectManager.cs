using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObjectManager
{
    private GameObject _rootObject;
    private bool[] _loadCompletes;

    private Dictionary<string, GameObject> _mapDic = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _heroDic = new Dictionary<string, GameObject>();
    private Dictionary<string, ObjectPool> _dropItemDic = new Dictionary<string, ObjectPool>();
    private Dictionary<string, ObjectPool> _monsterPoolDic = new Dictionary<string, ObjectPool>();
    private Dictionary<string, GameObject> _weaponControllerDic = new Dictionary<string, GameObject>();
    private ObjectPool _damageTextPool = new ObjectPool();

    public GameObject RepositionArea { get; private set; }
    public GameObject MonsterSpawner { get; private set; }
    public GameObject LevelUpText { get; private set; }

    private const int INDEX_TOTAL_VALUE = 10;
    private const int INDEX_REPOSITION_AREA = 0;
    private const int INDEX_MONSTER_SPAWNER = 1;
    private const int INDEX_LEVEL_UP_TEXT = 2;
    private const int INDEX_DAMAGE_TEXT = 3;
    private const int INDEX_EXP_GEM = 4;
    private const int INDEX_GOLD = 5;
    private const int INDEX_MONSTER_NORMAL_BAT = 6;
    private const int INDEX_WEAPON_BOMB_CONTROLLER = 7;
    private const int INDEX_WEAPON_BOOMERNAG_CONTROLLER = 8;
    private const int INDEX_WEAPON_CROSSBOW_CONTROLLER = 9;
    private const string NAME_ROOT_OBJECT = "[ROOT_OBJECT]";

    public void Init()
    {
        _rootObject = new GameObject(NAME_ROOT_OBJECT);

        _loadCompletes = new bool[INDEX_TOTAL_VALUE];
        Manager.Instance.Resource.Instantiate(Define.RESOURCE_REPOSITION_AREA, _rootObject.transform, (repositionArea) =>
        {
            RepositionArea = repositionArea;
            Utils.SetActive(RepositionArea, false);
            _loadCompletes[INDEX_REPOSITION_AREA] = true;
        });

        Manager.Instance.Resource.Instantiate(Define.RESOURCE_MONSTER_SPAWNER, _rootObject.transform, (monsterSpawner) =>
        {
            MonsterSpawner = monsterSpawner;
            Utils.SetActive(MonsterSpawner, false);
            _loadCompletes[INDEX_MONSTER_SPAWNER] = true;
        });

        Manager.Instance.Resource.Instantiate(Define.RESROUCE_LEVEL_UP_TEXT, _rootObject.transform, (levelUpText) =>
        {
            LevelUpText = levelUpText;
            Utils.SetActive(LevelUpText, false);
            _loadCompletes[INDEX_LEVEL_UP_TEXT] = true;
        });

        Manager.Instance.Resource.LoadAsync<GameObject>(Define.RESROUCE_DAMAGE_TEXT, (damageText) =>
        {
            _damageTextPool.InitPool(damageText, _rootObject, DEFAULT_INSTANTIATE_DAMAGE_TEXT_COUNT);
            _loadCompletes[INDEX_DAMAGE_TEXT] = true;
        });

        _InitDropItem();
        _InitMonster();
        _InitWeaponController();
    }

    public bool LoadComplete()
    {
        for (int ii = 0; ii < _loadCompletes.Length; ++ii)
        {
            if (false == _loadCompletes[ii])
                return false;
        }
        return true;
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
    private const int DEFAULT_INSTANTIATE_MONSTER_COUNT = 50;

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

    private void _InitMonster()
    {
        _InitMonster(Define.RESOURCE_MONSTER_NORMAL_BAT, DEFAULT_INSTANTIATE_MONSTER_COUNT, (monster) => { _loadCompletes[INDEX_MONSTER_NORMAL_BAT] = true; });
    }

    private void _InitMonster(string key, int count, Action<GameObject> callback)
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
    private const int DEFAULT_INSTANTIATE_DAMAGE_TEXT_COUNT = 50;

    public GameObject GetDamageText()
    {
        return _damageTextPool.GetObject();
    }

    public void ReturnDamageText(GameObject damageText)
    {
        _damageTextPool.ReturnObject(damageText);
    }
    #endregion

    #region DropItem
    private const int DEFAULT_INSTANTIATE_DROP_ITEM_COUNT = 50;

    public void GetDropItem(string key, Action<GameObject> callback)
    {
        // 캐시 확인
        if (_dropItemDic.TryGetValue(key, out var dropItemPool))
        {
            callback?.Invoke(dropItemPool.GetObject());
            return;
        }

        // 드랍 아이템 오브젝트 생성 후 캐싱
        _InitDropItem(key, DEFAULT_INSTANTIATE_DROP_ITEM_COUNT, callback);
    }

    public void ReturnDropItem(string key, GameObject monster)
    {
        _dropItemDic[key].ReturnObject(monster);
    }

    private void _InitDropItem()
    {
        _InitDropItem(Define.RESOURCE_EXP_GEM, DEFAULT_INSTANTIATE_DROP_ITEM_COUNT, (expGem) => { _loadCompletes[INDEX_EXP_GEM] = true; });

        _InitDropItem(Define.RESOURCE_GOLD, DEFAULT_INSTANTIATE_DROP_ITEM_COUNT, (expGem) => { _loadCompletes[INDEX_GOLD] = true; });
    }

    private void _InitDropItem(string key, int count, Action<GameObject> callback = null)
    {
        var objectPool = new ObjectPool();

        Manager.Instance.Resource.LoadAsync<GameObject>(key, (dropItem) =>
        {
            objectPool.InitPool(dropItem, _rootObject, count);
            _dropItemDic.Add(key, objectPool);
            callback?.Invoke(objectPool.GetObject());
        });
    }
    #endregion

    #region WeaponController
    public void GetWeaponController(string key, Action<GameObject> callback)
    {
        // 캐시 확인
        if (_weaponControllerDic.TryGetValue(key, out var weaponController))
        {
            callback?.Invoke(weaponController);
            return;
        }

        // 무기 컨트롤러 오브젝트 생성 후 캐싱
        _InitWeaponController(key, callback);
    }

    public void ReturnWeaponController(string key)
    {
        Utils.SetActive(_weaponControllerDic[key], false);
    }

    private void _InitWeaponController()
    {
        _InitWeaponController(Define.RESOURCE_WEAPON_BOMB_CONTROLLER, (bombController) => { _loadCompletes[INDEX_WEAPON_BOMB_CONTROLLER] = true; });
        _InitWeaponController(Define.RESOURCE_WEAPON_BOOMERANG_CONTROLLER, (boomerangController) => { _loadCompletes[INDEX_WEAPON_BOOMERNAG_CONTROLLER] = true; });
        _InitWeaponController(Define.RESOURCE_WEAPON_CROSSBOW_CONTROLLER, (crossbowController) => { _loadCompletes[INDEX_WEAPON_CROSSBOW_CONTROLLER] = true; });
    }

    private void _InitWeaponController(string key, Action<GameObject> callback = null)
    {
        Manager.Instance.Resource.Instantiate(key, _rootObject.transform, (weaponController) =>
        {
            _weaponControllerDic.Add(key, weaponController);
            Utils.SetActive(weaponController, false);
            callback?.Invoke(weaponController);
        });
    }
    #endregion
}
