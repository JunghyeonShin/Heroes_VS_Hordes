using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SelectAbility : UI_Element
{
    private enum EButtons
    {
        SelectAbilityButton
    }

    private enum ETexts
    {
        SelectAbilityLevelText,
        SelectAbilityNameText,
        SelectAbilityDescriptionText
    }

    public event Action OnSelectAbilityHandler;

    private TextMeshProUGUI _selectAbilityLevelText;
    private TextMeshProUGUI _selectAbilityNameText;
    private TextMeshProUGUI _selectAbilityDescriptionText;

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.SelectAbilityButton).gameObject, OnClick);

        _selectAbilityLevelText = _GetText((int)ETexts.SelectAbilityLevelText);
        _selectAbilityNameText = _GetText((int)ETexts.SelectAbilityNameText);
        _selectAbilityDescriptionText = _GetText((int)ETexts.SelectAbilityDescriptionText);
    }

    #region Event
    private void OnClick()
    {
        var ingame = Manager.Instance.Ingame;
        OnSelectAbilityHandler?.Invoke();
        ingame.ControlIngame(true);
        --ingame.HeroLevelUpCount;
        ingame.EnhanceHeroAbility();
    }
    #endregion

    public void InitSelectAbilityUI(Transform parent)
    {
        transform.SetParent(parent);
        transform.localScale = Vector3.one;
        Utils.SetActive(gameObject, true);
    }

    public void ReturnSelectAbilityUI()
    {
        Manager.Instance.UI.ReturnElementUI(Define.RESOURCE_UI_SELECT_ABILITY, gameObject);
    }
}
