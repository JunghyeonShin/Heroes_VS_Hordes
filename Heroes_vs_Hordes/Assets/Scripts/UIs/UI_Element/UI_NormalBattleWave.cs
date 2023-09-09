using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NormalBattleWave : UI_Element
{
    private enum EGameObjects
    {
        NextBattleIcon,
        CurrentBattleIcon,
        FinishedBattleIcon
    }

    private enum EImages
    {
        FillSliderImage
    }

    private enum ESliders
    {
        WaveSlider
    }

    private GameObject _nextBattleIcon;
    private GameObject _currentBattleIcon;
    private GameObject _finishedBattleIcon;

    private Image _fillSliderImage;
    private Sprite _redSliderSprite;
    private Sprite _yellowSliderSprite;

    private Slider _waveSlider;

    private const float CLEAR_WAVE_SLIDER_VALUE = 1f;

    private readonly Vector2 ADJUST_CURRENT_BATTLE_ICON_SIZE = new Vector2(10f, 15f);

    protected override void _Init()
    {
        _BindGameObject(typeof(EGameObjects));
        _BindImage(typeof(EImages));
        _BindSlider(typeof(ESliders));

        _nextBattleIcon = _GetGameObject((int)EGameObjects.NextBattleIcon);
        _currentBattleIcon = _GetGameObject((int)EGameObjects.CurrentBattleIcon);
        _finishedBattleIcon = _GetGameObject((int)EGameObjects.FinishedBattleIcon);

        _fillSliderImage = _GetImage((int)EImages.FillSliderImage);
        Manager.Instance.Resource.LoadAsync<Sprite>(Define.RESOURCE_SPRITES_SLIDER_RED, (sprite) =>
        {
            _redSliderSprite = sprite;
        });
        Manager.Instance.Resource.LoadAsync<Sprite>(Define.RESOURCE_SPRITES_SLIDER_YELLOW, (sprite) =>
        {
            _yellowSliderSprite = sprite;
        });

        _waveSlider = _GetSlider((int)ESliders.WaveSlider);
    }

    public override void InitUIElement(int elementIndex, Transform parent, Vector2 elementPosition, Vector2 elementSize, Vector2 iconSize)
    {
        _elementIndex = elementIndex;
        transform.SetParent(parent);
        var rectTransform = transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = elementPosition;
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = elementSize;

        _InitIconTransform(_nextBattleIcon, iconSize);
        _InitIconTransform(_currentBattleIcon, iconSize + ADJUST_CURRENT_BATTLE_ICON_SIZE);
        _InitIconTransform(_finishedBattleIcon, iconSize);
    }

    public override void UpdateUIElement()
    {
        var currentWaveIndex = Manager.Instance.Ingame.CurrentWaveIndex;
        if (currentWaveIndex == _elementIndex)
            _ActiveWaveIcon(false, true, false);
        else if (_elementIndex <= currentWaveIndex - 1)
        {
            _ActiveWaveIcon(false, false, true);
            _waveSlider.value = CLEAR_WAVE_SLIDER_VALUE;
            if (currentWaveIndex - 1 == _elementIndex)
                _fillSliderImage.sprite = _redSliderSprite;
            else
                _fillSliderImage.sprite = _yellowSliderSprite;
        }
    }

    private void _InitIconTransform(GameObject icon, Vector2 size)
    {
        var rectTransform = icon.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
    }

    private void _ActiveWaveIcon(bool nextBattleWaveIconActive, bool currentBattleWaveIconActive, bool finishedBattleWaveIconActive)
    {
        Utils.SetActive(_nextBattleIcon, nextBattleWaveIconActive);
        Utils.SetActive(_currentBattleIcon, currentBattleWaveIconActive);
        Utils.SetActive(_finishedBattleIcon, finishedBattleWaveIconActive);
    }
}
