using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCrossbow : MonoBehaviour
{
    [SerializeField] private TestHeroController _testHeroController;
    [SerializeField] private GameObject _arrowhead;

    private Rigidbody2D _rigid;
    private Vector3 _targetPos;
    private float _speed;
    private float _attack;

    private bool _reflectHorizontal;
    private bool _reflectVertical;

    private const float REVERSE_ANGLE = -1f;
    private const float CHECK_DIRECTION = 0f;
    private const float MIN_IN_WORLD_TO_VIEWPORT_POINT = 0.3f;
    private const float MAX_IN_WORLD_TO_VIEWPORT_POINT = 0.7f;
    private const float MIN_OUT_WORLD_TO_VIEWPORT_POINT = 0f;
    private const float MAX_OUT_WORLD_TO_VIEWPORT_POINT = 1f;
    private const float MIN_DAMAGE_TEXT_POSITION_X = -1f;
    private const float MAX_DAMAGE_TEXT_POSITION_X = 1f;
    private const float DAMAGE_TEXT_POSITION_Y = 1f;
    private const float TWO_MULTIPLES_VALUE = 2f;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {

    }

    private void FixedUpdate()
    {
        var moveVec = new Vector2(_targetPos.x, _targetPos.y) * _speed * Time.fixedDeltaTime;
        _rigid.MovePosition(_rigid.position + moveVec);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_MONSTER))
        {
            var randomPos = new Vector3(UnityEngine.Random.Range(MIN_DAMAGE_TEXT_POSITION_X, MAX_DAMAGE_TEXT_POSITION_X), DAMAGE_TEXT_POSITION_Y, 0f);
            var initDamageTextPos = collision.transform.position + randomPos;
            var damageTextGO = Manager.Instance.Object.GetDamageText();
            var damageText = Utils.GetOrAddComponent<DamageText>(damageTextGO);
            var attack = _attack;
            var isCritical = _testHeroController.IsCritical();
            if (isCritical)
                attack = _attack * TWO_MULTIPLES_VALUE;
            damageText.FloatDamageText(initDamageTextPos, attack, isCritical);
            Utils.SetActive(damageTextGO, true);

            var testMonster = Utils.GetOrAddComponent<TestMonster>(collision.gameObject);
            testMonster.OnDamaged(attack);
        }
    }

    private void Update()
    {
        var pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x <= MIN_OUT_WORLD_TO_VIEWPORT_POINT || pos.x >= MAX_OUT_WORLD_TO_VIEWPORT_POINT)
        {
            if (_reflectHorizontal)
                return;

            _reflectHorizontal = true;
            _targetPos = Vector3.Reflect(_targetPos, Vector3.right);
            _RotateArrowheadToTarget();
        }
        else if (MIN_IN_WORLD_TO_VIEWPORT_POINT <= pos.x && pos.x <= MAX_IN_WORLD_TO_VIEWPORT_POINT)
        {
            if (_reflectHorizontal)
                _reflectHorizontal = false;
        }

        if (pos.y <= MIN_OUT_WORLD_TO_VIEWPORT_POINT || pos.y >= MAX_OUT_WORLD_TO_VIEWPORT_POINT)
        {
            if (_reflectVertical)
                return;

            _reflectVertical = true;
            _targetPos = Vector3.Reflect(_targetPos, Vector3.up);
            _RotateArrowheadToTarget();
        }
        else if (MIN_IN_WORLD_TO_VIEWPORT_POINT <= pos.y && pos.y <= MAX_IN_WORLD_TO_VIEWPORT_POINT)
        {
            if (_reflectVertical)
                _reflectVertical = false;
        }
    }

    public void Init(Vector3 initPos, Vector3 targetPos, float speed, float attack)
    {
        transform.position = initPos;
        _targetPos = targetPos;
        _RotateArrowheadToTarget();
        _speed = speed;
        _attack = attack;
    }

    private void _RotateArrowheadToTarget()
    {
        var angle = Vector3.Angle(Vector3.up, _targetPos);
        if (_IsRightSide(_targetPos.x))
            angle *= REVERSE_ANGLE;
        _arrowhead.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    private bool _IsRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }
}
