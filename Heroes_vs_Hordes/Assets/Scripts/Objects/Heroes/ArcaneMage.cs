using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneMage : Hero
{
    private ObjectPool _projectilePool = new ObjectPool();
    private Queue<GameObject> _usedProjectileQueue = new Queue<GameObject>();

    private float _projectileCount;

    private const float DELAY_CREATE_PROJECTILE_TIME = 0.06f;
    private const float MIN_DISTANCE = 987654321f;
    private const int CREATE_PROJECTILE_COUNT = 50;
    private const string NAME_ROOT_PROJECTILE = "[ROOT_PROJECTILE]";

    private float[] _minDistancesToMonster = new float[] { MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE };
    private Vector3[] _targetMonsterPositions = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

    private readonly Vector2 OVERLAP_SIZE = new Vector2(22.5f, 40f);
    private readonly Vector3[] INIT_PROJECTILE_POSITIONS = new Vector3[]
    {
            new Vector3(1f, -0.276f, 0f),
            new Vector3(0.8f, -0.75f, 0f),
            new Vector3(0.388f, -0.408f, 0f),
            new Vector3(0.233f, -0.872f, 0f),
            new Vector3(-0.19f, -0.201f, 0f),
            new Vector3(-0.334f, -0.647f, 0f),
            new Vector3(-0.714f, -0.342f, 0f),
            new Vector3(-0.867f, -0.872f, 0f)
    };

    protected override void Awake()
    {
        base.Awake();
        _heroName = Define.RESOURCE_HERO_ARCANE_MAGE;
        _heroWeaponName = Define.WEAPON_ARCANE_MAGE_WAND;
        _InitProjectile();
    }

    public override void SetAbilities()
    {
        base.SetAbilities();
        var weaponLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(_heroWeaponName);
        _projectileCount = WeaponAbility.GetWeaponProjectileCount(_heroWeaponName, weaponLevel);
    }

    public override void ReturnAbilities()
    {
        base.ReturnAbilities();
        _ReturnAllProjectile();
    }

    protected override void _DetectMonster()
    {
        if (false == _detectMonster)
            _DetectMonsterAsync().Forget();
    }

    protected override void _AttackMonster()
    {
        _AttackMonsterAsync().Forget();
    }

    private async UniTaskVoid _DetectMonsterAsync()
    {
        _detectMonster = true;
        if (_attackMonster)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));
            _attackMonster = false;
        }

        var monsters = Physics2D.OverlapBoxAll(transform.position, OVERLAP_SIZE, DEFAULT_DETECT_BOX_ANGLE, Define.LAYER_MASK_MONSTER);
        if (monsters.Length > 0)
        {
            _ResetDetectContainer();
            foreach (var monster in monsters)
            {
                var distance = Vector3.Distance(transform.position, monster.transform.position);
                for (int ii = 0; ii < _minDistancesToMonster.Length; ++ii)
                {
                    if (ii >= _projectileCount)
                        break;

                    if (MIN_DISTANCE == _minDistancesToMonster[ii])
                    {
                        _minDistancesToMonster[ii] = distance;
                        _targetMonsterPositions[ii] = monster.transform.position;
                        break;
                    }

                    if (distance < _minDistancesToMonster[ii])
                    {
                        var index = Mathf.FloorToInt(_projectileCount - 1f);
                        for (int jj = index; jj > 0 && jj >= ii; --jj)
                        {
                            _minDistancesToMonster[jj] = _minDistancesToMonster[jj - 1];
                            _targetMonsterPositions[jj] = _targetMonsterPositions[jj - 1];
                        }
                        _minDistancesToMonster[ii] = distance;
                        _targetMonsterPositions[ii] = monster.transform.position;
                        break;
                    }
                }
            }
            _AttackMonster();
        }
        _detectMonster = false;
    }

    private async UniTaskVoid _AttackMonsterAsync()
    {
        _attackMonster = true;
        _animator.SetTrigger(Define.ANIMATOR_TRIGGER_ATTACK);
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_CREATE_PROJECTILE_TIME));

        for (int ii = 0; ii < _targetMonsterPositions.Length; ++ii)
        {
            if (ii >= _projectileCount)
                break;

            var initProjectilePos = transform.TransformPoint(INIT_PROJECTILE_POSITIONS[ii]);
            var projectileGO = _GetProjectile();
            _usedProjectileQueue.Enqueue(projectileGO);
            var projectile = Utils.GetOrAddComponent<ArcaneMage_Projectile>(projectileGO);
            projectile.Init(initProjectilePos, _targetMonsterPositions[ii], _ReturnProjectile);
            Utils.SetActive(projectileGO, true);
        }
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

    private void _ReturnAllProjectile()
    {
        while (_usedProjectileQueue.Count > 0)
        {
            var projectile = _usedProjectileQueue.Dequeue();
            if (projectile.activeSelf)
                _ReturnProjectile(projectile);
        }
    }

    private void _ReturnProjectile(GameObject projectile)
    {
        _projectilePool.ReturnObject(projectile);
    }

    private void _ResetDetectContainer()
    {
        for (int ii = 0; ii < _minDistancesToMonster.Length; ++ii)
            _minDistancesToMonster[ii] = MIN_DISTANCE;
        for (int ii = 0; ii < _targetMonsterPositions.Length; ++ii)
            _targetMonsterPositions[ii] = Vector3.zero;
    }
}
