using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroHealth : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;

    private RectTransform _rectTransform;

    private const float INIT_HEALTH_SLIDER_VALUE = 1f;

    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _healthSlider.value = INIT_HEALTH_SLIDER_VALUE;
    }

    public void ChangeHealthValue(float value)
    {
        _healthSlider.value = value;
    }
}
