using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    private void Awake()
    {
        _InitWaves();
    }

    #region Ingame
    public event Action<float> ChangeProgressTimeHandler;
    public event Action<int> ChangeModeHandler;
    public event Action<string> ShowWavePanelHandler;

    private List<Wave> _waveList = new List<Wave>();

    public Wave CurrentWave { get; private set; }
    public int CurrentChapterIndex { get; private set; }
    public int CurrentWaveIndex { get; private set; }
    public int TotalWaveIndex { get; private set; }
    public int ClearChapterReward { get; set; }
    public int ClearWaveReward { get; set; }
    public bool DefeatIngame { get; set; }
    public bool ExitIngameForce { get; set; }

    private const float PAUSE_INGAME = 0f;
    private const float RESTART_INGAME = 1f;
    private const float RESTORE_TIMESCALE = 1f;
    private const int INIT_WAVE_INDEX = 0;
    private const int NEXT_WAVE_INDEX = 1;
    private const int INIT_REWARD = 0;
    private const string NAME_WAVE = "[WAVE]";

    /// <summary>
    /// 인게임을 처음 진입하여 초기 세팅할 때 호출
    /// </summary>
    public void InitIngame(int chapterIndex, UI_Loading loadingUI)
    {
        HeroLevelUpCount = Define.INIT_HERO_LEVEL_UP_COUNT;

        CurrentChapterIndex = chapterIndex;
        CurrentWaveIndex = INIT_WAVE_INDEX;
        TotalWaveIndex = Manager.Instance.Data.ChapterInfoDataList[CurrentChapterIndex].TotalWaveIndex;

        ClearChapterReward = INIT_REWARD;
        ClearWaveReward = INIT_REWARD;

        DefeatIngame = false;
        ExitIngameForce = false;

        _remainingMonsterCount = INIT_REMAINIG_MONSTER_COUNT;
        _remainingExp = INIT_REMAINING_EXP;

        AcquiredGold = INIT_REWARD;
        _remainingGold = INIT_REMAINING_GOLD;

        _InitWeaponController();

        _ownedAbilityInfoDic.Clear();
        _ownedWeaponList.Clear();
        _ownedBookList.Clear();

        // UI_PauseIngame 초기 세팅
        var pauseIngameUI = Manager.Instance.UI.FindUI<UI_PauseIngame>(Define.RESOURCE_UI_PAUSE_INGAME);
        pauseIngameUI.InitWavePanel();
        pauseIngameUI.InitAbilityUI();

        // UI_ClearWave 초기 세팅
        var clearWaveUI = Manager.Instance.UI.FindUI<UI_ClearWave>(Define.RESOURCE_UI_CLEAR_WAVE);
        clearWaveUI.InitWavePanel();

        // UI_LevelUpHero 초기 세팅
        var levelUpHeroUI = Manager.Instance.UI.FindUI<UI_LevelUpHero>(Define.RESOURCE_UI_LEVEL_UP_HERO);
        levelUpHeroUI.InitSelectAbilityPanel();
        levelUpHeroUI.InitAbilityUI();

        // 맵 생성
        Manager.Instance.Object.GetMap(Manager.Instance.Data.ChapterInfoDataList[CurrentChapterIndex].MapType, (mapGO) =>
        {
            Utils.SetActive(mapGO, true);

            // 영웅 생성
            Manager.Instance.Object.GetHero(Define.RESOURCE_HERO_ARCANE_MAGE, (heroGO) =>
            {
                var hero = heroGO.GetComponent<Hero>();
                UsedHero = hero;
                // 보유한 무기 세팅
                RegistAbility(UsedHero.HeroWeaponName);
                UsedHero.InitHeroAbilities();

                var heroController = Utils.GetOrAddComponent<HeroController>(UsedHero.gameObject);

                var mapController = Utils.GetOrAddComponent<MapController>(mapGO);
                mapController.SetHeroController(heroController);

                var heroHealthGO = Manager.Instance.Object.HeroHealth;
                var heroHealth = Utils.GetOrAddComponent<HeroHealth>(heroHealthGO);
                heroHealth.HeroTransform = UsedHero.transform;
                UsedHero.ChangeHealthHandler -= heroHealth.ChangeHealthValue;
                UsedHero.ChangeHealthHandler += heroHealth.ChangeHealthValue;
                Utils.SetActive(heroHealthGO, true);

                {
                    var repositionAreaGO = Manager.Instance.Object.RepositionArea;
                    var chaseHero = Utils.GetOrAddComponent<ChaseHero>(repositionAreaGO);
                    chaseHero.HeroTransform = UsedHero.transform;
                    Utils.SetActive(repositionAreaGO, true);
                }

                {
                    var monsterSpawnerGO = Manager.Instance.Object.MonsterSpawner;
                    var chaseHero = Utils.GetOrAddComponent<ChaseHero>(monsterSpawnerGO);
                    chaseHero.HeroTransform = UsedHero.transform;
                    _monsterSpawner = Utils.GetOrAddComponent<MonsterSpawner>(monsterSpawnerGO);
                    _monsterSpawner.HeroController = heroController;
                    _monsterSpawner.InitSpawnMonster();
                    Utils.SetActive(monsterSpawnerGO, true);
                }

                // 카메라 팔로워 세팅
                Manager.Instance.CameraController.SetFollower(UsedHero.transform);

                Utils.SetActive(UsedHero.gameObject, true);

                loadingUI.CompleteLoading();
            });
        });
    }

    /// <summary>
    /// 인게임을 처음 실행하거나 웨이브를 클리어하고 다음 웨이브를 실행할 때 호출
    /// </summary>
    public void StartIngame()
    {
        Utils.SetTimeScale(RESTORE_TIMESCALE);
        _remainingMonsterCount = INIT_REMAINIG_MONSTER_COUNT;
        CurrentWave = _waveList[Manager.Instance.Data.ChapterInfoDataList[CurrentChapterIndex].WaveIndex[CurrentWaveIndex]];
        CurrentWave.StartWave();
    }

    /// <summary>
    /// 인게임을 멈추거나 재시작할 때 호출
    /// </summary>
    /// <param name="control">인게임 조절로 false면 멈춤, true면 재시작</param>
    public void ControlIngame(bool control)
    {
        CurrentWave.ControlWave(control);
        if (control)
            Utils.SetTimeScale(RESTART_INGAME);
        else
            Utils.SetTimeScale(PAUSE_INGAME);
    }

    /// <summary>
    /// 현재 웨이브를 클리어 했을 때 호출
    /// </summary>
    public void ClearIngame()
    {
        CurrentWave.ClearWave();
    }

    /// <summary>
    /// 현재 웨이브의 클리어 후처리를 할 때 호출
    /// </summary>
    public void ClearIngamePostProcessing()
    {
        Utils.SetTimeScale(PAUSE_INGAME);
        CurrentWaveIndex += NEXT_WAVE_INDEX;
    }

    /// <summary>
    /// 인게임을 중간에 포기하거나 클리어하고 나갈 때 호출
    /// </summary>
    public void ExitIngame()
    {
        Utils.SetTimeScale(RESTORE_TIMESCALE);
        CurrentWave.ExitWave();
        StopSpawnMonster();
        ReturnUsedMonster();
        ReturnUsedExpGem();
        ReturnUsedGold();
        _ReturnHeroAbility();
        _ReturnWeaponController();
        Manager.Instance.CameraController.SetFollower();
        Utils.SetActive(Manager.Instance.Object.MonsterSpawner, false);
        Utils.SetActive(Manager.Instance.Object.RepositionArea, false);
        Utils.SetActive(Manager.Instance.Object.HeroHealth, false);
        Manager.Instance.Object.ReturnHero(Define.RESOURCE_HERO_ARCANE_MAGE);
        Manager.Instance.Object.ReturnMap(Manager.Instance.Data.ChapterInfoDataList[CurrentChapterIndex].MapType);

        if (false == ExitIngameForce)
            Manager.Instance.SaveData.OwnedGold += (ClearChapterReward + ClearWaveReward + AcquiredGold);

        Manager.Instance.UI.ShowSceneUI<UI_MainScene>(Define.RESOURCE_UI_MAIN_SCENE, (mainsceneUI) =>
        {
            if (false == DefeatIngame && false == ExitIngameForce)
                mainsceneUI.SetChapter(CurrentChapterIndex + Define.ADJUSE_CHAPTER_INDEX);
        });
    }

    public void ShowWavePanel(string wavePanelText)
    {
        ShowWavePanelHandler?.Invoke(wavePanelText);
    }

    public void ChangeProgressWaveTime(float time)
    {
        ChangeProgressTimeHandler?.Invoke(time);
    }

    public void ChangeMode(int mode)
    {
        ChangeModeHandler?.Invoke(mode);
    }

    private void _InitWaves()
    {
        var wave = new GameObject(NAME_WAVE);
        wave.transform.SetParent(transform);
        var normalBattleWave = Utils.GetOrAddComponent<NormalBattleWave>(wave);
        _waveList.Add(normalBattleWave);
        var goldRushWave = Utils.GetOrAddComponent<GoldRushWave>(wave);
        _waveList.Add(goldRushWave);
        var bossBattleWave = Utils.GetOrAddComponent<BossBattleWave>(wave);
        _waveList.Add(bossBattleWave);
    }
    #endregion

    #region Hero
    public event Action<float> ChangeHeroExpHandler;
    public event Action<int> ChangeHeroLevelHandler;
    public event Action ChangeHeroLevelUpPostProcessingHandler;
    public event Action LevelUpHeroAbilityHandler;

    public Hero UsedHero { get; private set; }
    public int HeroLevelUpCount { get; set; }

    private const int INIT_HERO_LEVEL = 1;

    public void ChangeHeroExp(float value)
    {
        ChangeHeroExpHandler?.Invoke(value);
    }

    public void ChangeHeroLevel(int level)
    {
        if (level > INIT_HERO_LEVEL)
            ++HeroLevelUpCount;
        ChangeHeroLevelHandler?.Invoke(level);
    }

    public void LevelUpHeroAbility()
    {
        if (HeroLevelUpCount > Define.INIT_HERO_LEVEL_UP_COUNT)
        {
            _DrawAbility();
            LevelUpHeroAbilityHandler?.Invoke();
        }
        else
            ChangeHeroLevelUpPostProcessingHandler?.Invoke();
    }

    private void _ReturnHeroAbility()
    {
        UsedHero.ReturnAbilities();
    }
    #endregion

    #region Monster
    public event Action RemainingMonsterHandler;
    public event Action<float> ChangeBossMonsterHealthHandler;

    private Queue<NormalMonster> _usedMonsterQueue = new Queue<NormalMonster>();

    private MonsterSpawner _monsterSpawner;
    private int _remainingMonsterCount;

    public int RemainingMonsterCount { get { return _remainingMonsterCount; } }

    private const int INIT_REMAINIG_MONSTER_COUNT = 0;
    private const int EMPTY_USED_MONSTER = 0;

    public void StartSpawnMonster()
    {
        _monsterSpawner.StartSpawnMonster();
    }

    public void StopSpawnMonster()
    {
        _monsterSpawner.StopSpawnMonster();
        var waveIndex = Manager.Instance.Data.ChapterInfoDataList[CurrentChapterIndex].WaveIndex[CurrentWaveIndex];
        if (Define.INDEX_GOLD_RUSH_WAVE != waveIndex)
            RemainingMonsterHandler?.Invoke();
    }

    public void OnDeadMonster()
    {
        --_remainingMonsterCount;
        RemainingMonsterHandler?.Invoke();
    }

    public void SetBossMonsterHealth(float value)
    {
        ChangeBossMonsterHealthHandler?.Invoke(value);
    }

    public void EnqueueUsedMonster(NormalMonster normalMmonster)
    {
        ++_remainingMonsterCount;
        _usedMonsterQueue.Enqueue(normalMmonster);
    }

    public void ReturnUsedMonster()
    {
        while (_usedMonsterQueue.Count > EMPTY_USED_MONSTER)
        {
            var normalMonster = _usedMonsterQueue.Dequeue();
            if (false == normalMonster.gameObject.activeSelf)
                continue;

            normalMonster.ReturnMonster();
        }
    }
    #endregion

    #region ExpGem
    private Queue<ExpGem> _usedExpGemQueue = new Queue<ExpGem>();

    private float _remainingExp;

    private const float INIT_REMAINING_EXP = 0f;
    private const int EMPTY_USED_EXP_GEM = 0;

    public void EnqueueUsedExpGem(ExpGem expGem)
    {
        _usedExpGemQueue.Enqueue(expGem);
    }

    public void ReturnUsedExpGem()
    {
        while (_usedExpGemQueue.Count > EMPTY_USED_EXP_GEM)
        {
            var expGem = _usedExpGemQueue.Dequeue();
            if (false == expGem.gameObject.activeSelf)
                continue;

            if (DefeatIngame || ExitIngameForce)
                expGem.ReturnDropItem();
            else
            {
                expGem.GiveEffect(UsedHero, false);
                _remainingExp += Define.INCREASE_HERO_EXP_VALUE;
            }
        }
    }

    public void GetExpAtOnce()
    {
        UsedHero.GetExp(_remainingExp);
        _remainingExp = INIT_REMAINING_EXP;
    }
    #endregion

    #region Gold
    public event Action ChangeGoldHandler;

    private Queue<Gold> _usedGoldQueue = new Queue<Gold>();

    private int _remainingGold;

    public int AcquiredGold { get; private set; }

    private const int INIT_REMAINING_GOLD = 0;

    public void EnqueueUsedGold(Gold gold)
    {
        _usedGoldQueue.Enqueue(gold);
    }

    public void ReturnUsedGold()
    {
        while (_usedGoldQueue.Count > INIT_REMAINING_GOLD)
        {
            var gold = _usedGoldQueue.Dequeue();
            if (false == gold.gameObject.activeSelf)
                continue;

            if (DefeatIngame || ExitIngameForce)
                gold.ReturnDropItem();
            else
            {
                gold.GiveEffect(UsedHero, false);
                _remainingGold += Define.INCREASE_GOLD_VALUE;
            }
        }
    }

    public void ChangeGold()
    {
        ChangeGoldHandler?.Invoke();
    }

    public void GetGold(int gold)
    {
        AcquiredGold += gold;
        ChangeGold();
    }

    public void GetGoldAtOnce()
    {
        AcquiredGold += _remainingGold;
        _remainingGold = INIT_REMAINING_GOLD;
        ChangeGold();
    }
    #endregion

    #region WeaponController
    private Dictionary<string, WeaponController> _usedWeaponControllerDic = new Dictionary<string, WeaponController>();

    private void _InitWeaponController()
    {
        _usedWeaponControllerDic.Clear();

        _InitWaeponController(Define.RESOURCE_WEAPON_BOMB_CONTROLLER);
        _InitWaeponController(Define.RESOURCE_WEAPON_BOOMERANG_CONTROLLER);
        _InitWaeponController(Define.RESOURCE_WEAPON_CROSSBOW_CONTROLLER);
        _InitWaeponController(Define.RESOURCE_WEAPON_DIVINE_AURA_CONTROLLER);
        _InitWaeponController(Define.RESOURCE_WEAPON_FIREBALL_CONTROLLER);
    }

    private void _InitWaeponController(string key)
    {
        Manager.Instance.Object.GetWeaponController(key, (weaponControllerGO) =>
        {
            var weaponController = Utils.GetOrAddComponent<WeaponController>(weaponControllerGO);
            _usedWeaponControllerDic.Add(key, weaponController);
        });
    }

    private void _ReturnWeaponController()
    {
        foreach (var abilityController in _usedWeaponControllerDic)
        {
            abilityController.Value.ReturnAbilities();
            Manager.Instance.Object.ReturnWeaponController(abilityController.Key);
        }
    }
    #endregion

    #region Ability
    private Dictionary<string, OwnedAbilityInfo> _ownedAbilityInfoDic = new Dictionary<string, OwnedAbilityInfo>();
    private List<string> _ownedWeaponList = new List<string>();
    private List<string> _ownedBookList = new List<string>();
    private List<string> _drawAbilityList = new List<string>();

    public List<string> OwnedWeaponList { get { return _ownedWeaponList; } }
    public List<string> OwnedBookList { get { return _ownedBookList; } }
    public List<string> DrawAbilityList { get { return _drawAbilityList; } }

    private const int NEW_ABILITY_LEVEL = 0;
    private const int INIT_OWNED_ABILITY_LEVEL = 1;
    private const int DRAW_ABILITY_COUNT = 3;
    private const int MAX_WEAPON_ABILITY_LEVEL = 5;
    private const int MAX_BOOK_ABILITY_LEVEL = 3;

    public int GetOwnedAbilityLevel(string abilityName)
    {
        if (false == _ownedAbilityInfoDic.TryGetValue(abilityName, out var abilityInfo))
            return NEW_ABILITY_LEVEL;
        return abilityInfo.Level;
    }

    public void RegistAbility(string abilityName)
    {
        if (_ownedAbilityInfoDic.TryGetValue(abilityName, out var ownedAbilityInfo))
        {
            ++ownedAbilityInfo.Level;
            foreach (var abilityController in ownedAbilityInfo.AbilityControllerList)
                abilityController.SetAbilities();
            return;
        }

        switch (abilityName)
        {
            case Define.WEAPON_ARCANE_MAGE_WAND:
                _ownedAbilityInfoDic.Add(abilityName, new OwnedAbilityInfo() { Level = INIT_OWNED_ABILITY_LEVEL, AbilityControllerList = new List<IAbilityController>() { UsedHero } });
                break;
            case Define.WEAPON_BOMB:
                _ownedAbilityInfoDic.Add(abilityName, new OwnedAbilityInfo() { Level = INIT_OWNED_ABILITY_LEVEL, AbilityControllerList = new List<IAbilityController>() { _usedWeaponControllerDic[Define.RESOURCE_WEAPON_BOMB_CONTROLLER] } });
                Utils.SetActive(_usedWeaponControllerDic[Define.RESOURCE_WEAPON_BOMB_CONTROLLER].gameObject, true);
                break;
            case Define.WEAPON_BOOMERANG:
                _ownedAbilityInfoDic.Add(abilityName, new OwnedAbilityInfo() { Level = INIT_OWNED_ABILITY_LEVEL, AbilityControllerList = new List<IAbilityController>() { _usedWeaponControllerDic[Define.RESOURCE_WEAPON_BOOMERANG_CONTROLLER] } });
                Utils.SetActive(_usedWeaponControllerDic[Define.RESOURCE_WEAPON_BOOMERANG_CONTROLLER].gameObject, true);
                break;
            case Define.WEAPON_CROSSBOW:
                _ownedAbilityInfoDic.Add(abilityName, new OwnedAbilityInfo() { Level = INIT_OWNED_ABILITY_LEVEL, AbilityControllerList = new List<IAbilityController>() { _usedWeaponControllerDic[Define.RESOURCE_WEAPON_CROSSBOW_CONTROLLER] } });
                Utils.SetActive(_usedWeaponControllerDic[Define.RESOURCE_WEAPON_CROSSBOW_CONTROLLER].gameObject, true);
                break;
            case Define.WEAPON_DIVINE_AURA:
                _ownedAbilityInfoDic.Add(abilityName, new OwnedAbilityInfo() { Level = INIT_OWNED_ABILITY_LEVEL, AbilityControllerList = new List<IAbilityController>() { _usedWeaponControllerDic[Define.RESOURCE_WEAPON_DIVINE_AURA_CONTROLLER] } });
                Utils.SetActive(_usedWeaponControllerDic[Define.RESOURCE_WEAPON_DIVINE_AURA_CONTROLLER].gameObject, true);
                break;
            case Define.WEAPON_FIREBALL:
                _ownedAbilityInfoDic.Add(abilityName, new OwnedAbilityInfo() { Level = INIT_OWNED_ABILITY_LEVEL, AbilityControllerList = new List<IAbilityController>() { _usedWeaponControllerDic[Define.RESOURCE_WEAPON_FIREBALL_CONTROLLER] } });
                Utils.SetActive(_usedWeaponControllerDic[Define.RESOURCE_WEAPON_FIREBALL_CONTROLLER].gameObject, true);
                break;
            case Define.BOOK_COOLDOWN:
            case Define.BOOK_PROJECTILE_COPY:
                _ownedAbilityInfoDic.Add(abilityName, new OwnedAbilityInfo() { Level = INIT_OWNED_ABILITY_LEVEL, AbilityControllerList = new List<IAbilityController>() { UsedHero, _usedWeaponControllerDic[Define.RESOURCE_WEAPON_BOMB_CONTROLLER], _usedWeaponControllerDic[Define.RESOURCE_WEAPON_BOOMERANG_CONTROLLER], _usedWeaponControllerDic[Define.RESOURCE_WEAPON_CROSSBOW_CONTROLLER], _usedWeaponControllerDic[Define.RESOURCE_WEAPON_DIVINE_AURA_CONTROLLER], _usedWeaponControllerDic[Define.RESOURCE_WEAPON_FIREBALL_CONTROLLER] } });
                break;
            case Define.BOOK_HERO_MOVE_SPEED:
            case Define.BOOK_HERO_RECOVERY:
                _ownedAbilityInfoDic.Add(abilityName, new OwnedAbilityInfo() { Level = INIT_OWNED_ABILITY_LEVEL, AbilityControllerList = new List<IAbilityController>() { UsedHero } });
                break;
            case Define.BOOK_PROJECTILE_SPEED:
            case Define.BOOK_RANGE:
                _ownedAbilityInfoDic.Add(abilityName, new OwnedAbilityInfo() { Level = INIT_OWNED_ABILITY_LEVEL, AbilityControllerList = new List<IAbilityController>() { _usedWeaponControllerDic[Define.RESOURCE_WEAPON_BOMB_CONTROLLER], _usedWeaponControllerDic[Define.RESOURCE_WEAPON_BOOMERANG_CONTROLLER], _usedWeaponControllerDic[Define.RESOURCE_WEAPON_CROSSBOW_CONTROLLER], _usedWeaponControllerDic[Define.RESOURCE_WEAPON_DIVINE_AURA_CONTROLLER], _usedWeaponControllerDic[Define.RESOURCE_WEAPON_FIREBALL_CONTROLLER] } });
                break;
            default:
                Debug.LogError($"Out of range of ability! : {abilityName}");
                return;
        }
        foreach (var abilityController in _ownedAbilityInfoDic[abilityName].AbilityControllerList)
            abilityController.SetAbilities();

        var abilityInfo = Define.ABILITY_INFO_DIC[abilityName];
        if (EAbilityTypes.HeroWeapon == abilityInfo.AbilityType || EAbilityTypes.Weapon == abilityInfo.AbilityType)
            _ownedWeaponList.Add(abilityName);
        else if (EAbilityTypes.Book == abilityInfo.AbilityType)
            _ownedBookList.Add(abilityName);
    }

    private void _DrawAbility()
    {
        _drawAbilityList.Clear();
        for (int ii = 0; ii < DRAW_ABILITY_COUNT; ++ii)
            _drawAbilityList.Add(_Draw());
    }

    private string _Draw()
    {
        var drawIndex = UnityEngine.Random.Range(0, Define.ABILITY_LIST.Count);
        var abilityName = Define.ABILITY_LIST[drawIndex];

        // 영웅 전용 무기일 때 가져야할 영웅이 아니면 다시 뽑기
        var abilityType = Define.ABILITY_INFO_DIC[abilityName].AbilityType;
        if (EAbilityTypes.HeroWeapon == abilityType)
        {
            if (false == abilityName.Equals(UsedHero.HeroWeaponName))
                return _Draw();
        }
        // 선택한 능력이 최대 레벨일 때 다시 뽑기
        if (EAbilityTypes.HeroWeapon == abilityType || EAbilityTypes.Weapon == abilityType)
        {
            if (MAX_WEAPON_ABILITY_LEVEL == GetOwnedAbilityLevel(abilityName))
                return _Draw();
        }
        else if (EAbilityTypes.Book == abilityType)
        {
            if (MAX_BOOK_ABILITY_LEVEL == GetOwnedAbilityLevel(abilityName))
                return _Draw();
        }
        // 이미 뽑은 무기라면 다시 뽑기
        foreach (var drawAbility in DrawAbilityList)
        {
            if (abilityName.Equals(drawAbility))
                return _Draw();
        }
        return abilityName;
    }
    #endregion
}
