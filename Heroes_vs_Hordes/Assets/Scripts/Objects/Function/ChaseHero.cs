using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseHero : MonoBehaviour
{
    public Transform HeroTransform { get; set; }

    private void LateUpdate()
    {
        if (null != HeroTransform)
            transform.position = HeroTransform.position;
    }

    private void OnDisable()
    {
        HeroTransform = null;
        transform.position = Vector3.zero;
    }
}
