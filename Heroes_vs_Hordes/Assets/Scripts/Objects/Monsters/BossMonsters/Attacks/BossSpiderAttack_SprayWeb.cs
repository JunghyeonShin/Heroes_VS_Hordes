using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpiderAttack_SprayWeb : BossSpiderAttack
{
    private ObjectPool _spiderWebPool = new ObjectPool();

    private Queue<GameObject> _usedWpiderWebQueue = new Queue<GameObject>();

    private const int CREATE_SPIDER_WEB = 10;
    private const int EMPTY_SPIDER_WEB = 0;

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
        var spiderWebGO = _spiderWebPool.GetObject();
        _usedWpiderWebQueue.Enqueue(spiderWebGO);
        var spiderWeb = Utils.GetOrAddComponent<SpiderWeb>(spiderWebGO);
        spiderWeb.InitTransform(_owner.transform.position, targetPos);
        spiderWeb.ReturnHandler -= _ReturnSpiderWeb;
        spiderWeb.ReturnHandler += _ReturnSpiderWeb;
        Utils.SetActive(spiderWebGO, true);
    }

    public override void ReturnObject()
    {
        while (_usedWpiderWebQueue.Count > EMPTY_SPIDER_WEB)
        {
            var spiderWebGO = _usedWpiderWebQueue.Dequeue();
            if (spiderWebGO.activeSelf)
                Utils.SetActive(spiderWebGO, false);
        }
    }

    private void _ReturnSpiderWeb(GameObject go)
    {
        _spiderWebPool.ReturnObject(go);
    }
}
