using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBossSpiderAttackTypes
{
    HatchSpider,
    SprayWeb,
}

public interface IBossSpiderAttack
{
    public void Init(GameObject owner);
    public void Attack(Vector3 targetPos);
}

public abstract class BossSpiderAttack : IBossSpiderAttack
{
    protected GameObject _owner;
    protected Vector3 _targetPos;

    public virtual void Init(GameObject owner)
    {
        _owner = owner;
    }

    public virtual void Attack(Vector3 targetPos)
    {
        _targetPos = targetPos;
    }
}
