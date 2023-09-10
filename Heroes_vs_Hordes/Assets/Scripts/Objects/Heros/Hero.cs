using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private HeroController _heroController;

    public HeroController HeroController { get { return _heroController; } }

    private void OnEnable()
    {
        _heroController.InitTransform();
    }
}
