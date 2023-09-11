using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rigid;
    private Vector2 _moveVec;

    private Action<GameObject> _returnObjectHandler;
    private Vector3 _targetPos;
    private float _moveSpeed;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        var aimingVec = _targetPos - transform.position;
        var aimingNormalVec = new Vector2(aimingVec.x, aimingVec.y).normalized;
        _moveVec = aimingNormalVec * _moveSpeed * Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        _rigid.MovePosition(_rigid.position + _moveVec);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_MONSTER))
        {
            _returnObjectHandler?.Invoke(gameObject);
            Utils.SetActive(gameObject, false);
        }
    }

    public void Init(Vector3 initPos, Vector3 targetPos, float moveSpeed, Action<GameObject> returnObjectCallback)
    {
        transform.position = initPos;
        _targetPos = targetPos;
        _moveSpeed = moveSpeed;

        _returnObjectHandler -= returnObjectCallback;
        _returnObjectHandler += returnObjectCallback;
    }
}
