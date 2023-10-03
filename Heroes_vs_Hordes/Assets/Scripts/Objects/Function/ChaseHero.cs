using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseHero : MonoBehaviour
{
    public Transform HeroTransform { get; set; }
    public Vector3 AdjustPosition { get; set; } = Vector3.zero;

    private void LateUpdate()
    {
        if (null != HeroTransform)
            transform.position = HeroTransform.position + AdjustPosition;
    }

    private void OnDisable()
    {
        HeroTransform = null;
        transform.position = Vector3.zero;
    }
}
