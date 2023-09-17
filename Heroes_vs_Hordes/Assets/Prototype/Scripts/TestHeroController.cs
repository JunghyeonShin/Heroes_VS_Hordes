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
        [SerializeField] private float _moveSpeed = 5f;

        private ObjectPool _projectilePool = new ObjectPool();
        private Rigidbody2D _rigid;
        private Animator _animator;

        private float _attack;
        private float _attackCooldown;
        private float _projectileSpeed;
        private float _critical;
        private int _normalAttackCount;
        private bool _startNormalAttack;
        private bool _detectMonster;
        private bool _attackMonster;

        public Vector2 InputVec { get; set; }

        private const float ANGLE_180 = 180f;
        private const float ATTACK_TIME = 0.06f;
        private const float CHECK_DIRECTION = 0f;
        private const float DEFAULT_DETECT_BOX_ANGLE = 0f;
        private const float MIN_DISTANCE = 987654321f;
        private const float MIN_CRITICAL_VALUE = 0f;
        private const float MAX_CRITICAL_VALUE = 1f;
        private const int CREATE_PROJECTILE_COUNT = 50;
        private const int MAX_NORMAL_ATTACK_COUNT = 2;

        private readonly Vector3 INIT_PROJECTILE_POSITION = new Vector3(1f, -0.276f, 0f);

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
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

        public void StartNormalAttack()
        {
            _normalAttackCount = 0;
            _startNormalAttack = true;
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
                _AttackMonster(targetPos).Forget();
            }
            _detectMonster = false;
        }

        private async UniTaskVoid _AttackMonster(Vector3 targetPos)
        {
            _attackMonster = true;
            _animator.SetTrigger(Define.ANIMATOR_TRIGGER_ATTACK);
            await UniTask.Delay(TimeSpan.FromSeconds(ATTACK_TIME));

            var initProjectilePos = transform.TransformPoint(INIT_PROJECTILE_POSITION);
            var testProjectileGO = _GetProjectile();
            var testProjectile = Utils.GetOrAddComponent<TestProjectile>(testProjectileGO);
            testProjectile.Init(initProjectilePos, targetPos, _projectileSpeed, _IsCritical(), _attack, _ReturnProjectile);
            Utils.SetActive(testProjectileGO, true);
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
