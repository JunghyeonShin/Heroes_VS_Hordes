using System;
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

    public event Action<int> ChangeTalentHandler;

    private GameObject _talentContent;

    private TextMeshProUGUI _totalTalentLevelText;
    private TextMeshProUGUI _ownedGoldText;
    private TextMeshProUGUI _needToGoldText;

    private int _drawTalentIndex;

    private const int MIN_DRAW_TALENT_INDEX = 0;

    private readonly string[] TALENT_ICONS_NAMES = new string[]
    {
        Define.RESOURCE_SPRITES_ICON_TALENT_SWORD,
        Define.RESOURCE_SPRITES_ICON_TALENT_HEART,
        Define.RESOURCE_SPRITES_ICON_TALENT_BOOTS,
        Define.RESOURCE_SPRITES_ICON_TALENT_BOMB,
        Define.RESOURCE_SPRITES_ICON_TALENT_STOPWATCH
    };

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.ExitButton).gameObject, _ExitButton);
        _BindEvent(_GetButton((int)EButtons.DrawTalentButton).gameObject, _DrawTalentButton);

        _talentContent = _GetGameObject((int)EGameObjects.TalentContent);
        for (int ii = 0; ii < Manager.Instance.SaveData.OwnedTalents.Length; ++ii)
        {
            Manager.Instance.Resource.Instantiate(Define.RESOURCE_UI_TALENT, _talentContent.transform, (talentGO) =>
            {
                var talentUI = Utils.GetOrAddComponent<UI_Talent>(talentGO);
                talentUI.SetTalent(ii, TALENT_ICONS_NAMES[ii]);
                ChangeTalentHandler -= talentUI.ChangeTalent;
                ChangeTalentHandler += talentUI.ChangeTalent;
            });
        }

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
        var totlaOwnedTalentLevel = Manager.Instance.SaveData.TotalOwnedTalentsLevel;
        var needGold = Manager.Instance.Data.CostToObtainTalentDataList[totlaOwnedTalentLevel].NeedGold;
        if (Manager.Instance.SaveData.OwnedGold < needGold)
            return;

        _drawTalentIndex = UnityEngine.Random.Range(MIN_DRAW_TALENT_INDEX, Manager.Instance.SaveData.OwnedTalents.Length);
        Manager.Instance.UI.ShowPopupUI<UI_SelectTalent>(Define.RESOURCE_UI_SELECT_TALENT, (selectTalentUI) =>
        {
            selectTalentUI.SetSelectTalent(_drawTalentIndex, TALENT_ICONS_NAMES[_drawTalentIndex]);
            selectTalentUI.CloseSelectTalentUIHandler -= _ChangeSelectTalent;
            selectTalentUI.CloseSelectTalentUIHandler += _ChangeSelectTalent;
        });
    }
    #endregion

    public void SetDrawTalent()
    {
        var totlaOwnedTalentLevel = Manager.Instance.SaveData.TotalOwnedTalentsLevel;
        var ownedGold = Manager.Instance.SaveData.OwnedGold;
        _totalTalentLevelText.text = totlaOwnedTalentLevel.ToString();
        _ownedGoldText.text = ownedGold.ToString();
        var needGold = Manager.Instance.Data.CostToObtainTalentDataList[totlaOwnedTalentLevel].NeedGold;
        _needToGoldText.text = needGold.ToString();
        if (ownedGold < needGold)
            _needToGoldText.color = Color.red;
        else
            _needToGoldText.color = Color.white;
    }

    private void _ChangeSelectTalent()
    {
        var totlaOwnedTalentLevel = Manager.Instance.SaveData.TotalOwnedTalentsLevel;
        var needGold = Manager.Instance.Data.CostToObtainTalentDataList[totlaOwnedTalentLevel].NeedGold;
        Manager.Instance.SaveData.SetOwnedTalent(_drawTalentIndex);
        SetDrawTalent();
        ChangeTalentHandler?.Invoke(_drawTalentIndex);
        Manager.Instance.SaveData.OwnedGold -= needGold;
    }
}
