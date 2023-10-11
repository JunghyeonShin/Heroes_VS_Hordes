using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectableHero : UI_Element
{
    private enum EButtons
    {
        SelectableHeroButton
    }

    private enum EGameObjects
    {
        NeedToGoldTag,
        SelectHeroBorder
    }

    private enum ETexts
    {
        NeedToGoldText,
        SelectableHeroNameText
    }

    public event Action<int> SelectHeroHandler;

    private Image _selectableHeroButtonImage;

    private GameObject _needToGoldTag;
    private GameObject _selectHeroBorder;

    private TextMeshProUGUI _needToGoldText;
    private TextMeshProUGUI _seletableHeroNameText;

    private int _selectableIndex;

    private const int INIT_HERO_LEVEL = 1;

    private readonly Dictionary<string, string> SELECTABLE_HERO_NAME_DIC = new Dictionary<string, string>()
    {
        {Define.RESOURCE_HERO_ARCANE_MAGE, "아케인 메이지" },
        {Define.RESOURCE_HERO_KNIGHT, "나이트" }
    };

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindText(typeof(ETexts));

        var selectableHeroButtonGO = _GetButton((int)EButtons.SelectableHeroButton).gameObject;
        _BindEvent(selectableHeroButtonGO, _ClickSelectableHero);
        _selectableHeroButtonImage = Utils.GetOrAddComponent<Image>(selectableHeroButtonGO);

        _needToGoldTag = _GetGameObject((int)EGameObjects.NeedToGoldTag);
        _selectHeroBorder = _GetGameObject((int)EGameObjects.SelectHeroBorder);

        _needToGoldText = _GetText((int)ETexts.NeedToGoldText);
        _seletableHeroNameText = _GetText((int)ETexts.SelectableHeroNameText);
    }

    #region Event
    private void _ClickSelectableHero()
    {
        SelectHeroHandler?.Invoke(_selectableIndex);
    }
    #endregion

    public void SetSelectableHero(int index, string heroName, string selectableHeroName)
    {
        _selectableIndex = index;
        if (Manager.Instance.SaveData.OwnedHeroes[_selectableIndex] < INIT_HERO_LEVEL)
            Utils.SetActive(_needToGoldTag, true);

        Manager.Instance.Resource.LoadAsync<Sprite>(selectableHeroName, (sprite) => { _selectableHeroButtonImage.sprite = sprite; });

        _seletableHeroNameText.text = SELECTABLE_HERO_NAME_DIC[heroName];
    }

    public void ChangeSelectHero(int index)
    {
        if (index == _selectableIndex)
            Utils.SetActive(_selectHeroBorder, true);
        else
            Utils.SetActive(_selectHeroBorder, false);
    }

    public void SetNeedToGold()
    {
        if (false == _needToGoldTag.activeSelf)
            return;

        _needToGoldText.text = Manager.Instance.Data.CostToObtainHeroDataList[_selectableIndex].NeedGold.ToString();
    }
}
