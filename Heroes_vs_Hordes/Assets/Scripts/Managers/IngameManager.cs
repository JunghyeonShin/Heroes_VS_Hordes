using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public event Action WavePanelHandler;
    public event Action<float> TimePassHandler;
    public event Action<int> RemainingMonsterHandler;
    public event Action<int> ChangeModeHandler;

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
    /// 인게임을 처음 실행하거나 웨이브를 클리어하고 다음 웨이브를 실행할 때 호출
    /// </summary>
    /// <param name="waveIndex">실행할 웨이브 목차</param>
    public void StartIngame(int waveIndex)
    {
        Utils.SetTimeScale(RESTORE_TIMESCALE);
        CurrentWaveIndex = waveIndex;
        WavePanelHandler?.Invoke();
        #region TEST
        _totalWaveProgressTime = 10f;
        #endregion
        TimePassHandler?.Invoke(_totalWaveProgressTime);
        ChangeModeHandler?.Invoke(Define.TIME_ATTACK_MODE);
    }

    /// <summary>
    /// 인게임을 멈추거나 재시작할 때 호출
    /// </summary>
    /// <param name="control">인게임 조절로 false면 멈춤, true면 재시작</param>
    public void ControlIngame(bool control)
    {
        ProgressWave = control;
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
    /// 인게임을 중간에 포기하거나 클리어하고 나갈 때 호출
    /// </summary>
    public void ExitIngame()
    {
        ProgressWave = false;
        Utils.SetTimeScale(RESTORE_TIMESCALE);
        _waveProgressTime = INIT_WAVE_PROGRESS_TIME;
    }

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
