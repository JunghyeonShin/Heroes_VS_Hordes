using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public event Action ChangeHeroLevelPostProcessingHandler;
    public event Action RemainingMonsterHandler;
    public event Action WavePanelHandler;
    public event Action<bool> ChangeModeHandler;
    public event Action<float> ChangeHeroExpHandler;
    public event Action<float> TimePassHandler;
    public event Action<int> ChangeHeroLevelHandler;

    private bool _spawnMonster;
    private float _totalWaveProgressTime;
    private float _waveProgressTime;
    private int _remainingMonsterCount = 0;

    public bool ProgressTimeAttack { get; set; }
    public int CurrentWaveIndex { get; private set; }
    public int TotalWaveIndex { get; private set; }
    public int RemainingMonsterCount { get { return _remainingMonsterCount; } }

    private const float INIT_WAVE_PROGRESS_TIME = 0f;
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;
    private const float PAUSE_INGAME = 0f;
    private const float RESTART_INGAME = 1f;
    private const float RESTORE_TIMESCALE = 1f;
    private const int INIT_WAVE_INDEX = 0;
    private const int NEXT_WAVE_INDEX = 1;
    #region TEST
    private const int CURRENT_CHAPTER_INDEX = 0;
    #endregion

    private void Update()
    {
        _CheckIngameProgressTime();
    }

    /// <summary>
    /// �ΰ����� ó�� �����Ͽ� �ʱ� ������ �� ȣ��
    /// </summary>
    public void InitIngame(UI_Loading loadingUI)
    {
        _spawnMonster = false;

        CurrentWaveIndex = INIT_WAVE_INDEX;
        TotalWaveIndex = Manager.Instance.Data.ChapterInfoList[CURRENT_CHAPTER_INDEX].TotalWaveIndex;

        // UI_PauseIngame�� ���̺� ���൵ �ʱ� ����
        var pauseIngameUI = Manager.Instance.UI.FindUI<UI_PauseIngame>(Define.RESOURCE_UI_PAUSE_INGAME);
        pauseIngameUI.InitWavePanel();

        // UI_ClearWave�� ���̺� ���൵ �ʱ� ����
        var clearWaveUI = Manager.Instance.UI.FindUI<UI_ClearWave>(Define.RESOURCE_UI_CLEAR_WAVE);
        clearWaveUI.InitWavePanel();

        // �� ����
        Manager.Instance.Object.GetMap(Manager.Instance.Data.ChapterInfoList[CURRENT_CHAPTER_INDEX].MapType, (mapGO) =>
        {
            Utils.SetActive(mapGO, true);

            // ���� ����
            Manager.Instance.Object.GetHero(Define.RESOURCE_HERO_ARCANE_MAGE, (heroGO) =>
            {
                var hero = heroGO.GetComponent<Hero>();
                hero.SetHeroAbilities();

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

                // ī�޶� �ȷο� ����
                Manager.Instance.CameraController.SetFollower(heroGO.transform);

                Utils.SetActive(heroGO, true);

                loadingUI.CompleteLoading();
            });
        });
    }

    /// <summary>
    /// �ΰ����� ó�� �����ϰų� ���̺긦 Ŭ�����ϰ� ���� ���̺긦 ������ �� ȣ��
    /// </summary>
    public void StartIngame()
    {
        Utils.SetTimeScale(RESTORE_TIMESCALE);
        WavePanelHandler?.Invoke();
        _totalWaveProgressTime = Manager.Instance.Data.ChapterInfoList[CURRENT_CHAPTER_INDEX].Time;
        _totalWaveProgressTime = 10f;
        TimePassHandler?.Invoke(_totalWaveProgressTime);
        ChangeModeHandler?.Invoke(true);
    }

    /// <summary>
    /// �ΰ����� ���߰ų� ������� �� ȣ��
    /// </summary>
    /// <param name="control">�ΰ��� ������ false�� ����, true�� �����</param>
    public void ControlIngame(bool control)
    {
        ProgressTimeAttack = control;
        if (control)
            Utils.SetTimeScale(RESTART_INGAME);
        else
            Utils.SetTimeScale(PAUSE_INGAME);
    }

    /// <summary>
    /// ���� ���̺긦 Ŭ���� ���� �� ȣ��
    /// </summary>
    public void ClearIngame()
    {
        // Ŭ���� ���� �߰�
        // ����ġ ȸ�� �� �������� UI ȣ��

        ProgressTimeAttack = false;
        Utils.SetTimeScale(PAUSE_INGAME);
        CurrentWaveIndex += NEXT_WAVE_INDEX;
        if (CurrentWaveIndex < TotalWaveIndex)
            Manager.Instance.UI.ShowPopupUI<UI_ClearWave>(Define.RESOURCE_UI_CLEAR_WAVE, (clearWaveUI) =>
            {
                clearWaveUI.SetClearWaveText();
                clearWaveUI.UpdateWavePanel();
            });
        else
            Manager.Instance.UI.ShowPopupUI<UI_ClearChapter>(Define.RESOURCE_UI_CLEAR_CHAPTER);
    }

    /// <summary>
    /// �ΰ����� �߰��� �����ϰų� Ŭ�����ϰ� ���� �� ȣ��
    /// </summary>
    public void ExitIngame()
    {
        ProgressTimeAttack = false;
        Utils.SetTimeScale(RESTORE_TIMESCALE);
        _waveProgressTime = INIT_WAVE_PROGRESS_TIME;

        Manager.Instance.CameraController.SetFollower();
        Utils.SetActive(Manager.Instance.Object.MonsterSpawner, false);
        Utils.SetActive(Manager.Instance.Object.RepositionArea, false);
        Manager.Instance.Object.ReturnHero(Define.RESOURCE_HERO_ARCANE_MAGE);
        Manager.Instance.Object.ReturnMap(Manager.Instance.Data.ChapterInfoList[CURRENT_CHAPTER_INDEX].MapType);
        Manager.Instance.UI.ShowSceneUI<UI_MainScene>(Define.RESOURCE_UI_MAIN_SCENE);
    }

    #region Hero
    public void ChangeHeroExp(float value)
    {
        ChangeHeroExpHandler?.Invoke(value);
    }

    public void ChangeHeroLevel(int level)
    {
        ChangeHeroLevelHandler?.Invoke(level);
    }

    public void ChangeHeroLevelPostProcessing()
    {
        ChangeHeroLevelPostProcessingHandler?.Invoke();
    }
    #endregion

    #region Monster
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

    private void _CheckIngameProgressTime()
    {
        if (false == ProgressTimeAttack)
            return;

        if (_totalWaveProgressTime > ZERO_SECOND)
        {
            _waveProgressTime += Time.deltaTime;
            if (_waveProgressTime >= ONE_SECOND)
            {
                _waveProgressTime -= ONE_SECOND;
                _totalWaveProgressTime -= ONE_SECOND;
                TimePassHandler?.Invoke(_totalWaveProgressTime);
            }

            if (ZERO_SECOND == _totalWaveProgressTime)
                ChangeModeHandler?.Invoke(false);
        }
    }
}
