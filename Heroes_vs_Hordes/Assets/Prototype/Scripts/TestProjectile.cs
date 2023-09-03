using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody2D _rigid;

    public Transform TargetMonster { get; set; }

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (null != TargetMonster)
        {
            var projectileToMonsterVec = (TargetMonster.position - transform.position);
            var projectileToMonsterNormalVec = new Vector2(projectileToMonsterVec.x, projectileToMonsterVec.y).normalized;
            var moveVec = projectileToMonsterNormalVec * _moveSpeed * Time.fixedDeltaTime;
            _rigid.MovePosition(_rigid.position + moveVec);
        }
    }
}
