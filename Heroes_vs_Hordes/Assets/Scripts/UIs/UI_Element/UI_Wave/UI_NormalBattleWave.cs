using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NormalBattleWave : UI_Wave
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

    private const float INIT_WAVE_SLIDER_VALUE = 0f;
    private const float CLEAR_WAVE_SLIDER_VALUE = 1f;
    private const float SLIDER_PROGRESS_SPEED = 0.2f;
    private const int PREV_WAVE_INDEX = 1;

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

    public override void InitWaveUI(int elementIndex, Transform parent, Vector2 elementPosition, Vector2 elementSize, Vector2 iconSize)
    {
        base.InitWaveUI(elementIndex, parent, elementPosition, elementSize, iconSize);

        _InitIconTransform(_nextBattleIcon, iconSize);
        _InitIconTransform(_currentBattleIcon, iconSize + ADJUST_CURRENT_BATTLE_ICON_SIZE);
        _InitIconTransform(_finishedBattleIcon, iconSize);

        _waveSlider.value = INIT_WAVE_SLIDER_VALUE;
        var currentWaveIndex = Manager.Instance.Ingame.CurrentWaveIndex;
        if (currentWaveIndex == _elementIndex)
            _ActiveWaveIcon(false, true, false);
        else
            _ActiveWaveIcon(true, false, false);
    }

    public override void UpdateWaveUI()
    {
        var currentWaveIndex = Manager.Instance.Ingame.CurrentWaveIndex;
        if (currentWaveIndex == _elementIndex)
            _ActiveWaveIcon(false, true, false);
        else if (_elementIndex <= currentWaveIndex - PREV_WAVE_INDEX)
        {
            _ActiveWaveIcon(false, false, true);
            _waveSlider.value = CLEAR_WAVE_SLIDER_VALUE;
            if (currentWaveIndex - PREV_WAVE_INDEX == _elementIndex)
                _fillSliderImage.sprite = _redSliderSprite;
            else
                _fillSliderImage.sprite = _yellowSliderSprite;
        }
    }

    public override void UpdateWaveUIAnimation(Action completeAnimationCallback)
    {
        _UpdateWaveUIAnimation(completeAnimationCallback).Forget();
    }

    public override void ReturnWaveUI()
    {
        Manager.Instance.UI.ReturnElementUI(Define.RESOURCE_UI_NORMAL_BATTLE_WAVE, gameObject);
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

    private async UniTaskVoid _UpdateWaveUIAnimation(Action completeAnimationCallback)
    {
        _ActiveWaveIcon(false, false, true);
        _fillSliderImage.sprite = _redSliderSprite;
        var sliderValue = INIT_WAVE_SLIDER_VALUE;
        while (sliderValue < CLEAR_WAVE_SLIDER_VALUE)
        {
            sliderValue += SLIDER_PROGRESS_SPEED;
            if (sliderValue > CLEAR_WAVE_SLIDER_VALUE)
                sliderValue = CLEAR_WAVE_SLIDER_VALUE;
            _waveSlider.value = Mathf.Lerp(INIT_WAVE_SLIDER_VALUE, CLEAR_WAVE_SLIDER_VALUE, sliderValue);
            await UniTask.Delay(TimeSpan.FromSeconds(SLIDER_PROGRESS_SPEED), ignoreTimeScale: true);
        }
        completeAnimationCallback?.Invoke();
    }
}
