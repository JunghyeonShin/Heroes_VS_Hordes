using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : Weapon
{
    [SerializeField] private GameObject _arrowhead;

    private bool _reflectHorizontal;
    private bool _reflectVertical;

    private const float REVERSE_ANGLE = -1f;
    private const float CHECK_DIRECTION = 0f;
    private const float MIN_WORLD_TO_VIEWPORT_POINT = 0f;
    private const float MAX_WORLD_TO_VIEWPORT_POINT = 1f;

    protected override void Awake()
    {
        base.Awake();
        _weaponName = Define.WEAPON_CROSSBOW;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        var moveVec = new Vector2(_targetPos.x, _targetPos.y) * _speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + moveVec);
    }

    protected override void Update()
    {
        base.Update();

        var pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x <= MIN_WORLD_TO_VIEWPORT_POINT || pos.x >= MAX_WORLD_TO_VIEWPORT_POINT)
        {
            if (_reflectHorizontal)
                return;

            _reflectHorizontal = true;
            _targetPos = Vector3.Reflect(_targetPos, Vector3.right);
            _RotateArrowheadToTarget();
        }
        else
            _reflectHorizontal = false;

        if (pos.y <= MIN_WORLD_TO_VIEWPORT_POINT || pos.y >= MAX_WORLD_TO_VIEWPORT_POINT)
        {
            if (_reflectVertical)
                return;

            _reflectVertical = true;
            _targetPos = Vector3.Reflect(_targetPos, Vector3.up);
            _RotateArrowheadToTarget();
        }
        else
            _reflectVertical = false;
    }

    public override void Init(Vector3 initPos, Vector3 targetPos)
    {
        base.Init(initPos, targetPos);
        _RotateArrowheadToTarget();
    }

    private void _RotateArrowheadToTarget()
    {
        var angle = Vector3.Angle(Vector3.up, _targetPos);
        if (_IsRightSide(_targetPos.x))
            angle *= REVERSE_ANGLE;
        _arrowhead.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    private bool _IsRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }
}
