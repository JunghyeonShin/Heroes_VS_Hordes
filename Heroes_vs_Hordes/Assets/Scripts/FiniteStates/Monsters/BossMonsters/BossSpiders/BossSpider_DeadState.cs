using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpider_DeadState : BossMonsterState
{
    private const float DELAY_CHECK_CLEAR_BOSS_WAVE = 1f;
    private const float INIT_EXP_GEM_POS_X = 2f;
    private const float MIN_INIT_EXP_GEM_POS_Y = -0.5f;
    private const float MAX_INIT_EXP_GEM_POS_Y = 1f;
    private const int EMPTY_NORMAL_MONSTER_COUNT = 0;
    private const int MIN_EXP_GEM_COUNT = 30;
    private const int MAX_EXP_GEM_COUNT = 75;

    public BossSpider_DeadState(GameObject owner) : base(owner)
    {

    }

    public override void EnterState()
    {
        if (false == _owner.activeSelf)
            return;

        _ClearBossSpider().Forget();
    }

    public override void ExitState()
    {
        Manager.Instance.Ingame.RemainingMonsterHandler -= _ClearBossWave;
    }

    public override void FixedUpdateState()
    {

    }

    public override void UpdateState()
    {

    }

    public override void ReturnObject()
    {

    }

    private async UniTaskVoid _ClearBossSpider()
    {
        Utils.SetActive(_owner, false);

        var randomExpGemCount = UnityEngine.Random.Range(MIN_EXP_GEM_COUNT, MAX_EXP_GEM_COUNT);
        for (int ii = 0; ii < randomExpGemCount; ++ii)
        {
            Manager.Instance.Object.GetDropItem(Define.RESOURCE_EXP_GEM, (dropItemGO) =>
            {
                var dropItem = Utils.GetOrAddComponent<ExpGem>(dropItemGO);
                var randomPosX = UnityEngine.Random.Range(-INIT_EXP_GEM_POS_X, INIT_EXP_GEM_POS_X);
                var randomPosY = UnityEngine.Random.Range(MIN_INIT_EXP_GEM_POS_Y, MAX_INIT_EXP_GEM_POS_Y);
                var randomPos = new Vector3(randomPosX, randomPosY, 0f);
                dropItem.InitTransform(_owner.transform.position + randomPos);
                Utils.SetActive(dropItemGO, true);
            });
        }
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_CHECK_CLEAR_BOSS_WAVE));

        var ingame = Manager.Instance.Ingame;
        if (ingame.RemainingMonsterCount > EMPTY_NORMAL_MONSTER_COUNT)
        {
            Manager.Instance.Ingame.RemainingMonsterHandler -= _ClearBossWave;
            Manager.Instance.Ingame.RemainingMonsterHandler += _ClearBossWave;
        }
        else
            _ClearBossWave();
    }

    private void _ClearBossWave()
    {
        var ingame = Manager.Instance.Ingame;
        if (ingame.RemainingMonsterCount <= EMPTY_NORMAL_MONSTER_COUNT)
        {
            var ingameSceneUI = Manager.Instance.UI.CurrentSceneUI as UI_IngameScene;
            ingameSceneUI.FinishIngame();
        }
    }
}
