using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossMonster : Monster
{
    private const float INIT_ROTATION_ANGLE = 180f;

    private readonly Vector3 INIT_POSITION = new Vector3(0f, 11f, 0f);

    public override void OnDamage(float damage)
    {
        if (_health <= ZERO_HEALTH)
            return;

        _health -= damage;
        if (_health <= ZERO_HEALTH)
        {

        }
    }

    public override void ReturnMonster()
    {
        Manager.Instance.Object.ReturnBossMonster(_monsterName);
    }

    public void InitTransform()
    {
        _rigidbody.position = Target.transform.position + INIT_POSITION;
        _rigidbody.rotation = INIT_ROTATION_ANGLE;
    }
}
