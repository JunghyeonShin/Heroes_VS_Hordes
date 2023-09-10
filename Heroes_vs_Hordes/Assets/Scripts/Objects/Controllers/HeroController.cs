using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroController : MonoBehaviour
{
    [SerializeField] private Transform _mapCollisionArea;
    [SerializeField] private float _moveSpeed = 10f;

    private Rigidbody2D _rigid;

    public Vector2 InputVec { get; set; }

    private const float ANGLE_180 = 180f;
    private const float CHECK_DIRECTION = 0f;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var moveVec = InputVec * _moveSpeed * Time.fixedDeltaTime;
        _rigid.MovePosition(_rigid.position + moveVec);

        var angle = Vector2.Angle(Vector2.up, InputVec.normalized);
        if (InputVec.Equals(Vector2.zero))
            _rigid.rotation = angle;
        else
        {
            if (_IsRightSide(InputVec.x))
                _rigid.rotation = ANGLE_180 - angle;
            else
                _rigid.rotation = ANGLE_180 + angle;
        }
    }

    private void LateUpdate()
    {
        _mapCollisionArea.position = transform.position;
    }

    public void InitTransform()
    {
        transform.position = Vector3.zero;
        _mapCollisionArea.position = Vector3.zero;
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
