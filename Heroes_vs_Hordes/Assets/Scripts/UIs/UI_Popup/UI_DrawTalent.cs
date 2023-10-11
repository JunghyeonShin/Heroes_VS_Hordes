using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DrawTalent : UI_Popup
{
    private enum EButtons
    {
        ExitButton,
        DrawTalentButton
    }

    private enum EGameObjects
    {
        TalentContent
    }

    private enum ETexts
    {
        TotalTalentLevelText,
        OwnedGoldText,
        NeedToGoldText
    }

    private GameObject _talentContent;

    private TextMeshProUGUI _totalTalentLevelText;
    private TextMeshProUGUI _ownedGoldText;
    private TextMeshProUGUI _needToGoldText;

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.ExitButton).gameObject, _ExitButton);
        _BindEvent(_GetButton((int)EButtons.DrawTalentButton).gameObject, _DrawTalentButton);

        _talentContent = _GetGameObject((int)EGameObjects.TalentContent);

        _totalTalentLevelText = _GetText((int)ETexts.TotalTalentLevelText);
        _ownedGoldText = _GetText((int)ETexts.OwnedGoldText);
        _needToGoldText = _GetText((int)ETexts.NeedToGoldText);
    }

    #region Event
    private void _ExitButton()
    {
        ClosePopupUI();
    }

    private void _DrawTalentButton()
    {

    }
    #endregion

    public void SetDrawTalent()
    {

    }
}
