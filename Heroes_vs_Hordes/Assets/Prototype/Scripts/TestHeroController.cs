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
        [SerializeField] private GameObject _projectile;
        [SerializeField] private Vector2 _overlapSize = new Vector2(25f, 40f);
        [SerializeField] private float _attackDelayTime = 1.5f;
        [SerializeField] private float _moveSpeed = 5f;

        private ObjectPool _projectilePool = new ObjectPool();
        private Rigidbody2D _rigid;
        private Animator _animator;
        private TestMonsterController _monster;
        private Vector2 _inputVec;

        private const float ANGLE_180 = 180f;
        private const float ATTACK_TIME = 0.06f;
        private const float CHECK_DIRECTION = 0f;
        private const float DEFAULT_DETECT_BOX_ANGLE = 0f;
        private const int CREATE_PROJECTILE_COUNT = 50;
        private const int DETECT_MONSTER_COUNT = 0;
        private const string ANIMATOR_TRIGGER_ATTACK = "Attack";
        private const string LAYER_MONSTER = "Monster";
        private const string NAME_PROJECTILE = "[ROOT_PROJECTILE]";
        private const string RESOURCE_PROJECTILE = "ArcaneMage_Projectile";

        private readonly Vector3 CREATE_PROJECTILE_POSITION = new Vector3(1f, -0.276f, 0f);

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();


            var poolGO = new GameObject(NAME_PROJECTILE);

            Manager.Instance.Resource.LoadAsync<GameObject>(RESOURCE_PROJECTILE, (projectile) =>
            {
                _projectilePool.InitPool(projectile, poolGO, CREATE_PROJECTILE_COUNT);
            });
        }

        private void FixedUpdate()
        {
            var moveVec = _inputVec * _moveSpeed * Time.fixedDeltaTime;
            _rigid.MovePosition(_rigid.position + moveVec);

            var angle = Vector2.Angle(Vector2.up, _inputVec.normalized);
            if (_inputVec.Equals(Vector2.zero))
                _rigid.rotation = angle;
            else
            {
                if (_IsRightSide(_inputVec.x))
                    _rigid.rotation = ANGLE_180 - angle;
                else
                    _rigid.rotation = ANGLE_180 + angle;
            }
        }

        private void Update()
        {
            _DetectMonster();
        }

        private bool _IsRightSide(float value)
        {
            return value >= CHECK_DIRECTION;
        }

        private void _DetectMonster()
        {
            if (null != _monster)
                return;

            var layerMask = 1 << LayerMask.NameToLayer(LAYER_MONSTER);
            var monsters = Physics2D.OverlapBoxAll(transform.position, _overlapSize, DEFAULT_DETECT_BOX_ANGLE, layerMask);
            if (monsters.Length > DETECT_MONSTER_COUNT)
            {
                _monster = Utils.GetOrAddComponent<TestMonsterController>(monsters[0].gameObject);
                _monster.OnDieHandler -= _OnDeadTargetMonster;
                _monster.OnDieHandler += _OnDeadTargetMonster;
                _AttackMonster().Forget();
            }
        }

        private async UniTaskVoid _AttackMonster()
        {
            while (null != _monster)
            {
                _animator.SetTrigger(ANIMATOR_TRIGGER_ATTACK);
                await UniTask.Delay(TimeSpan.FromSeconds(ATTACK_TIME));

                var projectileGO = _projectilePool.GetObject();
                var createProjectilePos = transform.TransformPoint(CREATE_PROJECTILE_POSITION);
                projectileGO.transform.position = createProjectilePos;
                var projectile = Utils.GetOrAddComponent<TestProjectile>(projectileGO);
                projectile.TargetMonster = _monster.transform;
                Utils.SetActive(projectileGO, true);
                await UniTask.Delay(TimeSpan.FromSeconds(_attackDelayTime - ATTACK_TIME));
            }
        }

        private void _OnDeadTargetMonster()
        {
            _monster = null;
        }

        private void OnMove(InputValue value)
        {
            _inputVec = value.Get<Vector2>();
        }
    }
}
