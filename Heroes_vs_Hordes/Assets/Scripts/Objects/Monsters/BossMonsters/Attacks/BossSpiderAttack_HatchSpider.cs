using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpiderAttack_HatchSpider : BossSpiderAttack
{
    private ObjectPool _webBundlePool = new ObjectPool();
    private Queue<GameObject> _usedWebBundleQueue = new Queue<GameObject>();

    private const float DELAY_SPAWN_SPIDER_TIME = 0.7f;
    private const float DELAY_RETURN_WEB_BUNDLE = 1f;
    private const int CREATE_WEB_BUNDLE_COUNT = 10;
    private const int CREATE_SPIDER_COUNT = 10;
    private const int EMPTY_USED_WEB_BUNDLE = 0;
    private const string ANIMATOR_BOOL_HATCH = "Hatch";

    public override void Init(GameObject owner)
    {
        base.Init(owner);

        Manager.Instance.Resource.LoadAsync<GameObject>(Define.RESOURCE_BOSS_SPIDER_WEB_BUNDLE, (webBundle) =>
        {
            _webBundlePool.InitPool(webBundle, Manager.Instance.Object.RootObject, CREATE_WEB_BUNDLE_COUNT);
        });
    }

    public override void Attack(Vector3 targetPos)
    {
        _Hatch(targetPos).Forget();
    }

    public override void ReturnObject()
    {
        _ReturnAllWebBundle();
    }

    private async UniTaskVoid _Hatch(Vector3 targetPos)
    {
        var webBundleGO = _webBundlePool.GetObject();
        _usedWebBundleQueue.Enqueue(webBundleGO);
        webBundleGO.transform.position = targetPos;
        Utils.SetActive(webBundleGO, true);
        var webBundleAnimator = Utils.GetOrAddComponent<Animator>(webBundleGO);
        webBundleAnimator.SetBool(ANIMATOR_BOOL_HATCH, true);
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_SPAWN_SPIDER_TIME));

        for (int ii = 0; ii < CREATE_SPIDER_COUNT; ++ii)
        {
            if (false == _owner.activeSelf)
                return;

            Manager.Instance.Object.GetMonster(Define.RESOURCE_MONSTER_NORMAL_SPIDER, (normalSpiderGO) =>
            {
                var normalSpider = Utils.GetOrAddComponent<Normal_Spider>(normalSpiderGO);
                normalSpider.Target = Manager.Instance.Ingame.UsedHero.transform;
                normalSpider.InitMonsterAbilities();
                normalSpider.transform.position = targetPos;

                Manager.Instance.Ingame.EnqueueUsedMonster(normalSpider);

                Utils.SetActive(normalSpiderGO, true);
            });
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_RETURN_WEB_BUNDLE));

        _webBundlePool.ReturnObject(webBundleGO);
    }

    private void _ReturnAllWebBundle()
    {
        while (_usedWebBundleQueue.Count > EMPTY_USED_WEB_BUNDLE)
        {
            var webBundleGO = _usedWebBundleQueue.Dequeue();
            if (webBundleGO.activeSelf)
                _webBundlePool.ReturnObject(webBundleGO);
        }
    }
}
