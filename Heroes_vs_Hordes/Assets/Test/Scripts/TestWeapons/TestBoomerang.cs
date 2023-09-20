using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoomerang : MonoBehaviour
{
    [SerializeField] private TestHeroController _testHeroController;

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
    }
}
