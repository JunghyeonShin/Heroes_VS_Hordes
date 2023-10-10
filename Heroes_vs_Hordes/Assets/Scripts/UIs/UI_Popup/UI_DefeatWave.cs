using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DefeatWave : UI_Popup
{
    private enum EButtons
    {
        ExitIngameButton
    }

    private enum EGameObjects
    {
        RewardPanel
    }

    private enum ETexts
    {
        RewardGoldText
    }

    private GameObject _rewardPanel;

    private TextMeshProUGUI _rewardGoldText;

    private const int EMPTY_REWARD_GOLD = 0;

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.ExitIngameButton).gameObject, _ExitIngame);

        _rewardPanel = _GetGameObject((int)EGameObjects.RewardPanel);

        _rewardGoldText = _GetText((int)ETexts.RewardGoldText);
    }

    #region Event
    private void _ExitIngame()
    {
        var manager = Manager.Instance;
        manager.Ingame.DefeatIngame = true;
        manager.Ingame.ExitIngame();
    }
    #endregion

    public void SetRewardItem()
    {
        var rewardGold = Manager.Instance.Ingame.ClearWaveReward;
        if (rewardGold <= EMPTY_REWARD_GOLD)
            Utils.SetActive(_rewardPanel, false);
        else
        {
            Utils.SetActive(_rewardPanel, true);
            _rewardGoldText.text = rewardGold.ToString();
        }
    }
}
