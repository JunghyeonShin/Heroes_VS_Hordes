using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Talent : UI_Element
{
    private enum EGameObjects
    {
        TalentPanel,
        BlockPanel
    }

    private enum EImages
    {
        TalentIcon
    }

    private enum ETexts
    {
        TalentLevelText,
        TalentText
    }

    private GameObject _talentPanel;
    private GameObject _blockPanel;

    private Image _talentIcon;

    private TextMeshProUGUI _talentLevelText;
    private TextMeshProUGUI _TalentText;

    private int _talentIndex;

    private const int INIT_TALENT_LEVEL = 0;

    protected override void _Init()
    {
        _BindGameObject(typeof(EGameObjects));
        _BindImage(typeof(EImages));
        _BindText(typeof(ETexts));

        _talentPanel = _GetGameObject((int)EGameObjects.TalentPanel);
        _blockPanel = _GetGameObject((int)EGameObjects.BlockPanel);

        _talentIcon = _GetImage((int)EImages.TalentIcon);

        _talentLevelText = _GetText((int)ETexts.TalentLevelText);
        _TalentText = _GetText((int)ETexts.TalentText);
    }

    public void SetTalent(int index, string talentIconName)
    {
        _talentIndex = index;
        Manager.Instance.Resource.LoadAsync<Sprite>(talentIconName, (sprite) => { _talentIcon.sprite = sprite; });
        ChangeTalent(_talentIndex);
    }

    public void ChangeTalent(int index)
    {
        if (index != _talentIndex)
            return;

        var talentLevel = Manager.Instance.SaveData.OwnedTalents[_talentIndex];
        if (talentLevel <= INIT_TALENT_LEVEL)
            _ActivePanel(false, true);
        else
        {
            _talentLevelText.text = talentLevel.ToString();
            _TalentText.text = Manager.Instance.Data.TalentInfoDataList[_talentIndex][talentLevel].Description;
            _ActivePanel(true, false);
        }
    }

    private void _ActivePanel(bool _activeTalentPanel, bool _activeBlockPanel)
    {
        Utils.SetActive(_talentPanel, _activeTalentPanel);
        Utils.SetActive(_blockPanel, _activeBlockPanel);
    }
}
