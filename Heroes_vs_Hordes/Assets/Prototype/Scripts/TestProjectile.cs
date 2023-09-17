using ProtoType;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    private Rigidbody2D _rigid;
    private Vector2 _moveVec;

    private Vector3 _targetPos;
    private float _moveSpeed;
    private float _attack;
    private bool _isCritical;
    private Action<GameObject> _returnObjectHandler;

    private const float MIN_DAMAGE_TEXT_POSITION_X = -1f;
    private const float MAX_DAMAGE_TEXT_POSITION_X = 1f;
    private const float DAMAGE_TEXT_POSITION_Y = 1f;
    private const float TWO_MULTIPLES_VALUE = 2f;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        var projectileToMonsterVec = _targetPos - transform.position;
        var projectileToMonsterNormalVec = new Vector2(projectileToMonsterVec.x, projectileToMonsterVec.y).normalized;
        _moveVec = projectileToMonsterNormalVec * _moveSpeed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_MONSTER))
        {
            var randomPos = new Vector3(UnityEngine.Random.Range(MIN_DAMAGE_TEXT_POSITION_X, MAX_DAMAGE_TEXT_POSITION_X), DAMAGE_TEXT_POSITION_Y, 0f);
            var initDamageTextPos = collision.transform.position + randomPos;
            var damageTextGO = Manager.Instance.Object.GetDamageText();
            var damageText = Utils.GetOrAddComponent<DamageText>(damageTextGO);
            damageText.FloatDamageText(initDamageTextPos, _attack, _isCritical);

            var testMonster = Utils.GetOrAddComponent<TestMonster>(collision.gameObject);
            testMonster.OnDamaged(_attack);

            _returnObjectHandler?.Invoke(gameObject);
        }
    }

    private void FixedUpdate()
    {
        _rigid.MovePosition(_rigid.position + _moveVec);
    }

    public void Init(Vector3 initPos, Vector3 targetPos, float moveSpeed, bool isCritical, float attack, Action<GameObject> returnObjectCallback)
    {
        transform.position = initPos;
        _targetPos = targetPos;
        _moveSpeed = moveSpeed;
        _isCritical = isCritical;
        if (_isCritical)
            _attack = attack * TWO_MULTIPLES_VALUE;
        else
            _attack = attack;

        _returnObjectHandler -= returnObjectCallback;
        _returnObjectHandler += returnObjectCallback;
    }
}
