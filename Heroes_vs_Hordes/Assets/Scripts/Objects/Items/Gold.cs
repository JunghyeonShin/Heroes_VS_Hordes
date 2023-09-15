using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : DropItem
{
    public override void InitTransform(Vector3 initPos)
    {
        base.InitTransform(initPos);
        Manager.Instance.Ingame.EnqueueUsedGold(this);
        GiveEffect(Manager.Instance.Ingame.UsedHero, true);
    }
    public override void ReturnDropItem()
    {
        Manager.Instance.Object.ReturnDropItem(Define.RESOURCE_GOLD, gameObject);
    }

    protected override void _DeliverEffect(Hero targetHero, bool getEffect)
    {
        if (getEffect)
            Manager.Instance.Ingame.GetGold(Define.INCREASE_GOLD_VALUE);
        ReturnDropItem();
    }
}
