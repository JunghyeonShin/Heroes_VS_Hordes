using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : MonoBehaviour
{
    [SerializeField] Transform _target;

    private Rigidbody2D _rigid;

    private const float REVERSE_ANGLE = -1f;
    private const float CHECK_DIRECTION = 0f;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var enemyToHeroVec = _target.position - transform.position;
        var lookAngle = Vector2.Angle(Vector2.up, new Vector2(enemyToHeroVec.x, enemyToHeroVec.y).normalized);
        if (_IsLocatedTargetRightSide(enemyToHeroVec.x))
            lookAngle *= REVERSE_ANGLE;
        _rigid.rotation = lookAngle;
    }

    private bool _IsLocatedTargetRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }
}
