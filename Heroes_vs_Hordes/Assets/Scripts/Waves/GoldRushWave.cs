using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldRushWave : Wave
{
    private const string NAME_GOLD_RUSH = "°ñµå ·¯½¬";
    private const float DELAY_CLEAR_CHAPTER_TIME = 1f;

    #region TEST
    private bool _test;
    #endregion

    public override void StartWave()
    {
        Manager.Instance.Ingame.ShowWavePanel(NAME_GOLD_RUSH);
        Manager.Instance.Ingame.ChangeGold();
        _progressTime = INIT_PROGRESS_TIME;
        #region TEST
        _test = true; ;
        #endregion
    }

    public override void ClearWave()
    {
        Manager.Instance.Ingame.StopSpawnMonster();
        _ClearWave().Forget();
    }

    protected override void _ProgressWaveTime()
    {
        if (false == ProgressWave)
            return;

        #region TEST
        if (_test)
        {
            _progressTime += Time.deltaTime;
            if (_progressTime > 20f)
            {
                Manager.Instance.Ingame.UsedHero.SetDead();
                _test = false;
            }
        }
        #endregion
    }

    private async UniTaskVoid _ClearWave()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_CLEAR_INGAME));

        Manager.Instance.Ingame.ReturnUsedGold();
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_GET_DROP_ITEM));

        Manager.Instance.Ingame.GetGoldAtOnce();
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_CLEAR_CHAPTER_TIME));

        Manager.Instance.UI.ShowPopupUI<UI_ClearChapter>(Define.RESOURCE_UI_CLEAR_CHAPTER);
        Manager.Instance.Ingame.ReturnUsedMonster();
    }
}
