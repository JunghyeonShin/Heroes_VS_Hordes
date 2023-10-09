using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_HERO))
        {
            var usedHero = Utils.GetOrAddComponent<Hero>(collision.gameObject);
            usedHero.IsSlow = true;

            var slowEffectGO = Manager.Instance.Object.SlowEffect;
            var chaseHero = Utils.GetOrAddComponent<ChaseHero>(slowEffectGO);
            chaseHero.HeroTransform = collision.transform;
            Utils.SetActive(slowEffectGO, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_HERO))
        {
            var usedHero = Utils.GetOrAddComponent<Hero>(collision.gameObject);
            usedHero.IsSlow = false;

            Utils.SetActive(Manager.Instance.Object.SlowEffect, false);
        }
    }
}
