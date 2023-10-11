using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableHeroInfo
{
    public string HeroName;
    public string HeroPortraitName;
    public string SelectHeroButtonName;
    public Vector3 PortraitPosition;
    public Vector2 PortraitSize;
}

public class UI_SelectHero : UI_Popup
{
    private enum EButtons
    {
        ExitButton,
        SelectHeroButton,
        UnlockHeroButton
    }

    private enum EGameObjects
    {
        SelectedHeroPanel,
        SelectableHeroContent
    }

    private enum EImages
    {
        SelectHeroPortrait
    }

    private enum ETexts
    {
        OwnedGoldText
    }

    public event Action<int> ChangeSelectedHeroHandler;
    public event Action NeedToGoldHandler;

    private GameObject _selectHeroButton;
    private GameObject _selectedHeroPanel;
    private GameObject _unlockHeroButton;
    private GameObject _selectableHeroContent;

    private Image _selectHeroPortrait;
    private RectTransform _selectHeroPortraitRectTransform;

    private TextMeshProUGUI _ownedGoldText;

    private int _selectHeroIndex;
    private int _selectedHeroIndex;

    private const int VISIBLE_COUNT = 3;
    private const int INIT_HERO_LEVEL = 1;

    private readonly SelectableHeroInfo[] SELECTABLE_HERO_INFO = new SelectableHeroInfo[]
    {
        new SelectableHeroInfo() {HeroName = Define.RESOURCE_HERO_ARCANE_MAGE, HeroPortraitName = Define.RESOURCE_HERO_PORTRAIT_ARCANE_MAGE, SelectHeroButtonName = Define.RESOURCE_SELECTABLE_HERO_ARCANE_MAGE, PortraitPosition = new Vector3(0f, 127f, 0f), PortraitSize = new Vector2(300f, 500f) },
        new SelectableHeroInfo() {HeroName = Define.RESOURCE_HERO_KNIGHT, HeroPortraitName = Define.RESOURCE_HERO_PORTRAIT_KNIGHT, SelectHeroButtonName = Define.RESOURCE_SELECTABLE_HERO_KNIGHT, PortraitPosition = new Vector3(0f, 100f, 0f), PortraitSize = new Vector2(400f, 500f) }
    };

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindImage(typeof(EImages));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.ExitButton).gameObject, _ExitButton);
        _selectHeroButton = _GetButton((int)EButtons.SelectHeroButton).gameObject;
        _unlockHeroButton = _GetButton((int)EButtons.UnlockHeroButton).gameObject;
        _BindEvent(_selectHeroButton, _ClickSelectHeroButton);
        _BindEvent(_unlockHeroButton, _ClickUnlockHeroButton);

        _selectedHeroPanel = _GetGameObject((int)EGameObjects.SelectedHeroPanel);
        _selectableHeroContent = _GetGameObject((int)EGameObjects.SelectableHeroContent);
        for (int ii = 0; ii < SELECTABLE_HERO_INFO.Length; ++ii)
        {
            Manager.Instance.Resource.Instantiate(Define.RESOURCE_UI_SELECTABLE_HERO, _selectableHeroContent.transform, (selectableHeroGO) =>
            {
                var selectableHeroUI = Utils.GetOrAddComponent<UI_SelectableHero>(selectableHeroGO);
                selectableHeroUI.SetSelectableHero(ii, SELECTABLE_HERO_INFO[ii].HeroName, SELECTABLE_HERO_INFO[ii].SelectHeroButtonName);
                ChangeSelectedHeroHandler -= selectableHeroUI.ChangeSelectHero;
                ChangeSelectedHeroHandler += selectableHeroUI.ChangeSelectHero;
                NeedToGoldHandler -= selectableHeroUI.SetNeedToGold;
                NeedToGoldHandler += selectableHeroUI.SetNeedToGold;

                selectableHeroUI.SelectHeroHandler -= _SelectHero;
                selectableHeroUI.SelectHeroHandler += _SelectHero;
            });
        }
        var blankCount = VISIBLE_COUNT - (SELECTABLE_HERO_INFO.Length % VISIBLE_COUNT);
        for (int ii = 0; ii < blankCount; ++ii)
            Manager.Instance.Resource.Instantiate(Define.RESOURCE_UI_BLANK_SELECTABLE_HERO, _selectableHeroContent.transform, (blankSelectableHero) => { });

        _selectHeroPortrait = _GetImage((int)EImages.SelectHeroPortrait);
        _selectHeroPortraitRectTransform = Utils.GetOrAddComponent<RectTransform>(_selectHeroPortrait.gameObject);

        _ownedGoldText = _GetText((int)ETexts.OwnedGoldText);
    }

    #region Event
    private void _ExitButton()
    {
        ClosePopupUI();
    }

    private void _ClickSelectHeroButton()
    {
        _selectedHeroIndex = _selectHeroIndex;
        ChangeSelectedHeroHandler?.Invoke(_selectedHeroIndex);
        Manager.Instance.SaveData.SelectHero = SELECTABLE_HERO_INFO[_selectedHeroIndex].HeroName;
        _ActiveHeroButton(false, true, false);
    }

    private void _ClickUnlockHeroButton()
    {
        _ownedGoldText.text = Manager.Instance.SaveData.OwnedGold.ToString();
        _ActiveHeroButton(true, false, false);
        Manager.Instance.SaveData.SetOwnedHero(_selectHeroIndex);
        Manager.Instance.SaveData.OwnedGold -= Manager.Instance.Data.CostToObtainHeroDataList[_selectHeroIndex].NeedGold;
    }
    #endregion

    public void SetSelectHero()
    {
        _ownedGoldText.text = Manager.Instance.SaveData.OwnedGold.ToString();
        var selectHeroName = Manager.Instance.SaveData.SelectHero;
        for (int ii = 0; ii < SELECTABLE_HERO_INFO.Length; ++ii)
        {
            if (SELECTABLE_HERO_INFO[ii].HeroName.Equals(selectHeroName))
            {
                _selectHeroIndex = ii;
                _selectedHeroIndex = ii;
                _SetHeroPortrait();

                ChangeSelectedHeroHandler?.Invoke(_selectedHeroIndex);
                break;
            }
        }
        NeedToGoldHandler?.Invoke();
        _ActiveHeroButton(false, true, false);
    }

    private void _SelectHero(int index)
    {
        if (index == _selectHeroIndex)
            return;

        _selectHeroIndex = index;
        _SetHeroPortrait();
        if (Manager.Instance.SaveData.OwnedHeroes[_selectHeroIndex] < INIT_HERO_LEVEL)
        {
            if (Manager.Instance.SaveData.OwnedGold < Manager.Instance.Data.CostToObtainHeroDataList[_selectHeroIndex].NeedGold)
                _ActiveHeroButton(false, false, false);
            else
                _ActiveHeroButton(false, false, true);
        }
        else
        {
            if (index == _selectedHeroIndex)
                _ActiveHeroButton(false, true, false);
            else
                _ActiveHeroButton(true, false, false);
        }
    }

    private void _SetHeroPortrait()
    {
        Manager.Instance.Resource.LoadAsync<Sprite>(SELECTABLE_HERO_INFO[_selectHeroIndex].HeroPortraitName, (sprite) => { _selectHeroPortrait.sprite = sprite; });
        _selectHeroPortraitRectTransform.anchoredPosition = SELECTABLE_HERO_INFO[_selectHeroIndex].PortraitPosition;
        _selectHeroPortraitRectTransform.sizeDelta = SELECTABLE_HERO_INFO[_selectHeroIndex].PortraitSize;
    }

    private void _ActiveHeroButton(bool activeSelectHeroButton, bool activeSelectedHeroPanel, bool activeUnlockHeroButton)
    {
        Utils.SetActive(_selectHeroButton, activeSelectHeroButton);
        Utils.SetActive(_selectedHeroPanel, activeSelectedHeroPanel);
        Utils.SetActive(_unlockHeroButton, activeUnlockHeroButton);
    }
}
