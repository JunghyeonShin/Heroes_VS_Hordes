using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ability : UI_Element
{
    private enum EGameObjects
    {
        OwnedAbilityPanel
    }

    private enum EImages
    {
        AbilityIcon
    }

    private enum ETexts
    {
        AbilityLevelText
    }

    private GameObject _ownedAblityPanel;
    private Image _abilityIcon;
    private TextMeshProUGUI _abilityLevelText;

    protected override void _Init()
    {
        _BindGameObject(typeof(EGameObjects));
        _BindImage(typeof(EImages));
        _BindText(typeof(ETexts));

        _ownedAblityPanel = _GetGameObject((int)EGameObjects.OwnedAbilityPanel);
        _abilityIcon = _GetImage((int)EImages.AbilityIcon);
        _abilityLevelText = _GetText((int)ETexts.AbilityLevelText);
    }

    public void InitAbilityUI(Transform parent)
    {
        transform.SetParent(parent);
        transform.localScale = Vector3.one;
        Utils.SetActive(gameObject, true);
        Utils.SetActive(_ownedAblityPanel, false);
    }

    public void UpdateAbilityUI(string abilityName)
    {
        if (string.IsNullOrEmpty(abilityName))
            return;
        if (false == Define.ABILITY_INFO_DIC.TryGetValue(abilityName, out var abilityInfo))
            return;

        Manager.Instance.Resource.LoadAsync<Sprite>(abilityInfo.SpriteName, (sprite) =>
        {
            _abilityIcon.sprite = sprite;
            _abilityLevelText.text = Manager.Instance.Ingame.GetOwnedAbilityLevel(abilityName).ToString();
        });
        Utils.SetActive(_ownedAblityPanel, true);
    }

    public void ReturnAbilityUI()
    {
        Manager.Instance.UI.ReturnElementUI(Define.RESOURCE_UI_ABILITY, gameObject);
    }
}
