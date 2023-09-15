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
    /// ���̺긦 ������ �� ȣ��
    /// </summary>
    public abstract void StartWave();

    /// <summary>
    /// ���̺긦 ���߰ų� ������� �� ȣ��
    /// </summary>
    /// <param name="control">�ΰ��� ������ false�� ����, true�� �����</param>
    public virtual void ControlWave(bool control)
    {
        ProgressWave = control;
    }

    /// <summary>
    /// ���� ���̺긦 Ŭ���� ���� �� ȣ��
    /// </summary>
    public abstract void ClearWave();

    /// <summary>
    /// �ΰ����� �߰��� �����ϰų� Ŭ�����ϰ� ���� �� ȣ��
    /// </summary>
    public virtual void ExitWave()
    {
        ProgressWave = false;
        _progressTime = INIT_PROGRESS_TIME;
    }

    protected abstract void _ProgressWaveTime();
}
