using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleWave : Wave
{
    private UI_Fade _fadeUI;
    private BossMonster _usedBossMonster;

    private bool _loadCompleteBossMap;
    private bool _loadCompleteBossMonster;

    private const float FADE_TIME = 0.3f;
    private const float DELAY_LOADING_TIME = 1f;
    private const string NAME_BOSS_BATTLE = "���� ����";

    private readonly Vector3 ADJUST_FOLLOW_POSITION = new Vector3(0f, 8f, 0f);

    public override void StartWave()
    {
        _usedBossMonster = null;
        _loadCompleteBossMap = false;
        _loadCompleteBossMonster = false;
        _StartWave().Forget();
    }

    public override void ClearWave()
    {
        Manager.Instance.CameraController.SetFollower(Manager.Instance.Ingame.UsedHero.transform);
        Manager.Instance.Object.ReturnBossMap(Manager.Instance.Data.ChapterInfoDataList[Manager.Instance.Ingame.CurrentChapterIndex].BossMapType);
        _ClearWave().Forget();
    }

    public override void ExitWave()
    {
        base.ExitWave();

        Manager.Instance.Object.ReturnBossMap(Manager.Instance.Data.ChapterInfoDataList[Manager.Instance.Ingame.CurrentChapterIndex].BossMapType);
        _usedBossMonster.ReturnMonster();
    }

    private async UniTaskVoid _StartWave()
    {
        Manager.Instance.UI.ShowPopupUI<UI_Fade>(Define.RESOURCE_UI_FADE, (fadeUI) =>
        {
            _fadeUI = fadeUI;
            _fadeUI.FadeOut(FADE_TIME, null);
        });

        var bossMapPosition = Vector3.zero;
        var currentChapterInfo = Manager.Instance.Data.ChapterInfoDataList[Manager.Instance.Ingame.CurrentChapterIndex];
        Manager.Instance.Object.GetBossMap(currentChapterInfo.BossMapType, (bossMapGO) =>
        {
            var usedHero = Manager.Instance.Ingame.UsedHero;

            bossMapGO.transform.position = usedHero.transform.position;
            bossMapPosition = bossMapGO.transform.position;
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

        Manager.Instance.Object.GetBossMonster(currentChapterInfo.BossMonsterType, (bossMonsterGO) =>
        {
            _usedBossMonster = Utils.GetOrAddComponent<BossMonster>(bossMonsterGO);
            _usedBossMonster.Target = Manager.Instance.Ingame.UsedHero.transform;
            _usedBossMonster.BossMapPosition = bossMapPosition;
            _usedBossMonster.InitMonsterAbilities();
            Utils.SetActive(_usedBossMonster.gameObject, true);

            _loadCompleteBossMonster = true;
        });

        while (false == _loadCompleteBossMonster)
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
            _usedBossMonster.ReturnMonster();
            Manager.Instance.Ingame.ReturnUsedMonster();
        });
    }
}
