using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Weapon
{
    public event Action FinishAttackHandler;

    private const float ANGLE_360 = 360f;
    private const float CHECK_DIRECTION = 0f;

    protected override void Awake()
    {
        base.Awake();

        _weaponName = Define.WEAPON_FIREBALL;
    }

    public override void Init(Vector3 initPos, Vector3 targetPos)
    {
        base.Init(initPos, targetPos);

        transform.localScale = Vector3.zero;
        _collider.enabled = false;
        _RotateFireball().Forget();
    }

    private async UniTaskVoid _RotateFireball()
    {
        var hero = Manager.Instance.Ingame.UsedHero;

        // Fade-In
        var time = ZERO_SECOND;
        while (time < ONE_SECOND)
        {
            time += Time.deltaTime;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;

            transform.position = Vector3.Lerp(hero.transform.position, hero.transform.position + _targetPos, time);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime), delayTiming: PlayerLoopTiming.LastUpdate);
        }
        _collider.enabled = true;

        // Rotate
        time = ZERO_SECOND;
        var angle = Vector3.Angle(Vector3.up, (transform.position - hero.transform.position).normalized);
        if (_IsRightSide(transform.position.x - hero.transform.position.x))
            angle = ANGLE_360 - angle;
        while (time < _effectTime)
        {
            time += Time.fixedDeltaTime;
            if (time >= _effectTime)
                time = _effectTime;

            angle += _speed * Time.fixedDeltaTime;
            var rotateVec = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;
            transform.position = hero.transform.position + rotateVec.normalized * _effectRange;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedDeltaTime), delayTiming: PlayerLoopTiming.FixedUpdate);
        }
        _collider.enabled = false;

        // Fade-Out
        time = ONE_SECOND;
        var lastPos = transform.position;
        while (time > ZERO_SECOND)
        {
            time -= Time.deltaTime;
            if (time <= ZERO_SECOND)
                time = ZERO_SECOND;

            transform.position = Vector3.Lerp(hero.transform.position, lastPos, time);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime), delayTiming: PlayerLoopTiming.LastUpdate);
        }
        FinishAttackHandler?.Invoke();
    }

    private bool _IsRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }
}
