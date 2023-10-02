using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleWave : Wave
{
    private const string NAME_BOSS_BATTLE = "보스 웨이브";
    public override void StartWave()
    {
        Manager.Instance.Ingame.ShowWavePanel(NAME_BOSS_BATTLE);
    }

    public override void ClearWave()
    {
        _ClearWave().Forget();
    }

    private async UniTaskVoid _ClearWave()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_CLEAR_INGAME));

        Manager.Instance.Ingame.ReturnUsedExpGem();
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_GET_DROP_ITEM));

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
