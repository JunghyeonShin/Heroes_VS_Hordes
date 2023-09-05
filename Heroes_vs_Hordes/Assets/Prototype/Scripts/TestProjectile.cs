using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody2D _rigid;
    private Vector2 _moveVec;

    public Transform TargetMonster { get; set; }

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        var projectileToMonsterVec = (TargetMonster.position - transform.position);
        var projectileToMonsterNormalVec = new Vector2(projectileToMonsterVec.x, projectileToMonsterVec.y).normalized;
        _moveVec = projectileToMonsterNormalVec * _moveSpeed * Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        _rigid.MovePosition(_rigid.position + _moveVec);
    }
}
