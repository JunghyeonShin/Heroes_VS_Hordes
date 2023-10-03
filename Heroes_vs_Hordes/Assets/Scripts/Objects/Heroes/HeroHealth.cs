using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroHealth : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;

    private RectTransform _rectTransform;

    public Transform HeroTransform { get; set; }

    private const float INIT_HEALTH_SLIDER_VALUE = 1f;

    private readonly Vector3 FLOATING_HEALTH_SLIDER_POSITION = new Vector3(0f, 2.25f, 0f);

    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _healthSlider.value = INIT_HEALTH_SLIDER_VALUE;
    }

    private void LateUpdate()
    {
        if (null != HeroTransform)
            _rectTransform.anchoredPosition = HeroTransform.position + FLOATING_HEALTH_SLIDER_POSITION;
    }

    public void ChangeHealthValue(float value)
    {
        _healthSlider.value = value;
    }
}
