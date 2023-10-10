using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldRushWave : Wave
{
    private const string NAME_GOLD_RUSH = "°ñµå ·¯½¬";
    private const float DELAY_CLEAR_CHAPTER_TIME = 1f;

    public override void StartWave()
    {
        Manager.Instance.Ingame.ShowWavePanel(NAME_GOLD_RUSH);
        Manager.Instance.Ingame.ChangeGold();
        _progressTime = INIT_PROGRESS_TIME;
    }

    public override void ClearWave()
    {
        Manager.Instance.Ingame.StopSpawnMonster();
        _ClearWave().Forget();
    }

    public override void OnDeadHero()
    {
        var heroDeath = Manager.Instance.Object.HeroDeath;
        var floatHeroDeath = Utils.GetOrAddComponent<FloatHeroDeath>(heroDeath);
        floatHeroDeath.SetTransform(Manager.Instance.Ingame.UsedHero.transform.position);
        Utils.SetActive(heroDeath, true);

        var ingameSceneUI = Manager.Instance.UI.CurrentSceneUI as UI_IngameScene;
        ingameSceneUI.FinishIngame();
    }

    private async UniTaskVoid _ClearWave()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_CLEAR_INGAME));

        Manager.Instance.Ingame.ReturnUsedGold();
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_GET_DROP_ITEM));

        Manager.Instance.Ingame.GetGoldAtOnce();
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_CLEAR_CHAPTER_TIME));

        Manager.Instance.UI.ShowPopupUI<UI_ClearChapter>(Define.RESOURCE_UI_CLEAR_CHAPTER, (clearChapterUI) =>
        {
            clearChapterUI.SetClearChapterUI();
        });
        Manager.Instance.Ingame.ReturnUsedMonster();
    }
}
