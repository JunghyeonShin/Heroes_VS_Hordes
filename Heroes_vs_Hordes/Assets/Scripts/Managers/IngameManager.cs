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
    #region TEST
    private const int ANNIHILATION_MODE = 1;
    private const int MAX_WAVE_INDEX = 4;
    #endregion

    private void Update()
    {
        _CheckIngameProgressTime();

        #region TEST
        if (Input.GetKeyDown(KeyCode.F))
            RemainingMonsterHandler?.Invoke(0);
        #endregion
    }

    /// <summary>
    /// �ΰ����� ó�� �����ϰų� ���̺긦 Ŭ�����ϰ� ���� ���̺긦 ������ �� ȣ��
    /// </summary>
    /// <param name="waveIndex">������ ���̺� ����</param>
    public void StartIngame(int waveIndex)
    {
        Time.timeScale = RESTORE_TIMESCALE;
        CurrentWaveIndex = waveIndex;
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
            Time.timeScale = RESTART_INGAME;
        else
            Time.timeScale = PAUSE_INGAME;
    }

    /// <summary>
    /// �ΰ����� �߰��� ������ �� ȣ��
    /// </summary>
    public void GiveUpIngame()
    {
        ProgressWave = false;
        Time.timeScale = RESTORE_TIMESCALE;
        _waveProgressTime = INIT_WAVE_PROGRESS_TIME;
    }

    /// <summary>
    /// ���� ���̺긦 Ŭ���� ���� �� ȣ��
    /// </summary>
    public void ClearIngame()
    {
        ProgressWave = false;
        Time.timeScale = PAUSE_INGAME;
        if (CurrentWaveIndex < MAX_WAVE_INDEX - 1)
            Manager.Instance.UI.ShowPopupUI<UI_ClearWave>(Define.RESOURCE_UI_CLEAR_WAVE, (clearWaveUI) =>
            {
                clearWaveUI.SetClearWaveText();
            });
        else
            Manager.Instance.UI.ShowPopupUI<UI_ClearChapter>(Define.RESOURCE_UI_CLEAR_CHAPTER);
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
