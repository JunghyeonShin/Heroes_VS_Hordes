using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossMonster : Monster
{
    protected FiniteStateMachine _bossMonsterFSM;

    public Vector3 BossMapPosition { get; set; }

    private const float INIT_BOSS_MONSTER_HEALTH_VALUE = 1f;
    private const float INIT_ROTATION_ANGLE = 180f;
    private const float RANDOM_POSITION_X = 9.5f;
    private const float MIN_RANDOM_POSITION_Y = -11f;
    private const float MAX_RANDOM_POSITION_Y = 5.5f;

    private readonly Vector3 INIT_POSITION = new Vector3(0f, 11f, 0f);

    protected override void OnEnable()
    {
        if (null == Target)
            return;

        _rigidbody.position = Target.transform.position + INIT_POSITION;
        _rigidbody.rotation = INIT_ROTATION_ANGLE;

        Manager.Instance.Ingame.SetBossMonsterHealth(INIT_BOSS_MONSTER_HEALTH_VALUE);
    }

    public override void OnDamage(float damage)
    {
        if (_health <= ZERO_HEALTH)
            return;

        _health -= damage;
        if (_health <= ZERO_HEALTH)
            _health = ZERO_HEALTH;
        Manager.Instance.Ingame.SetBossMonsterHealth(_health / _totalHealth);
        if (_health <= ZERO_HEALTH)
            ChangeState(EStateTypes.Dead);
    }

    public override void ReturnMonster()
    {
        Manager.Instance.Object.ReturnBossMonster(_monsterName);
    }

    public void ChangeState(EStateTypes state)
    {
        _bossMonsterFSM.ChangeState(state);
    }

    public Vector3 GetRandomPosition()
    {
        var randomPosX = UnityEngine.Random.Range(-RANDOM_POSITION_X, RANDOM_POSITION_X);
        var randomPosY = UnityEngine.Random.Range(MIN_RANDOM_POSITION_Y, MAX_RANDOM_POSITION_Y);
        return new Vector3(BossMapPosition.x + randomPosX, BossMapPosition.y + randomPosY, 0f);
    }
}
