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
    public int CurrentWaveIndex { get; private set; }
    public int TotalWaveIndex { get; private set; }

    private const float PAUSE_INGAME = 0f;
    private const float RESTART_INGAME = 1f;
    private const float RESTORE_TIMESCALE = 1f;
    private const int INIT_WAVE_INDEX = 0;
    private const int NEXT_WAVE_INDEX = 1;
    private const string NAME_WAVE = "[WAVE]";

    /// <summary>
    /// 인게임을 처음 진입하여 초기 세팅할 때 호출
    /// </summary>
    public void InitIngame(UI_Loading loadingUI)
    {
        _spawnMonster = false;
        HeroLevelUpCount = Define.INIT_HERO_LEVEL_UP_COUNT;

        CurrentWaveIndex = INIT_WAVE_INDEX;
        TotalWaveIndex = Manager.Instance.Data.ChapterInfoList[Define.CURRENT_CHAPTER_INDEX].TotalWaveIndex;

        // UI_PauseIngame의 웨이브 진행도 초기 세팅
        var pauseIngameUI = Manager.Instance.UI.FindUI<UI_PauseIngame>(Define.RESOURCE_UI_PAUSE_INGAME);
        pauseIngameUI.InitWavePanel();

        // UI_ClearWave의 웨이브 진행도 초기 세팅
        var clearWaveUI = Manager.Instance.UI.FindUI<UI_ClearWave>(Define.RESOURCE_UI_CLEAR_WAVE);
        clearWaveUI.InitWavePanel();

        // 맵 생성
        Manager.Instance.Object.GetMap(Manager.Instance.Data.ChapterInfoList[Define.CURRENT_CHAPTER_INDEX].MapType, (mapGO) =>
        {
            Utils.SetActive(mapGO, true);

            // 영웅 생성
            Manager.Instance.Object.GetHero(Define.RESOURCE_HERO_ARCANE_MAGE, (heroGO) =>
            {
                var hero = heroGO.GetComponent<Hero>();
                _usedHero = hero;
                _usedHero.SetHeroAbilities();

                var heroController = Utils.GetOrAddComponent<HeroController>(heroGO);

                var mapController = Utils.GetOrAddComponent<MapController>(mapGO);
                mapController.SetHeroController(heroController);

                {
                    var mapCollisionArea = Manager.Instance.Object.RepositionArea;
                    var chaseHero = Utils.GetOrAddComponent<ChaseHero>(mapCollisionArea);
                    chaseHero.HeroTransform = heroGO.transform;
                    Utils.SetActive(mapCollisionArea, true);
                }

                {
                    var monsterSpawner = Manager.Instance.Object.MonsterSpawner;
                    var chaseHero = Utils.GetOrAddComponent<ChaseHero>(monsterSpawner);
                    chaseHero.HeroTransform = heroGO.transform;
                    var spawnMonster = Utils.GetOrAddComponent<SpawnMonster>(monsterSpawner);
                    spawnMonster.HeroController = heroController;
                    Utils.SetActive(monsterSpawner, true);
                }

                // 카메라 팔로워 세팅
                Manager.Instance.CameraController.SetFollower(heroGO.transform);

                Utils.SetActive(heroGO, true);

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
        CurrentWave = _waveList[Manager.Instance.Data.ChapterInfoList[Define.CURRENT_CHAPTER_INDEX].WaveIndex[CurrentWaveIndex]];
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
        Manager.Instance.CameraController.SetFollower();
        Utils.SetActive(Manager.Instance.Object.MonsterSpawner, false);
        Utils.SetActive(Manager.Instance.Object.RepositionArea, false);
        Manager.Instance.Object.ReturnHero(Define.RESOURCE_HERO_ARCANE_MAGE);
        Manager.Instance.Object.ReturnMap(Manager.Instance.Data.ChapterInfoList[Define.CURRENT_CHAPTER_INDEX].MapType);
        Manager.Instance.UI.ShowSceneUI<UI_MainScene>(Define.RESOURCE_UI_MAIN_SCENE);
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
    }
    #endregion

    #region Hero
    public event Action<float> ChangeHeroExpHandler;
    public event Action<int> ChangeHeroLevelHandler;
    public event Action ChangeHeroLevelUpPostProcessingHandler;
    public event Action EnhanceHeroAbilityHandler;

    public int HeroLevelUpCount { get; set; }
    private Hero _usedHero;

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

    public void EnhanceHeroAbility()
    {
        if (HeroLevelUpCount > Define.INIT_HERO_LEVEL_UP_COUNT)
            EnhanceHeroAbilityHandler?.Invoke();
        else
            ChangeHeroLevelUpPostProcessingHandler?.Invoke();
    }

    public void GetExpAtOnce()
    {
        _usedHero.GetExp(_remainingExp);
        _remainingExp = INIT_REMAINING_EXP;
    }
    #endregion

    #region Monster
    public event Action RemainingMonsterHandler;

    private bool _spawnMonster;
    private int _remainingMonsterCount = 0;

    public int RemainingMonsterCount { get { return _remainingMonsterCount; } }

    public void StartSpawnMonster()
    {
        _spawnMonster = true;
        _SpawnMonster().Forget();
    }

    public void StopSpawnMonster()
    {
        _spawnMonster = false;
        RemainingMonsterHandler?.Invoke();
    }

    public void OnDeadMonster()
    {
        --_remainingMonsterCount;
        RemainingMonsterHandler?.Invoke();
    }

    private async UniTaskVoid _SpawnMonster()
    {
        while (_spawnMonster)
        {
            #region TEST
            var spawnMonster = Utils.GetOrAddComponent<SpawnMonster>(Manager.Instance.Object.MonsterSpawner);
            spawnMonster.Spawn(Define.RESOURCE_MONSTER_NORMAL_BAT, 10);
            _remainingMonsterCount += 10;
            await UniTask.Delay(TimeSpan.FromSeconds(10));
            #endregion
        }
    }
    #endregion

    #region ExpGem
    private Queue<ExpGem> _usedExpGem = new Queue<ExpGem>();

    private float _remainingExp = 0;

    private const float INIT_REMAINING_EXP = 0f;
    private const int EMPTY_USED_EXP_GEM = 0;

    public void EnqueueUsedExpGem(ExpGem expGem)
    {
        _usedExpGem.Enqueue(expGem);
    }

    public void ReturnUsedExpGem()
    {
        while (_usedExpGem.Count > EMPTY_USED_EXP_GEM)
        {
            var expGem = _usedExpGem.Dequeue();
            if (false == expGem.gameObject.activeSelf)
                continue;

            expGem.GiveEffect(_usedHero, false);
            _remainingExp += Define.INCREASE_HERO_EXP_VALUE;
        }
    }
    #endregion
}
