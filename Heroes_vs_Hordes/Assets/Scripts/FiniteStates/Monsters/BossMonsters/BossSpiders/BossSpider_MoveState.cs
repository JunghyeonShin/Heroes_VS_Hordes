using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpider_MoveState : BossMonsterState
{
    private Vector3 _randomPos;
    private Vector2 _moveVec;
    private bool _isMove;

    private const float DISTANCE_OWNER_TO_RANDOM_POSITION = 12.5f;
    private const float DELAY_MOVE_TIME = 0.5f;
    private const float DELAY_IDLE_STATE = 1f;
    private const float CHECK_DIRECTION = 0f;
    private const float ANGLE_360 = 360f;
    private const string MOVE = "Move";

    public BossSpider_MoveState(GameObject owner) : base(owner)
    {

    }

    public override void EnterState()
    {
        _randomPos = _GetRandomPosition();
        _RotateToRandomPosition().Forget();
    }

    public override void ExitState()
    {
        _isMove = false;
        _moveVec = Vector2.zero;
    }

    public override void FixedUpdateState()
    {
        if (false == _isMove)
            return;

        var distance = Vector3.Distance(_owner.transform.position, _randomPos);
        if (distance <= _moveVec.magnitude)
        {
            _ArriveRandomPosition().Forget();
            return;
        }
        else
            _rigidbody.MovePosition(_rigidbody.position + _moveVec);
    }

    public override void UpdateState()
    {

    }

    private Vector3 _GetRandomPosition()
    {
        var randomPosition = _bossMonster.GetRandomPosition();
        var distance = Vector3.Distance(_owner.transform.position, randomPosition);
        if (distance >= DISTANCE_OWNER_TO_RANDOM_POSITION)
            return randomPosition;
        else
            return _GetRandomPosition();
    }

    private async UniTaskVoid _RotateToRandomPosition()
    {
        await UniTask.Yield();

        var aimingVec = _randomPos - _owner.transform.position;
        var angle = Vector3.Angle(Vector3.up, aimingVec);
        if (_IsRightSide(aimingVec.x))
            angle = ANGLE_360 - angle;
        _rigidbody.rotation = angle;
        _MoveToRandomPosition(aimingVec).Forget();
    }

    private bool _IsRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }

    private async UniTaskVoid _MoveToRandomPosition(Vector3 aimingVec)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_MOVE_TIME));

        var aimingNormalVec = new Vector2(aimingVec.x, aimingVec.y).normalized;
        _moveVec = aimingNormalVec * _bossMonster.MoveSpeed * Time.fixedDeltaTime;
        _isMove = true;
        _animator.SetBool(MOVE, _isMove);
    }

    private async UniTaskVoid _ArriveRandomPosition()
    {
        _isMove = false;
        _animator.SetBool(MOVE, _isMove);
        await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedDeltaTime));

        _owner.transform.position = _randomPos;
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_IDLE_STATE));

        _bossMonster.ChangeState(EStateTypes.Idle);
    }
}
