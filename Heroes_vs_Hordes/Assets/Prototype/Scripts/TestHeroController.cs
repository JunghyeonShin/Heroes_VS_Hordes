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

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var moveVec = _inputVec * _moveSpeed * Time.fixedDeltaTime;
            _rigid.MovePosition(_rigid.position + moveVec);
        }

        private void OnMove(InputValue value)
        {
            _inputVec = value.Get<Vector2>();
        }
    }
}
