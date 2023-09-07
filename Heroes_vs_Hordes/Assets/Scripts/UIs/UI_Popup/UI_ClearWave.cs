using System.Collections;
using System.Collections.Generic;
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

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.StartNextWaveButton).gameObject, _StartNextWave);
    }

    private void _StartNextWave()
    {

    }
}
