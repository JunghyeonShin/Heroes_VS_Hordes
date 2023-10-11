using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Talent : UI_Element
{
    private enum EGameObjects
    {
        BlockPanel
    }

    private enum ETexts
    {
        TalentLevelText,
        TalentText
    }

    private GameObject _blockPanel;

    private TextMeshProUGUI _talentLevelText;
    private TextMeshProUGUI _TalentText;

    protected override void _Init()
    {
        _BindGameObject(typeof(EGameObjects));
        _BindText(typeof(ETexts));

        _blockPanel = _GetGameObject((int)EGameObjects.BlockPanel);

        _talentLevelText = _GetText((int)ETexts.TalentLevelText);
        _TalentText = _GetText((int)ETexts.TalentText);
    }
}
