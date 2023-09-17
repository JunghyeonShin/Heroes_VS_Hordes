using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneMage : Hero
{
    private ObjectPool _projectilePool = new ObjectPool();

    private const float ATTACK_TIME = 0.06f;
    private const float DEFAULT_DETECT_BOX_ANGLE = 0f;
    private const float MIN_DISTANCE = 987654321f;
    private const int CREATE_PROJECTILE_COUNT = 50;
    private const string NAME_ROOT_PROJECTILE = "[ROOT_PROJECTILE]";

    private readonly Vector2 OVERLAP_SIZE = new Vector2(22.5f, 40f);
    private readonly Vector3 INIT_PROJECTILE_POSITION = new Vector3(1f, -0.276f, 0f);

    protected override void Awake()
    {
        base.Awake();
        _InitProjectile();
    }

    protected override void _DetectMonster()
    {
        if (false == _detectMonster)
            _DetectMonsterAsync().Forget();
    }

    protected override void _AttackMonster(Vector3 targetPos)
    {
        _AttackMonsterAsync(targetPos).Forget();
    }

    private async UniTaskVoid _DetectMonsterAsync()
    {
        _detectMonster = true;
        if (_attackMonster)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));
            _attackMonster = false;
        }

        var layerMask = 1 << LayerMask.NameToLayer(Define.LAYER_MONSTER);
        var monsters = Physics2D.OverlapBoxAll(transform.position, OVERLAP_SIZE, DEFAULT_DETECT_BOX_ANGLE, layerMask);
        if (monsters.Length > 0)
        {
            var minDistance = MIN_DISTANCE;
            var targetPos = Vector3.zero;
            foreach (var monster in monsters)
            {
                var distance = Vector3.Distance(transform.position, monster.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    targetPos = monster.transform.position;
                }
            }
            _AttackMonster(targetPos);
        }
        _detectMonster = false;
    }

    private async UniTaskVoid _AttackMonsterAsync(Vector3 targetPos)
    {
        _attackMonster = true;
        _animator.SetTrigger(Define.ANIMATOR_TRIGGER_ATTACK);
        await UniTask.Delay(TimeSpan.FromSeconds(ATTACK_TIME));

        var initProjectilePos = transform.TransformPoint(INIT_PROJECTILE_POSITION);
        var projectileGO = _GetProjectile();
        var projectile = Utils.GetOrAddComponent<ArcaneMage_Projectile>(projectileGO);
        projectile.Init(initProjectilePos, targetPos, ProjectileSpeed, _IsCritical(), _attack, _ReturnProjectile);
        Utils.SetActive(projectileGO, true);
    }

    private void _InitProjectile()
    {
        var rootProjectile = new GameObject(NAME_ROOT_PROJECTILE);
        rootProjectile.transform.SetParent(transform);

        Manager.Instance.Resource.LoadAsync<GameObject>(Define.RESOURCE_WEAPON_ARCANE_MAGE_PROJECTILE, (projectile) =>
        {
            _projectilePool.InitPool(projectile, rootProjectile, CREATE_PROJECTILE_COUNT);
        });
    }

    private GameObject _GetProjectile()
    {
        return _projectilePool.GetObject();
    }

    private void _ReturnProjectile(GameObject projectile)
    {
        _projectilePool.ReturnObject(projectile);
    }
}
