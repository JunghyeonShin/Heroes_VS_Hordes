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
        RewardItem
    }

    private enum ETexts
    {
        RewardGoldText
    }

    private GameObject _rewardItem;

    private TextMeshProUGUI _rewardGoldText;

    private const int EMPTY_REWARD_GOLD = 0;

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.ExitIngameButton).gameObject, _ExitIngame);

        _rewardItem = _GetGameObject((int)EGameObjects.RewardItem);

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
            Utils.SetActive(_rewardItem, false);
        else
        {
            Utils.SetActive(_rewardItem, true);
            _rewardGoldText.text = rewardGold.ToString();
        }
    }
}
