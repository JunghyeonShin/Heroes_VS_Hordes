using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpGem : DropItem
{
    public override void InitTransform(Vector3 initPos)
    {
        base.InitTransform(initPos);
        Manager.Instance.Ingame.EnqueueUsedExpGem(this);
    }

    public override void GiveEffect(Hero targetHero, bool getEffect)
    {
        _collider.enabled = false;
        base.GiveEffect(targetHero, getEffect);
    }

    protected override void _DeliverEffect(Hero targetHero, bool getEffect)
    {
        if (getEffect)
            targetHero.GetExp(Define.INCREASE_HERO_EXP_VALUE);
        Manager.Instance.Object.ReturnExpGem(gameObject);
    }
}
