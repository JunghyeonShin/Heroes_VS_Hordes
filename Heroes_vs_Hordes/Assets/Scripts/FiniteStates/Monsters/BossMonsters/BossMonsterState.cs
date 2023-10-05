using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossMonsterState : BaseState
{
    protected BossMonster _bossMonster;
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;

    public BossMonsterState(GameObject owner) : base(owner)
    {

    }

    public override void InitState()
    {
        _bossMonster = Utils.GetOrAddComponent<BossMonster>(_owner);
        _rigidbody = Utils.GetOrAddComponent<Rigidbody2D>(_owner);
        _animator = Utils.GetOrAddComponent<Animator>(_owner);
    }
}
