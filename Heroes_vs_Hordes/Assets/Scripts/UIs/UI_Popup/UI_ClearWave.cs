using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ClearWave : UI_Popup
{
    private enum EButtons
    {
        StartNextWaveButton
    }

    private enum ETexts
    {
        ClearWaveText,
        RewardGoldText
    }

    private TextMeshProUGUI _clearWaveText;

    private const int ADJUST_WAVE = 1;

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.StartNextWaveButton).gameObject, _StartNextWave);

        _clearWaveText = _GetText((int)ETexts.ClearWaveText);
    }

    public void SetClearWaveText()
    {
        _clearWaveText.text = $"웨이브 {Manager.Instance.Ingame.CurrentWaveIndex + ADJUST_WAVE} 클리어";
    }

    private void _StartNextWave()
    {
        var ingame = Manager.Instance.Ingame;
        ingame.StartIngame(ingame.CurrentWaveIndex + ADJUST_WAVE);
        _ClosePopupUI();
    }
}
