namespace ProtoType
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class TestHeroController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;

        private Rigidbody2D _rigid;
        private Vector2 _inputVec;

        private const float ANGLE_180 = 180f;
        private const float CHECK_DIRECTION = 0f;

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var moveVec = _inputVec * _moveSpeed * Time.fixedDeltaTime;
            _rigid.MovePosition(_rigid.position + moveVec);

            var angle = Vector2.Angle(Vector2.up, _inputVec.normalized);
            if (_inputVec.Equals(Vector2.zero))
                _rigid.rotation = angle;
            else
            {
                if (_IsRightSide(_inputVec.x))
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
            _inputVec = value.Get<Vector2>();
        }
    }
}
