using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UI_TestScene : UI_Scene
{
    private enum EButtons
    {
        Spawn,
        LevelUp,
        LevelDown,
        NormalAttack,
        CrossbowAttack,
        FireballAttack
    }

    private enum ETexts
    {
        WeaponName,
        AbilityText
    }

    public event Action SpawnMonsterHandler;
    public event Action LevelUpHandler;
    public event Action LevelDownHandler;
    public event Action NormalAttackHandler;
    public event Action CrossbowAttackHandler;
    public event Action FireballAttackHandler;

    private TextMeshProUGUI _weaponName;
    private TextMeshProUGUI _abilityText;

    private const float EMPTY_VALUE = 0f;

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.Spawn).gameObject, _SpawnMonster);
        _BindEvent(_GetButton((int)EButtons.LevelUp).gameObject, _LevelUp);
        _BindEvent(_GetButton((int)EButtons.LevelDown).gameObject, _LevelDown);
        _BindEvent(_GetButton((int)EButtons.NormalAttack).gameObject, _StartNormalAttack);
        _BindEvent(_GetButton((int)EButtons.CrossbowAttack).gameObject, _StartCrossbowAttack);
        _BindEvent(_GetButton((int)EButtons.FireballAttack).gameObject, _StartFireballAttack);

        _weaponName = _GetText((int)ETexts.WeaponName);
        _abilityText = _GetText((int)ETexts.AbilityText);
    }

    #region Event
    private void _SpawnMonster()
    {
        SpawnMonsterHandler?.Invoke();
    }

    private void _LevelUp()
    {
        LevelUpHandler?.Invoke();
    }

    private void _LevelDown()
    {
        LevelDownHandler?.Invoke();
    }

    private void _StartNormalAttack()
    {
        NormalAttackHandler?.Invoke();
    }

    private void _StartCrossbowAttack()
    {
        CrossbowAttackHandler?.Invoke();
    }

    private void _StartFireballAttack()
    {
        FireballAttackHandler?.Invoke();
    }
    #endregion

    public void SetWeaponName(string weaponName)
    {
        _weaponName.text = weaponName;
    }

    public void SetAbilityText(float attack, float attackCooldown, float speed, float effecRange, float effectTime, float projectileCount, float penetraitCount)
    {
        var sb = new StringBuilder();
        if (attack > EMPTY_VALUE)
            sb.Append($"Attack : {attack}\n");
        if (attackCooldown > EMPTY_VALUE)
            sb.Append($"AttackCooldown : {attackCooldown}\n");
        if (speed > EMPTY_VALUE)
            sb.Append($"Speed : {speed}\n");
        if (effecRange > EMPTY_VALUE)
            sb.Append($"EffectRange : {effecRange}\n");
        if (effectTime > EMPTY_VALUE)
            sb.Append($"EffectTime : {effectTime}\n");
        if (projectileCount > EMPTY_VALUE)
            sb.Append($"ProjectileCount : {projectileCount}\n");
        if (penetraitCount > EMPTY_VALUE)
            sb.Append($"PenetraitCount : {penetraitCount}");
        _abilityText.text = sb.ToString();
    }
}
