using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public event Action WavePanelHandler;
    public event Action ChangeHeroLevelPostProcessingHandler;
    public event Action<float> TimePassHandler;
    public event Action<float> ChangeHeroExpHandler;
    public event Action<int> ChangeHeroLevelHandler;
    public event Action<int> ChangeModeHandler;
    public event Action<int> RemainingMonsterHandler;

    private float _totalWaveProgressTime;
    private float _waveProgressTime;

    public bool ProgressWave { get; set; }
    public int CurrentWaveIndex { get; private set; }

    private const float INIT_WAVE_PROGRESS_TIME = 0f;
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;
    private const float PAUSE_INGAME = 0f;
    private const float RESTART_INGAME = 1f;
    private const float RESTORE_TIMESCALE = 1f;
    private const int INIT_WAVE_INDEX = 0;
    private const int NEXT_WAVE_INDEX = 1;
    #region TEST
    private const int ANNIHILATION_MODE = 1;
    #endregion

    private void Update()
    {
        _CheckIngameProgressTime();
        #region TEST
        if (Input.GetKeyDown(KeyCode.S))
        {
            var spawnMonster = Utils.GetOrAddComponent<SpawnMonster>(Manager.Instance.Object.MonsterSpawner);
            spawnMonster.Spawn(Define.RESOURCE_MONSTER_NORMAL_BAT, 10);
        }
        else if (Input.GetKeyDown(KeyCode.C))
            RemainingMonsterHandler?.Invoke(0);
        #endregion
    }

    /// <summary>
    /// �ΰ����� ó�� �����Ͽ� �ʱ� ������ �� ȣ��
    /// </summary>
    public void InitIngame(UI_Loading loadingUI)
    {
        CurrentWaveIndex = INIT_WAVE_INDEX;

        // UI_PauseIngame�� ���̺� ���൵ �ʱ� ����
        var pauseIngameUI = Manager.Instance.UI.FindUI<UI_PauseIngame>(Define.RESOURCE_UI_PAUSE_INGAME);
        pauseIngameUI.InitWavePanel();

        // UI_ClearWave�� ���̺� ���൵ �ʱ� ����
        var clearWaveUI = Manager.Instance.UI.FindUI<UI_ClearWave>(Define.RESOURCE_UI_CLEAR_WAVE);
        clearWaveUI.InitWavePanel();

        // �� ����
        Manager.Instance.Object.GetMap(Define.RESOURCE_MAP_00, (mapGO) =>
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
    /// <param name="waveIndex">������ ���̺� ����</param>
    public void StartIngame()
    {
        Utils.SetTimeScale(RESTORE_TIMESCALE);
        WavePanelHandler?.Invoke();
        #region TEST
        _totalWaveProgressTime = 10f;
        #endregion
        TimePassHandler?.Invoke(_totalWaveProgressTime);
        ChangeModeHandler?.Invoke(Define.TIME_ATTACK_MODE);
    }

    /// <summary>
    /// �ΰ����� ���߰ų� ������� �� ȣ��
    /// </summary>
    /// <param name="control">�ΰ��� ������ false�� ����, true�� �����</param>
    public void ControlIngame(bool control)
    {
        ProgressWave = control;
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
        ProgressWave = false;
        Utils.SetTimeScale(PAUSE_INGAME);
        CurrentWaveIndex += NEXT_WAVE_INDEX;
        if (CurrentWaveIndex < Define.MAX_WAVE_INDEX)
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
        ProgressWave = false;
        Utils.SetTimeScale(RESTORE_TIMESCALE);
        _waveProgressTime = INIT_WAVE_PROGRESS_TIME;
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

    private void _CheckIngameProgressTime()
    {
        if (false == ProgressWave)
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
                ChangeModeHandler?.Invoke(ANNIHILATION_MODE);
        }
    }
}
