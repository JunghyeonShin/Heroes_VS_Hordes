using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectAbility : UI_Element
{
    private enum EButtons
    {
        SelectAbilityButton
    }

    private enum EImages
    {
        SelectAbilityIcon
    }

    private enum ETexts
    {
        SelectAbilityLevelText,
        SelectAbilityNameText,
        SelectAbilityDescriptionText
    }

    public event Action OnSelectAbilityHandler;

    private Image _selectAbilityIcon;

    private TextMeshProUGUI _selectAbilityLevelText;
    private TextMeshProUGUI _selectAbilityNameText;
    private TextMeshProUGUI _selectAbilityDescriptionText;

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindImage(typeof(EImages));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.SelectAbilityButton).gameObject, _SelectAbility);

        _selectAbilityIcon = _GetImage((int)EImages.SelectAbilityIcon);

        _selectAbilityLevelText = _GetText((int)ETexts.SelectAbilityLevelText);
        _selectAbilityNameText = _GetText((int)ETexts.SelectAbilityNameText);
        _selectAbilityDescriptionText = _GetText((int)ETexts.SelectAbilityDescriptionText);
    }

    #region Event
    private void _SelectAbility()
    {
        var ingame = Manager.Instance.Ingame;
        OnSelectAbilityHandler?.Invoke();
        ingame.ControlIngame(true);
        --ingame.HeroLevelUpCount;
        ingame.LevelUpHeroAbility();
    }
    #endregion

    public void InitSelectAbilityUI(Transform parent)
    {
        transform.SetParent(parent);
        transform.localScale = Vector3.one;
        Utils.SetActive(gameObject, true);
    }

    public void UpdateSelectAbilityUI(string abilityName)
    {
        if (string.IsNullOrEmpty(abilityName))
            return;
        if (false == Define.ABILITY_SPRITE_DIC.TryGetValue(abilityName, out var abilityInfo))
            return;

        Manager.Instance.Resource.LoadAsync<Sprite>(abilityInfo.SpriteName, (sprite) =>
        {
            _selectAbilityIcon.sprite = sprite;

            var abilityDescription = Manager.Instance.Data.AbilityDescriptionDic[abilityName][Manager.Instance.Ingame.GetOwnedAbilityLevel(abilityName)];
            _selectAbilityLevelText.text = abilityDescription.AbilityLevel;
            _selectAbilityNameText.text = abilityDescription.AbilityName;
            _selectAbilityDescriptionText.text = abilityDescription.AbilityDescription;
        });
    }

    public void ReturnSelectAbilityUI()
    {
        Manager.Instance.UI.ReturnElementUI(Define.RESOURCE_UI_SELECT_ABILITY, gameObject);
    }
}
