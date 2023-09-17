namespace ProtoType
{
    using Cysharp.Threading.Tasks;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class TestHeroController : MonoBehaviour
    {
        [SerializeField] private GameObject _rootTestProjectile;
        [SerializeField] private GameObject _testProjectile;
        [SerializeField] private Vector2 _overlapSize = new Vector2(25f, 40f);

        private ObjectPool _projectilePool = new ObjectPool();
        private Rigidbody2D _rigid;
        private Animator _animator;

        private float _health;
        private float _defense;
        private float _attack;
        private float _attackCooldown;
        private float _critical;
        private float _moveSpeed;
        private float _projectileSpeed;
        private float _projectileCount;
        private float _penetraitCount;

        private int _weaponLevel;
        private int _normalAttackCount;
        private bool _startNormalAttack;
        private bool _detectMonster;
        private bool _attackMonster;

        public Vector2 InputVec { get; set; }

        private const float ANGLE_180 = 180f;
        private const float ATTACK_TIME = 0.06f;
        private const float CHECK_DIRECTION = 0f;
        private const float DEFAULT_DETECT_BOX_ANGLE = 0f;
        private const float DEFAULT_ABILITY_VALUE = 1f;
        private const float MIN_DISTANCE = 987654321f;
        private const float MIN_CRITICAL_VALUE = 0f;
        private const float MAX_CRITICAL_VALUE = 1f;
        private const int INIT_WEAPON_LEVEL = 1;
        private const int MAX_WEAPON_LEVEL = 5;
        private const int CREATE_PROJECTILE_COUNT = 50;
        private const int MAX_NORMAL_ATTACK_COUNT = 2;
        private const int ADJUST_WEAPON_LEVEL = 2;

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

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _weaponLevel = INIT_WEAPON_LEVEL;
            _projectilePool.InitPool(_testProjectile, _rootTestProjectile, CREATE_PROJECTILE_COUNT);
        }

        private void FixedUpdate()
        {
            var moveVec = InputVec * _moveSpeed * Time.fixedDeltaTime;
            _rigid.MovePosition(_rigid.position + moveVec);

            var angle = Vector2.Angle(Vector2.up, InputVec.normalized);
            if (InputVec.Equals(Vector2.zero))
                _rigid.rotation = angle;
            else
            {
                if (_IsRightSide(InputVec.x))
                    _rigid.rotation = ANGLE_180 - angle;
                else
                    _rigid.rotation = ANGLE_180 + angle;
            }
        }

        private void Update()
        {
            if (false == _detectMonster)
                _DetectMonster().Forget();
        }

        public void SetAbility()
        {
            var heroCommonAbility = Manager.Instance.Data.HeroCommonAbility;
            var heroIndividualAbility = Manager.Instance.Data.HeroIndividualAbilityDic[Define.RESOURCE_HERO_ARCANE_MAGE];
            var weaponAbility = Manager.Instance.Data.WeaponAbilityDic[Define.RESOURCE_WEAPON_ARCANE_MAGE_PROJECTILE];
            var weaponLevelAbilityList = Manager.Instance.Data.WeaponLevelAbilityDic[Define.RESOURCE_WEAPON_ARCANE_MAGE_PROJECTILE];

            _health = heroCommonAbility.Health + heroIndividualAbility.Health;
            _defense = heroCommonAbility.Defense + heroIndividualAbility.Defense;

            var weaponAttack = 0f;
            if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
            {
                for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                    weaponAttack += weaponLevelAbilityList[ii].Attack;
            }
            _attack = heroCommonAbility.Attack * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.Attack + weaponAttack);

            var weaponAttackCooldown = 0f;
            if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
            {
                for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                    weaponAttackCooldown += weaponLevelAbilityList[ii].AttackCooldown;
            }
            _attackCooldown = heroCommonAbility.AttackCooldown + heroIndividualAbility.AttackCooldown + weaponAttackCooldown;
            _critical = heroCommonAbility.Critical + heroIndividualAbility.Critical;
            _moveSpeed = heroCommonAbility.MoveSpeed * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.MoveSpeed);
            _projectileSpeed = heroCommonAbility.ProjectileSpeed * (DEFAULT_ABILITY_VALUE + heroIndividualAbility.ProjectileSpeed);

            var weaponProjectileCount = 0f;
            if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
            {
                for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                    weaponProjectileCount += weaponLevelAbilityList[ii].ProjectileCount;
            }
            _projectileCount = weaponAbility.ProjectileCount + weaponProjectileCount;

            var weaponPenetraitCount = 0f;
            if (_weaponLevel >= ADJUST_WEAPON_LEVEL)
            {
                for (int ii = 0; ii <= _weaponLevel - ADJUST_WEAPON_LEVEL; ++ii)
                    weaponPenetraitCount += weaponLevelAbilityList[ii].PenetrateCount;
            }
            _penetraitCount = weaponAbility.PenetrateCount + weaponPenetraitCount;
        }

        public void SetLevelUp()
        {
            ++_weaponLevel;
            if (_weaponLevel > MAX_WEAPON_LEVEL)
                _weaponLevel = MAX_WEAPON_LEVEL;
            SetAbility();
        }

        public void SetLevelDown()
        {
            --_weaponLevel;
            if (_weaponLevel < INIT_WEAPON_LEVEL)
                _weaponLevel = INIT_WEAPON_LEVEL;
            SetAbility();
        }

        public void StartNormalAttack()
        {
            _normalAttackCount = 0;
            _startNormalAttack = true;
            var testSceneUI = Manager.Instance.UI.CurrentSceneUI as UI_TestScene;
            testSceneUI.SetWeaponName(Define.RESOURCE_WEAPON_ARCANE_MAGE_PROJECTILE);
            testSceneUI.SetAttack(_attack);
            testSceneUI.SetAttackCooldown(_attackCooldown);
            testSceneUI.SetSpeed(_projectileSpeed);
            testSceneUI.SetProjectileCount(_projectileCount);
            testSceneUI.SetPenettraitCount(_penetraitCount);
        }

        private bool _IsRightSide(float value)
        {
            return value >= CHECK_DIRECTION;
        }

        private async UniTaskVoid _DetectMonster()
        {
            _detectMonster = true;
            if (_attackMonster)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));
                _attackMonster = false;
            }

            var layerMask = 1 << LayerMask.NameToLayer(Define.LAYER_MONSTER);
            var monsters = Physics2D.OverlapBoxAll(transform.position, _overlapSize, DEFAULT_DETECT_BOX_ANGLE, layerMask);
            if (monsters.Length > 0 && _startNormalAttack)
            {
                ++_normalAttackCount;
                if (_normalAttackCount >= MAX_NORMAL_ATTACK_COUNT)
                    _startNormalAttack = false;

                var minDistances = new float[] { MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE, MIN_DISTANCE };
                var targetPositions = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
                foreach (var monster in monsters)
                {
                    var distance = Vector3.Distance(transform.position, monster.transform.position);
                    for (int ii = 0; ii < minDistances.Length; ++ii)
                    {
                        if (ii >= _projectileCount)
                            break;

                        if (MIN_DISTANCE == minDistances[ii])
                        {
                            minDistances[ii] = distance;
                            targetPositions[ii] = monster.transform.position;
                            break;
                        }

                        if (distance < minDistances[ii])
                        {
                            var index = Mathf.FloorToInt(_projectileCount - 1f);
                            for (int jj = index; jj > 0 && jj >= ii; --jj)
                            {
                                minDistances[jj] = minDistances[jj - 1];
                                targetPositions[jj] = targetPositions[jj - 1];
                            }
                            minDistances[ii] = distance;
                            targetPositions[ii] = monster.transform.position;
                            break;
                        }
                    }
                }
                _AttackMonster(targetPositions).Forget();
            }
            _detectMonster = false;
        }

        private async UniTaskVoid _AttackMonster(Vector3[] targetPositions)
        {
            _attackMonster = true;
            _animator.SetTrigger(Define.ANIMATOR_TRIGGER_ATTACK);
            await UniTask.Delay(TimeSpan.FromSeconds(ATTACK_TIME));

            for (int ii = 0; ii < targetPositions.Length; ++ii)
            {
                if (ii >= _projectileCount)
                    break;
                var initProjectilePos = transform.TransformPoint(INIT_PROJECTILE_POSITIONS[ii]);
                var testProjectileGO = _GetProjectile();
                var testProjectile = Utils.GetOrAddComponent<TestProjectile>(testProjectileGO);
                testProjectile.Init(initProjectilePos, targetPositions[ii], _IsCritical(), _attack, _projectileSpeed, _penetraitCount, _ReturnProjectile);
                Utils.SetActive(testProjectileGO, true);
            }
        }

        private GameObject _GetProjectile()
        {
            return _projectilePool.GetObject();
        }

        private void _ReturnProjectile(GameObject projectile)
        {
            _projectilePool.ReturnObject(projectile);
        }

        private bool _IsCritical()
        {
            var randomValue = UnityEngine.Random.Range(MIN_CRITICAL_VALUE, MAX_CRITICAL_VALUE);
            if (_critical >= randomValue)
                return true;
            return false;
        }

        private void OnMove(InputValue value)
        {
            InputVec = value.Get<Vector2>();
        }
    }
}
