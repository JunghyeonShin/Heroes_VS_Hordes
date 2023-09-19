using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    [SerializeField] private TestHeroController _testHeroController;

    private Rigidbody2D _rigid;
    private Vector2 _moveVec;

    private Vector3 _targetPos;
    private float _moveSpeed;
    private float _attack;
    private float _penetraitCount;
    private Action<GameObject> _returnObjectHandler;

    private const float MIN_DAMAGE_TEXT_POSITION_X = -1f;
    private const float MAX_DAMAGE_TEXT_POSITION_X = 1f;
    private const float DAMAGE_TEXT_POSITION_Y = 1f;
    private const float TWO_MULTIPLES_VALUE = 2f;
    private const float END_LIFE = 0f;

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
            var attack = _attack;
            var isCritical = _testHeroController.IsCritical();
            if (isCritical)
                attack = _attack * TWO_MULTIPLES_VALUE;
            damageText.FloatDamageText(initDamageTextPos, attack, isCritical);
            Utils.SetActive(damageTextGO, true);

            var testMonster = Utils.GetOrAddComponent<TestMonster>(collision.gameObject);
            testMonster.OnDamaged(attack);

            --_penetraitCount;
            if (_penetraitCount <= END_LIFE)
                _returnObjectHandler?.Invoke(gameObject);
        }
    }

    private void FixedUpdate()
    {
        _rigid.MovePosition(_rigid.position + _moveVec);
    }

    public void Init(Vector3 initPos, Vector3 targetPos, float attack, float moveSpeed, float penetraitCount, Action<GameObject> returnObjectCallback)
    {
        transform.position = initPos;
        _targetPos = targetPos;
        _attack = attack;
        _moveSpeed = moveSpeed;
        _penetraitCount = penetraitCount;

        _returnObjectHandler -= returnObjectCallback;
        _returnObjectHandler += returnObjectCallback;
    }
}
