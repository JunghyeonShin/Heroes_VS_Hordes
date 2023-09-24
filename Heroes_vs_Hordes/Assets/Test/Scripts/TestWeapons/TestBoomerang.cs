using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoomerang : MonoBehaviour
{
    public event Action FinishAttackHandler;

    [SerializeField] private TestHeroController _testHeroController;

    private Vector3 _targetPos;
    private float _speed;
    private float _attack;
    private float _effectTime;

    private bool _actTest;

    private const float MIN_DAMAGE_TEXT_POSITION_X = -1f;
    private const float MAX_DAMAGE_TEXT_POSITION_X = 1f;
    private const float DAMAGE_TEXT_POSITION_Y = 1f;
    private const float TWO_MULTIPLES_VALUE = 2f;
    private const float MOVE_TIME = 0.4f;
    private const float WAIT_TIME = 1f - (MOVE_TIME * 2f);
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;

    private void FixedUpdate()
    {
        if (_actTest)
            transform.Rotate(new Vector3(0f, 0f, _speed) * Time.fixedDeltaTime);
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

    public void Init(Vector3 initPos, Vector3 targetPos, float speed, float attack, float effectTime)
    {
        _actTest = true;

        transform.position = initPos;
        _targetPos = targetPos;
        _speed = speed;
        _attack = attack;
        _effectTime = effectTime;

        _MoveBoomerang().Forget();
    }

    private async UniTaskVoid _MoveBoomerang()
    {
        // Move to target
        var time = ZERO_SECOND;
        var maxTime = _effectTime * MOVE_TIME;
        var startPos = _testHeroController.transform.position;
        var lastPos = _testHeroController.transform.position + _targetPos;
        while (time < maxTime)
        {
            time += Time.deltaTime;
            if (time >= maxTime)
                time = maxTime;

            transform.position = Vector3.Lerp(startPos, lastPos, time / maxTime);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime), delayTiming: PlayerLoopTiming.LastUpdate);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(_effectTime * WAIT_TIME));

        // Move to hero
        time = maxTime;
        lastPos = transform.position;
        while (time > ZERO_SECOND)
        {
            time -= Time.deltaTime;
            if (time <= ZERO_SECOND)
                time = ZERO_SECOND;

            transform.position = Vector3.Lerp(_testHeroController.transform.position, lastPos, time / maxTime);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime), delayTiming: PlayerLoopTiming.LastUpdate);
        }
        FinishAttackHandler?.Invoke();
    }
}
