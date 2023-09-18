using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : MonoBehaviour
{
    public event Action<GameObject> OnDieHandler;

    [SerializeField] private Transform _target;

    private Rigidbody2D _rigid;

    private const float REVERSE_ANGLE = -1f;
    private const float CHECK_DIRECTION = 0f;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var monsterToHeroVec = _target.position - transform.position;
        var monsterToHeroNormalVec = new Vector2(monsterToHeroVec.x, monsterToHeroVec.y).normalized;
        var lookAngle = Vector2.Angle(Vector2.up, monsterToHeroNormalVec);
        if (_IsLocatedTargetRightSide(monsterToHeroNormalVec.x))
            lookAngle *= REVERSE_ANGLE;
        _rigid.rotation = lookAngle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void OnDisable()
    {

    }

    public void OnDamaged(float damage)
    {
        OnDieHandler?.Invoke(gameObject);
    }

    private bool _IsLocatedTargetRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }
}
