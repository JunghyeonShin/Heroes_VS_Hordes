using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpider_IdleState : BossMonsterState
{
    private Dictionary<EStateTypes, float> _selectStateChanceDic = new Dictionary<EStateTypes, float>();
    private float _totalStateChance;

    private const float DELAY_SELECT_STATE_TIME = 2f;
    private const float SELECT_MOVE_STATE_CHANCE = 0.5f;
    private const float SELECT_ATTACK_STATE_CHANCE = 0.5f;

    public BossSpider_IdleState(GameObject owner) : base(owner)
    {

    }

    public override void InitState()
    {
        base.InitState();

        _selectStateChanceDic.Add(EStateTypes.Move, SELECT_MOVE_STATE_CHANCE);
        _selectStateChanceDic.Add(EStateTypes.Attack, SELECT_ATTACK_STATE_CHANCE);

        foreach (var selectStateChance in _selectStateChanceDic)
            _totalStateChance += selectStateChance.Value;
    }

    public override void EnterState()
    {
        if (false == _owner.activeSelf)
            return;

        _SelectNextState().Forget();
    }

    public override void ExitState()
    {

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

    private async UniTaskVoid _SelectNextState()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_SELECT_STATE_TIME));

        if (false == _owner.activeSelf)
            return;

        var randomState = UnityEngine.Random.value * _totalStateChance;
        foreach (var selectStateChance in _selectStateChanceDic)
        {
            if (randomState <= selectStateChance.Value)
            {
                _bossMonster.ChangeState(selectStateChance.Key);
                return;
            }
            else
                randomState -= selectStateChance.Value;
        }
    }
}
