using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBattleWave : Wave
{
    private float _totalProgressTime;

    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;
    private const float DELAY_GET_EXP = 1.2f;
    private const int ADJUST_WAVE_INDEX = 1;

    public override void StartWave()
    {
        Manager.Instance.Ingame.ShowWavePanel($"¿þÀÌºê {Manager.Instance.Ingame.CurrentWaveIndex + ADJUST_WAVE_INDEX}");
        _totalProgressTime = Manager.Instance.Data.ChapterInfoList[Define.CURRENT_CHAPTER_INDEX].Time;
        _totalProgressTime = 10;
        _progressTime = INIT_PROGRESS_TIME;
        Manager.Instance.Ingame.ChangeProgressWaveTime(_totalProgressTime);
        Manager.Instance.Ingame.ChangeMode(Define.INDEX_TIME_ATTACK_MODE);
    }

    public override void ClearWave()
    {
        ProgressWave = false;
        _ClearWave().Forget();
    }

    protected override void _ProgressWaveTime()
    {
        if (false == ProgressWave)
            return;

        if (_totalProgressTime > ZERO_SECOND)
        {
            _progressTime += Time.deltaTime;
            if (_progressTime >= ONE_SECOND)
            {
                _progressTime -= ONE_SECOND;
                _totalProgressTime -= ONE_SECOND;
                Manager.Instance.Ingame.ChangeProgressWaveTime(_totalProgressTime);
            }

            if (ZERO_SECOND == _totalProgressTime)
                Manager.Instance.Ingame.ChangeMode(Define.INDEX_ANNIHILATION_MODE);
        }
    }

    private async UniTaskVoid _ClearWave()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_CLEAR_INGAME));

        Manager.Instance.Ingame.ReturnUsedExpGem();
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_GET_EXP));

        Manager.Instance.Ingame.GetExpAtOnce();
        while (Manager.Instance.Ingame.HeroLevelUpCount > Define.INIT_HERO_LEVEL_UP_COUNT)
            await UniTask.Yield();

        Manager.Instance.Ingame.ClearIngamePostProcessing();
        Manager.Instance.UI.ShowPopupUI<UI_ClearWave>(Define.RESOURCE_UI_CLEAR_WAVE, (clearWaveUI) =>
        {
            clearWaveUI.SetClearWaveText();
            clearWaveUI.UpdateWavePanel();
            Manager.Instance.Ingame.ReturnUsedMonster();
        });
    }
}
