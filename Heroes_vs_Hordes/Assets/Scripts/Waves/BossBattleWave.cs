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
    private const float DELAY_LOADING_TIME = 1f;
    private const string NAME_BOSS_BATTLE = "보스 전투";

    private readonly Vector3 ADJUST_FOLLOW_POSITION = new Vector3(0f, 8f, 0f);

    public override void StartWave()
    {
        _loadCompleteBossMap = false;
        _StartWave().Forget();
    }

    public override void ClearWave()
    {
        Manager.Instance.CameraController.SetFollower(Manager.Instance.Ingame.UsedHero.transform);
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
            var usedHero = Manager.Instance.Ingame.UsedHero;

            bossMapGO.transform.position = usedHero.transform.position;
            Utils.SetActive(bossMapGO, true);

            usedHero.transform.position -= ADJUST_FOLLOW_POSITION;

            var bossMapCameraFollower = Manager.Instance.Object.BossMapCameraFollower;
            var chaseHero = Utils.GetOrAddComponent<ChaseHero>(bossMapCameraFollower);
            chaseHero.HeroTransform = usedHero.transform;
            chaseHero.AdjustPosition = ADJUST_FOLLOW_POSITION;
            Utils.SetActive(bossMapCameraFollower, true);

            Manager.Instance.CameraController.SetFollower(bossMapCameraFollower.transform);

            _loadCompleteBossMap = true;
        });

        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_LOADING_TIME));

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
