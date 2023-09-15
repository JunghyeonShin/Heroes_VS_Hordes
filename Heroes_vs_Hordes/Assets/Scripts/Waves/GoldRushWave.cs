using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldRushWave : Wave
{
    private const string NAME_GOLD_RUSH = "°ñµå ·¯½¬";

    public override void StartWave()
    {
        Manager.Instance.Ingame.ShowWavePanel(NAME_GOLD_RUSH);

        _progressTime = INIT_PROGRESS_TIME;
    }

    public override void ClearWave()
    {
        _ClearWave().Forget();
    }

    protected override void _ProgressWaveTime()
    {
        #region TEST
        if (false == ProgressWave)
            return;

        _progressTime += Time.deltaTime;
        if (_progressTime > 20f)
        {
            _progressTime = INIT_PROGRESS_TIME;
            Manager.Instance.Ingame.StopSpawnMonster();
            ClearWave();
        }
        #endregion
    }

    private async UniTaskVoid _ClearWave()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_CLEAR_INGAME));

        Manager.Instance.UI.ShowPopupUI<UI_ClearChapter>(Define.RESOURCE_UI_CLEAR_CHAPTER);
        Manager.Instance.Ingame.ReturnUsedMonster();
    }
}
