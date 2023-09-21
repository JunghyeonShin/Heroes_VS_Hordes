using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GoldRushWave : UI_Wave
{
    private enum EGameObjects
    {
        NextGoldIcon,
        CurrentGoldIcon
    }

    private GameObject _nextGoldIcon;
    private GameObject _currentGoldIcon;

    private readonly Vector2 ADJUST_CURRENT_ICON_SIZE = new Vector2(10f, 15f);

    protected override void _Init()
    {
        _BindGameObject(typeof(EGameObjects));

        _nextGoldIcon = _GetGameObject((int)EGameObjects.NextGoldIcon);
        _currentGoldIcon = _GetGameObject((int)EGameObjects.CurrentGoldIcon);
    }

    public override void InitWaveUI(int elementIndex, Transform parent, Vector2 elementPosition, Vector2 elementSize, Vector2 iconSize)
    {
        base.InitWaveUI(elementIndex, parent, elementPosition, elementSize, iconSize);

        _InitTransform(_nextGoldIcon, iconSize);
        _InitTransform(_currentGoldIcon, iconSize + ADJUST_CURRENT_ICON_SIZE);
    }

    public override void UpdateWaveUI()
    {
        var currentWaveIndex = Manager.Instance.Ingame.CurrentWaveIndex;
        if (currentWaveIndex == _elementIndex)
            _ActiveWaveIcon(false, true);
    }

    public override void UpdateWaveUIAnimation(Action completeAnimationCallback)
    {

    }

    public override void ReturnWaveUI()
    {
        _ActiveWaveIcon(true, false);
        Manager.Instance.UI.ReturnElementUI(Define.RESOURCE_UI_GOLD_RUSH_WAVE, gameObject);
    }

    private void _InitTransform(GameObject icon, Vector2 size)
    {
        var rectTransform = icon.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
    }

    private void _ActiveWaveIcon(bool _nextGoldIconActive, bool _currentGoldIconActive)
    {
        Utils.SetActive(_nextGoldIcon, _nextGoldIconActive);
        Utils.SetActive(_currentGoldIcon, _currentGoldIconActive);
    }
}
