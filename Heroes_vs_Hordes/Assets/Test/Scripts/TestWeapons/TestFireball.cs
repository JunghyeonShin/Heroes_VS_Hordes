using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFireball : MonoBehaviour
{
    [SerializeField] private TestHeroController _testHeroController;

    private Collider2D _collider;
    private Vector3 _targetPos;
    private float _speed;
    private float _attack;
    private float _effectRange;
    private float _effectTime;

    private const float MIN_DAMAGE_TEXT_POSITION_X = -1f;
    private const float MAX_DAMAGE_TEXT_POSITION_X = 1f;
    private const float DAMAGE_TEXT_POSITION_Y = 1f;
    private const float TWO_MULTIPLES_VALUE = 2f;
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;
    private const float PROGRESS_TIME = 0.02f;
    private const float CHECK_DIRECTION = 0f;
    private const float ANGLE_360 = 360f;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
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

    public void Init(Vector3 initPos, Vector3 targetPos, float speed, float attack, float effectRange, float effectTime)
    {
        transform.position = initPos;
        _targetPos = targetPos;
        _speed = speed;
        _attack = attack;
        _effectRange = effectRange;
        _effectTime = effectTime;

        transform.localScale = Vector3.zero;
        _collider.enabled = false;
        _RotateFireball().Forget();
    }

    private async UniTaskVoid _RotateFireball()
    {
        // Fade-In
        var time = ZERO_SECOND;
        while (time < ONE_SECOND)
        {
            time += PROGRESS_TIME;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;

            transform.position = Vector3.Lerp(_testHeroController.transform.position, _testHeroController.transform.position + _targetPos, time);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
            await UniTask.Delay(TimeSpan.FromSeconds(PROGRESS_TIME), delayTiming: PlayerLoopTiming.LastUpdate);
        }
        _collider.enabled = true;

        // Rotate
        time = ZERO_SECOND;
        var angle = Vector3.Angle(Vector3.up, (transform.position - _testHeroController.transform.position).normalized);
        if (_IsRightSide(transform.position.x - _testHeroController.transform.position.x))
            angle = ANGLE_360 - angle;
        while (time < _effectTime)
        {
            time += Time.fixedDeltaTime;
            if (time >= _effectTime)
                time = _effectTime;

            angle += _speed * Time.fixedDeltaTime;
            var rotateVec = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;
            transform.position = _testHeroController.transform.position + rotateVec.normalized * _effectRange;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedDeltaTime), delayTiming: PlayerLoopTiming.FixedUpdate);
        }
        _collider.enabled = false;

        // Fade-Out
        time = ONE_SECOND;
        var lastPos = transform.position;
        while (time > ZERO_SECOND)
        {
            time -= PROGRESS_TIME;
            if (time <= ZERO_SECOND)
                time = ZERO_SECOND;

            transform.position = Vector3.Lerp(_testHeroController.transform.position, lastPos, time);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
            await UniTask.Delay(TimeSpan.FromSeconds(PROGRESS_TIME), delayTiming: PlayerLoopTiming.LastUpdate);
        }
    }

    private bool _IsRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }
}
