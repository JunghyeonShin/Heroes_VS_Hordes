using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Wave : MonoBehaviour
{
    protected float _progressTime;

    public bool ProgressWave { get; set; }

    protected const float INIT_PROGRESS_TIME = 0f;
    protected const float DELAY_CLEAR_INGAME = 1.2f;
    protected const float DELAY_GET_DROP_ITEM = 1.2f;

    private void Update()
    {
        _ProgressWaveTime();
    }

    /// <summary>
    /// 웨이브를 실행할 때 호출
    /// </summary>
    public abstract void StartWave();

    /// <summary>
    /// 웨이브를 멈추거나 재시작할 때 호출
    /// </summary>
    /// <param name="control">인게임 조절로 false면 멈춤, true면 재시작</param>
    public virtual void ControlWave(bool control)
    {
        ProgressWave = control;
    }

    /// <summary>
    /// 현재 웨이브를 클리어 했을 때 호출
    /// </summary>
    public abstract void ClearWave();

    /// <summary>
    /// 인게임을 중간에 포기하거나 클리어하고 나갈 때 호출
    /// </summary>
    public virtual void ExitWave()
    {
        ProgressWave = false;
        _progressTime = INIT_PROGRESS_TIME;
    }

    protected abstract void _ProgressWaveTime();
}
