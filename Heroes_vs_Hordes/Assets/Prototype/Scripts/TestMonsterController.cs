namespace ProtoType
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TestMonsterController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 3f;

        public event Action OnDieHandler;
        public event Action<GameObject> ReturnMonsterHandler;

        private Rigidbody2D _rigid;

        public Transform Target { get; set; }

        private const float REVERSE_ANGLE = -1f;
        private const float CHECK_DIRECTION = 0f;
        private const string TAG_WEAPON = "Weapon";

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var monsterToHeroVec = Target.position - transform.position;
            var monsterToHeroNormalVec = new Vector2(monsterToHeroVec.x, monsterToHeroVec.y).normalized;
            var lookAngle = Vector2.Angle(Vector2.up, monsterToHeroNormalVec);
            if (_IsLocatedTargetRightSide(monsterToHeroNormalVec.x))
                lookAngle *= REVERSE_ANGLE;
            _rigid.rotation = lookAngle;

            var moveVec = monsterToHeroNormalVec * _moveSpeed * Time.fixedDeltaTime;
            _rigid.MovePosition(_rigid.position + moveVec);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG_WEAPON))
            {
                ReturnMonsterHandler?.Invoke(gameObject);
                Utils.SetActive(collision.gameObject, false);
            }
        }

        private void OnDisable()
        {
            OnDieHandler?.Invoke();
        }

        private bool _IsLocatedTargetRightSide(float value)
        {
            return value >= CHECK_DIRECTION;
        }
    }
}
