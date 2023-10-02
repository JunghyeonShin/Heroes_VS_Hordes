using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleWave : Wave
{
    private UI_Fade _fadeUI;

    private bool _loadCompleteBossMap;

    private const float FADE_TIME = 0.3f;
    private const string NAME_BOSS_BATTLE = "보스 웨이브";

    public override void StartWave()
    {
        _loadCompleteBossMap = false;
        _StartWave().Forget();
    }

    public override void ClearWave()
    {
        Manager.Instance.Object.ReturnBossMap(Manager.Instance.Data.ChapterInfoDataList[Define.CURRENT_CHAPTER_INDEX].BossMapType);
        _ClearWave().Forget();
    }

    public override void ExitWave()
    {
        base.ExitWave();

        Manager.Instance.Object.ReturnBossMap(Manager.Instance.Data.ChapterInfoDataList[Define.CURRENT_CHAPTER_INDEX].BossMapType);
    }

    private async UniTaskVoid _StartWave()
    {
        Manager.Instance.UI.ShowPopupUI<UI_Fade>(Define.RESOURCE_UI_FADE, (fadeUI) =>
        {
            _fadeUI = fadeUI;
            _fadeUI.FadeOut(FADE_TIME, null);
        });

        Manager.Instance.Object.GetBossMap(Manager.Instance.Data.ChapterInfoDataList[Define.CURRENT_CHAPTER_INDEX].BossMapType, (bossMapGO) =>
        {
            bossMapGO.transform.position = Manager.Instance.Ingame.UsedHero.transform.position;
            Utils.SetActive(bossMapGO, true);
            _loadCompleteBossMap = true;
        });

        while (false == _loadCompleteBossMap)
            await UniTask.Yield();

        _fadeUI.FadeIn(FADE_TIME, () =>
        {
            Manager.Instance.Ingame.ShowWavePanel(NAME_BOSS_BATTLE);
            _fadeUI.ClosePopupUI();
        });
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
