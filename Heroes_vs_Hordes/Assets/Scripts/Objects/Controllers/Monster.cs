using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;

    private Rigidbody2D _rigid;
    private Action _dieHandler;

    public Transform Target { get; set; }

    private const float REVERSE_ANGLE = -1f;
    private const float CHECK_DIRECTION = 0f;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _dieHandler -= Manager.Instance.Ingame.OnDeadMonster;
        _dieHandler += Manager.Instance.Ingame.OnDeadMonster;
    }

    private void OnDisable()
    {
        _dieHandler -= Manager.Instance.Ingame.OnDeadMonster;
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

    public void OnDamaged(float damage)
    {
        var expGemGO = Manager.Instance.Object.GetExpGem();
        var expGem = Utils.GetOrAddComponent<ExpGem>(expGemGO);
        expGem.InitTransform(transform.position);
        Utils.SetActive(expGemGO, true);

        _dieHandler?.Invoke();
        Utils.SetActive(gameObject, false);
    }

    private bool _IsLocatedTargetRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }
}
