using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDivineAura : MonoBehaviour
{
    [SerializeField] private TestHeroController _testHeroController;
    [SerializeField] private GameObject _divineArua;
    [SerializeField] private GameObject _divineAruaBorder;

    private Collider2D _collider;
    private float _attack;
    private float _attackCooldown;
    private float _effectRange;
    private float _effectTime;

    private const float TWO_MULTIPLIES_VALUE = 2f;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    public void Init(Vector3 initPPos, float attack, float attackCooldown, float effectRange, float effectTime)
    {
        transform.position = initPPos;
        _attack = attack;
        _attackCooldown = attackCooldown;
        _effectRange = effectRange;
        _effectTime = effectTime;

        var collider = _collider as CircleCollider2D;
        collider.radius = _effectRange * TWO_MULTIPLIES_VALUE;
        _divineArua.transform.localScale = new Vector3(_effectRange, _effectRange, 0f);
        _divineAruaBorder.transform.localScale = new Vector3(_effectRange * TWO_MULTIPLIES_VALUE, _effectRange * TWO_MULTIPLIES_VALUE, 0f);
    }
}
