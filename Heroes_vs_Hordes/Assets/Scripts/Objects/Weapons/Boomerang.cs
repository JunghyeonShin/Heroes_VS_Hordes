using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Boomerang : Weapon
{
    public event Action FinishAttackHandler;

    private const float MOVE_TIME = 0.4f;
    private const float WAIT_TIME = 1f - (MOVE_TIME * 2f);

    protected override void Awake()
    {
        base.Awake();

        _weaponName = Define.WEAPON_BOOMERANG;
    }

    protected override void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, 0f, _speed) * Time.fixedDeltaTime);
    }

    public override void Init(Vector3 initPos, Vector3 targetPos)
    {
        base.Init(initPos, targetPos);

        transform.Rotate(Vector3.zero);
        _MoveBoomerang().Forget();
    }

    private async UniTaskVoid _MoveBoomerang()
    {
        // Move to target
        var hero = Manager.Instance.Ingame.UsedHero;
        var time = ZERO_SECOND;
        var maxTime = _effectTime * MOVE_TIME;
        var startPos = hero.transform.position;
        var lastPos = startPos + _targetPos;
        while (time < maxTime)
        {
            time += Time.deltaTime;
            if (time >= maxTime)
                time = maxTime;

            transform.position = Vector3.Lerp(startPos, lastPos, time / maxTime);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime), delayTiming: PlayerLoopTiming.LastUpdate);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(_effectTime * WAIT_TIME));

        // Move to hero
        time = maxTime;
        lastPos = transform.position;
        while (time > ZERO_SECOND)
        {
            time -= Time.deltaTime;
            if (time <= ZERO_SECOND)
                time = ZERO_SECOND;

            transform.position = Vector3.Lerp(hero.transform.position, lastPos, time / maxTime);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime), delayTiming: PlayerLoopTiming.LastUpdate);
        }
        FinishAttackHandler?.Invoke();
    }
}
