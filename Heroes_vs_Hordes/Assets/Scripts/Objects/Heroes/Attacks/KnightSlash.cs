using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightSlash : MonoBehaviour
{
    private string _weaponName;

    private float _attack;
    private float _critical;

    private const float MIN_DAMAGE_TEXT_POSITION_X = -1f;
    private const float MAX_DAMAGE_TEXT_POSITION_X = 1f;
    private const float DAMAGE_TEXT_POSITION_Y = 1f;
    private const float TWO_MULTIPLES_VALUE = 2f;

    private void Awake()
    {
        _weaponName = Define.WEAPON_KNIGHT_SWORD;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_MONSTER))
        {
            var randomPos = new Vector3(UnityEngine.Random.Range(MIN_DAMAGE_TEXT_POSITION_X, MAX_DAMAGE_TEXT_POSITION_X), DAMAGE_TEXT_POSITION_Y, 0f);
            var initDamageTextPos = collision.transform.position + randomPos;
            var damageTextGO = Manager.Instance.Object.GetDamageText();
            var damageText = Utils.GetOrAddComponent<DamageText>(damageTextGO);
            var isCritical = HeroAbility.IsCritical(_critical);
            var attack = _attack;
            if (isCritical)
                attack = _attack * TWO_MULTIPLES_VALUE;
            damageText.FloatDamageText(initDamageTextPos, attack, isCritical);
            Utils.SetActive(damageTextGO, true);

            var monster = Utils.GetOrAddComponent<Monster>(collision.gameObject);
            monster.OnDamage(_attack);
        }
    }

    public void SetAbilities()
    {
        var usedHero = Manager.Instance.Ingame.UsedHero;
        var weaponLevel = Manager.Instance.Ingame.GetOwnedAbilityLevel(_weaponName);
        _attack = WeaponAbility.GetWeaponAttack(usedHero.HeroName, _weaponName, weaponLevel);
        _critical = HeroAbility.GetHeroCritical(usedHero.HeroName);
    }
}
