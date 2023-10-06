using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpiderAttack_SprayWeb : BossSpiderAttack
{
    private ObjectPool _spiderWebPool = new ObjectPool();

    private const int CREATE_SPIDER_WEB = 10;

    public override void Init(GameObject owner)
    {
        base.Init(owner);

        Manager.Instance.Resource.LoadAsync<GameObject>(Define.RESOURCE_BOSS_SPIDER_WEB, (spiderWeb) =>
        {
            _spiderWebPool.InitPool(spiderWeb, Manager.Instance.Object.RootObject, CREATE_SPIDER_WEB);
        });
    }

    public override void Attack(Vector3 targetPos)
    {
        base.Attack(targetPos);

        var spiderWebGO = _spiderWebPool.GetObject();
        var spiderWeb = Utils.GetOrAddComponent<Spider_Web>(spiderWebGO);
        spiderWeb.InitTransform(_owner.transform.position, targetPos);
        spiderWeb.ReturnHandler -= _ReturnSpiderWeb;
        spiderWeb.ReturnHandler += _ReturnSpiderWeb;
        Utils.SetActive(spiderWebGO, true);
    }

    private void _ReturnSpiderWeb(GameObject go)
    {
        _spiderWebPool.ReturnObject(go);
    }
}
