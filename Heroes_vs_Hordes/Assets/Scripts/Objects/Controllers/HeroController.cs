using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroController : MonoBehaviour
{
    private Hero _hero;
    private Rigidbody2D _rigid;

    public Vector2 InputVec { get; set; }

    private const float ANGLE_180 = 180f;
    private const float CHECK_DIRECTION = 0f;
    private const float INIT_ROTATION_VALUE = 0f;

    private void Awake()
    {
        _hero = GetComponent<Hero>();
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        transform.position = Vector3.zero;
        _rigid.rotation = INIT_ROTATION_VALUE;
    }

    private void FixedUpdate()
    {
        var moveVec = InputVec * _hero.MoveSpeed * Time.fixedDeltaTime;
        _rigid.MovePosition(_rigid.position + moveVec);

        var angle = Vector2.Angle(Vector2.up, InputVec.normalized);
        if (InputVec.Equals(Vector2.zero))
            _rigid.rotation = _rigid.rotation;
        else
        {
            if (_IsRightSide(InputVec.x))
                _rigid.rotation = ANGLE_180 - angle;
            else
                _rigid.rotation = ANGLE_180 + angle;
        }
    }

    private bool _IsRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }

    private void OnMove(InputValue value)
    {
        InputVec = value.Get<Vector2>();
    }
}
