namespace ProtoType
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TestMonsterController : MonoBehaviour
    {
        [SerializeField] Transform _target;

        private Rigidbody2D _rigid;

        private const float REVERSE_ANGLE = -1f;
        private const float CHECK_DIRECTION = 0f;
        private const string TAG_WEAPON = "Weapon";

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var monsterToHeroVec = _target.position - transform.position;
            var lookAngle = Vector2.Angle(Vector2.up, new Vector2(monsterToHeroVec.x, monsterToHeroVec.y).normalized);
            if (_IsLocatedTargetRightSide(monsterToHeroVec.x))
                lookAngle *= REVERSE_ANGLE;
            _rigid.rotation = lookAngle;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG_WEAPON))
                collision.gameObject.SetActive(false);
        }

        private bool _IsLocatedTargetRightSide(float value)
        {
            return value >= CHECK_DIRECTION;
        }
    }
}
