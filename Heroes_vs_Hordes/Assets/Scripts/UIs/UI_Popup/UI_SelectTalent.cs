using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectTalent : UI_Popup
{
    private enum EImages
    {
        BackgroundPanel,
        SelectTalentIcon
    }

    private enum ETexts
    {
        SelectTalentLevelText
    }

    public event Action CloseSelectTalentUIHandler;

    private Image _selectTalentIcon;

    private TextMeshProUGUI _selectTalentLevelText;

    private const int ADJUST_TALENT_LEVEL = 1;

    protected override void _Init()
    {
        _BindImage(typeof(EImages));
        _BindText(typeof(ETexts));

        _BindEvent(_GetImage((int)EImages.BackgroundPanel).gameObject, _ExitButton);
        _selectTalentIcon = _GetImage((int)EImages.SelectTalentIcon);

        _selectTalentLevelText = _GetText((int)ETexts.SelectTalentLevelText);
    }

    #region Event
    private void _ExitButton()
    {
        CloseSelectTalentUIHandler?.Invoke();
        ClosePopupUI();
    }
    #endregion

    public void SetSelectTalent(int index, string talentIconName)
    {
        Manager.Instance.Resource.LoadAsync<Sprite>(talentIconName, (sprite) => { _selectTalentIcon.sprite = sprite; });
        _selectTalentLevelText.text = (Manager.Instance.SaveData.OwnedTalents[index] + ADJUST_TALENT_LEVEL).ToString();
    }
}
