using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;

    private Rigidbody2D _rigid;
    private Action _dieHandler;

    public Transform Target { get; set; }

    private const float REVERSE_ANGLE = -1f;
    private const float CHECK_DIRECTION = 0f;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _dieHandler -= Manager.Instance.Ingame.OnDeadMonster;
        _dieHandler += Manager.Instance.Ingame.OnDeadMonster;
    }

    private void OnDisable()
    {
        _dieHandler -= Manager.Instance.Ingame.OnDeadMonster;
    }

    private void FixedUpdate()
    {
        var monsterToHeroVec = Target.position - transform.position;
        var monsterToHeroNormalVec = new Vector2(monsterToHeroVec.x, monsterToHeroVec.y).normalized;
        var lookAngle = Vector2.Angle(Vector2.up, monsterToHeroNormalVec);
        if (_IsLocatedTargetRightSide(monsterToHeroNormalVec.x))
            lookAngle *= REVERSE_ANGLE;
        _rigid.rotation = lookAngle;

        var moveVec = monsterToHeroNormalVec * _moveSpeed * Time.fixedDeltaTime;
        _rigid.MovePosition(_rigid.position + moveVec);
    }

    public void OnDamaged(float damage)
    {
        var waveIndex = Manager.Instance.Data.ChapterInfoList[Define.CURRENT_CHAPTER_INDEX].WaveIndex[Manager.Instance.Ingame.CurrentWaveIndex];
        if (Define.INDEX_GOLD_RUSH_WAVE == waveIndex)
            ShowDropItem<Gold>(Define.RESOURCE_GOLD);
        else
            ShowDropItem<ExpGem>(Define.RESOURCE_EXP_GEM);
    }

    public void ReturnMonster()
    {
        Manager.Instance.Object.ReturnMonster(Define.RESOURCE_MONSTER_NORMAL_BAT, gameObject);
    }

    private void ShowDropItem<T>(string dropItemKey) where T : DropItem
    {
        Manager.Instance.Object.GetDropItem(dropItemKey, (dropItemGO) =>
        {
            var dropItem = Utils.GetOrAddComponent<T>(dropItemGO);
            dropItem.InitTransform(transform.position);
            Utils.SetActive(dropItemGO, true);

            _dieHandler?.Invoke();
            ReturnMonster();
        });
    }

    private bool _IsLocatedTargetRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }
}
