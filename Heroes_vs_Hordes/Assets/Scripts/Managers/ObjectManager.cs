using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObjectManager
{
    private bool[] _loadCompletes;

    private Dictionary<string, GameObject> _mapDic = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _heroDic = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _bossMapDic = new Dictionary<string, GameObject>();
    private Dictionary<string, ObjectPool> _dropItemDic = new Dictionary<string, ObjectPool>();
    private Dictionary<string, GameObject> _weaponControllerDic = new Dictionary<string, GameObject>();
    private Dictionary<string, ObjectPool> _monsterPoolDic = new Dictionary<string, ObjectPool>();
    private Dictionary<string, GameObject> _bossMonsterDic = new Dictionary<string, GameObject>();
    private ObjectPool _damageTextPool = new ObjectPool();

    public GameObject RootObject { get; private set; }
    public GameObject HeroHealth { get; private set; }
    public GameObject RepositionArea { get; private set; }
    public GameObject MonsterSpawner { get; private set; }
    public GameObject LevelUpText { get; private set; }
    public GameObject BossMapCameraFollower { get; private set; }

    private const int INDEX_TOTAL_VALUE = 23;
    private const int INDEX_HERO_HEALTH = 0;
    private const int INDEX_REPOSITION_AREA = 1;
    private const int INDEX_MONSTER_SPAWNER = 2;
    private const int INDEX_LEVEL_UP_TEXT = 3;
    private const int INDEX_BOSS_MAP_CAMERA_FOLLOWER = 4;
    private const int INDEX_DAMAGE_TEXT = 5;
    private const int INDEX_EXP_GEM = 6;
    private const int INDEX_GOLD = 7;
    private const int INDEX_WEAPON_BOMB_CONTROLLER = 8;
    private const int INDEX_WEAPON_BOOMERNAG_CONTROLLER = 9;
    private const int INDEX_WEAPON_CROSSBOW_CONTROLLER = 10;
    private const int INDEX_WEAPON_DIVINE_AURA_CONTROLLER = 11;
    private const int INDEX_WEAPON_FIREBALL_CONTROLLER = 12;
    private const int INDEX_MONSTER_NORMAL_BAT = 13;
    private const int INDEX_MONSTER_SWARM_BAT = 14;
    private const int INDEX_MONSTER_NORMAL_GOBLIN = 15;
    private const int INDEX_MONSTER_CLUB_GOBLIN = 16;
    private const int INDEX_MONSTER_ARMOR_GOBLIN = 17;
    private const int INDEX_MONSTER_NORMAL_SKELETON = 18;
    private const int INDEX_MONSTER_ARMOR_SKELETON = 19;
    private const int INDEX_MONSTER_NORMAL_SPIDER = 20;
    private const int INDEX_MONSTER_CAVE_SPIDER = 21;
    private const int INDEX_MONSTER_BOSS_SPIDER = 22;
    private const string NAME_ROOT_OBJECT = "[ROOT_OBJECT]";

    public void Init()
    {
        RootObject = new GameObject(NAME_ROOT_OBJECT);

        _loadCompletes = new bool[INDEX_TOTAL_VALUE];

        Manager.Instance.Resource.Instantiate(Define.RESOURCE_HERO_HEALTH, RootObject.transform, (heroHealth) =>
        {
            HeroHealth = heroHealth;
            Utils.SetActive(heroHealth, false);
            _loadCompletes[INDEX_HERO_HEALTH] = true;
        });

        Manager.Instance.Resource.Instantiate(Define.RESOURCE_REPOSITION_AREA, RootObject.transform, (repositionArea) =>
        {
            RepositionArea = repositionArea;
            Utils.SetActive(RepositionArea, false);
            _loadCompletes[INDEX_REPOSITION_AREA] = true;
        });

        Manager.Instance.Resource.Instantiate(Define.RESOURCE_MONSTER_SPAWNER, RootObject.transform, (monsterSpawner) =>
        {
            MonsterSpawner = monsterSpawner;
            Utils.SetActive(MonsterSpawner, false);
            _loadCompletes[INDEX_MONSTER_SPAWNER] = true;
        });

        Manager.Instance.Resource.Instantiate(Define.RESROUCE_LEVEL_UP_TEXT, RootObject.transform, (levelUpText) =>
        {
            LevelUpText = levelUpText;
            Utils.SetActive(LevelUpText, false);
            _loadCompletes[INDEX_LEVEL_UP_TEXT] = true;
        });

        Manager.Instance.Resource.Instantiate(Define.RESOURCE_BOSS_MAP_CAMERA_FOLLOWER, RootObject.transform, (bossMapCameraFollower) =>
        {
            BossMapCameraFollower = bossMapCameraFollower;
            Utils.SetActive(BossMapCameraFollower, false);
            _loadCompletes[INDEX_BOSS_MAP_CAMERA_FOLLOWER] = true;
        });

        Manager.Instance.Resource.LoadAsync<GameObject>(Define.RESROUCE_DAMAGE_TEXT, (damageText) =>
        {
            _damageTextPool.InitPool(damageText, RootObject, DEFAULT_INSTANTIATE_DAMAGE_TEXT_COUNT);
            _loadCompletes[INDEX_DAMAGE_TEXT] = true;
        });

        _InitDropItem();
        _InitWeaponController();
        _InitMonster();
        _InitBossMonster();
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
        Manager.Instance.Resource.Instantiate(key, RootObject.transform, (map) =>
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
        Manager.Instance.Resource.Instantiate(key, RootObject.transform, (hero) =>
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

    #region Boss Map
    public void GetBossMap(string key, Action<GameObject> callback)
    {
        // 캐시 확인
        if (_bossMapDic.TryGetValue(key, out var map))
        {
            callback?.Invoke(map);
            return;
        }

        // 보스 맵 오브젝트 생성 후 캐싱
        Manager.Instance.Resource.Instantiate(key, RootObject.transform, (map) =>
        {
            _bossMapDic.Add(key, map);
            callback?.Invoke(map);
        });
    }

    public void ReturnBossMap(string key)
    {
        Utils.SetActive(_bossMapDic[key], false);
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
            objectPool.InitPool(dropItem, RootObject, count);
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
        _InitWeaponController(Define.RESOURCE_WEAPON_DIVINE_AURA_CONTROLLER, (crossbowController) => { _loadCompletes[INDEX_WEAPON_DIVINE_AURA_CONTROLLER] = true; });
        _InitWeaponController(Define.RESOURCE_WEAPON_FIREBALL_CONTROLLER, (crossbowController) => { _loadCompletes[INDEX_WEAPON_FIREBALL_CONTROLLER] = true; });
    }

    private void _InitWeaponController(string key, Action<GameObject> callback = null)
    {
        Manager.Instance.Resource.Instantiate(key, RootObject.transform, (weaponController) =>
        {
            _weaponControllerDic.Add(key, weaponController);
            Utils.SetActive(weaponController, false);
            callback?.Invoke(weaponController);
        });
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
        _InitMonster(Define.RESOURCE_MONSTER_SWARM_BAT, DEFAULT_INSTANTIATE_MONSTER_COUNT, (monster) => { _loadCompletes[INDEX_MONSTER_SWARM_BAT] = true; });
        _InitMonster(Define.RESOURCE_MONSTER_NORMAL_GOBLIN, DEFAULT_INSTANTIATE_MONSTER_COUNT, (monster) => { _loadCompletes[INDEX_MONSTER_NORMAL_GOBLIN] = true; });
        _InitMonster(Define.RESOURCE_MONSTER_CLUB_GOBLIN, DEFAULT_INSTANTIATE_MONSTER_COUNT, (monster) => { _loadCompletes[INDEX_MONSTER_CLUB_GOBLIN] = true; });
        _InitMonster(Define.RESOURCE_MONSTER_ARMOR_GOBLIN, DEFAULT_INSTANTIATE_MONSTER_COUNT, (monster) => { _loadCompletes[INDEX_MONSTER_ARMOR_GOBLIN] = true; });
        _InitMonster(Define.RESOURCE_MONSTER_NORMAL_SKELETON, DEFAULT_INSTANTIATE_MONSTER_COUNT, (monster) => { _loadCompletes[INDEX_MONSTER_NORMAL_SKELETON] = true; });
        _InitMonster(Define.RESOURCE_MONSTER_ARMOR_SKELETON, DEFAULT_INSTANTIATE_MONSTER_COUNT, (monster) => { _loadCompletes[INDEX_MONSTER_ARMOR_SKELETON] = true; });
        _InitMonster(Define.RESOURCE_MONSTER_NORMAL_SPIDER, DEFAULT_INSTANTIATE_MONSTER_COUNT, (monster) => { _loadCompletes[INDEX_MONSTER_NORMAL_SPIDER] = true; });
        _InitMonster(Define.RESOURCE_MONSTER_CAVE_SPIDER, DEFAULT_INSTANTIATE_MONSTER_COUNT, (monster) => { _loadCompletes[INDEX_MONSTER_CAVE_SPIDER] = true; });
    }

    private void _InitMonster(string key, int count, Action<GameObject> callback)
    {
        var objectPool = new ObjectPool();

        Manager.Instance.Resource.LoadAsync<GameObject>(key, (monster) =>
        {
            objectPool.InitPool(monster, RootObject, count);
            _monsterPoolDic.Add(key, objectPool);
            callback?.Invoke(objectPool.GetObject());
        });
    }
    #endregion

    #region BossMonster
    public void GetBossMonster(string key, Action<GameObject> callback)
    {
        // 캐시 확인
        if (_bossMonsterDic.TryGetValue(key, out var bossMonster))
        {
            callback?.Invoke(bossMonster);
            return;
        }

        // 보스 몬스터 오브젝트 생성 후 캐싱
        _InitBossMonster(key, callback);
    }

    public void ReturnBossMonster(string key)
    {
        Utils.SetActive(_bossMonsterDic[key], false);
    }

    private void _InitBossMonster()
    {
        _InitBossMonster(Define.RESOURCE_MONSTER_BOSS_SPIDER, (bossMonster) => { _loadCompletes[INDEX_MONSTER_BOSS_SPIDER] = true; });
    }

    private void _InitBossMonster(string key, Action<GameObject> callback)
    {
        Manager.Instance.Resource.Instantiate(key, RootObject.transform, (bossMonster) =>
        {
            _bossMonsterDic.Add(key, bossMonster);
            Utils.SetActive(bossMonster, false);
            callback?.Invoke(bossMonster);
        });
    }
    #endregion
}
