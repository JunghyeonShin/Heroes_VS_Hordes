using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CoinRushWave : UI_Wave
{
    private enum EGameObjects
    {
        NextCoinIcon,
        CurrentCoinIcon
    }

    private GameObject _nextCoinIcon;
    private GameObject _currentCoinIcon;

    private readonly Vector2 ADJUST_CURRENT_BATTLE_ICON_SIZE = new Vector2(10f, 15f);

    protected override void _Init()
    {
        _BindGameObject(typeof(EGameObjects));

        _nextCoinIcon = _GetGameObject((int)EGameObjects.NextCoinIcon);
        _currentCoinIcon = _GetGameObject((int)EGameObjects.CurrentCoinIcon);
    }

    public override void InitWaveUI(int elementIndex, Transform parent, Vector2 elementPosition, Vector2 elementSize, Vector2 iconSize)
    {
        base.InitWaveUI(elementIndex, parent, elementPosition, elementSize, iconSize);

        _InitIconTransform(_nextCoinIcon, iconSize);
        _InitIconTransform(_currentCoinIcon, iconSize + ADJUST_CURRENT_BATTLE_ICON_SIZE);
    }

    public override void UpdateWaveUI()
    {
        var currentWaveIndex = Manager.Instance.Ingame.CurrentWaveIndex;
        if (currentWaveIndex == _elementIndex)
            _ActiveWaveIcon(false, true);
    }

    public override void ReturnWaveUI()
    {
        Manager.Instance.UI.ReturnElementUI(Define.RESOURCE_UI_COIN_RUSH_WAVE, gameObject);
    }

    private void _InitIconTransform(GameObject icon, Vector2 size)
    {
        var rectTransform = icon.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
    }

    private void _ActiveWaveIcon(bool _nextCoinIconActive, bool _currentCoinIconActive)
    {
        Utils.SetActive(_nextCoinIcon, _nextCoinIconActive);
        Utils.SetActive(_currentCoinIcon, _currentCoinIconActive);
    }
}
