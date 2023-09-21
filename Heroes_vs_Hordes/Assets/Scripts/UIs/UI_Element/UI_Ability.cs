using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void _Init()
    {
        _BindGameObject(typeof(EGameObjects));
        _BindImage(typeof(EImages));
        _BindText(typeof(ETexts));

        _ownedAblityPanel = _GetGameObject((int)EGameObjects.OwnedAbilityPanel);
    }

    public void InitAbilityUI(Transform parent)
    {
        transform.SetParent(parent);
        transform.localScale = Vector3.one;
        Utils.SetActive(gameObject, true);
        Utils.SetActive(_ownedAblityPanel, false);
    }

    public void ReturnAbilityUI()
    {
        Manager.Instance.UI.ReturnElementUI(Define.RESOURCE_UI_ABILITY, gameObject);
    }
}
