using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EnhanceHeroAbility : UI_Popup
{
    private enum EButtons
    {
        TestButton
    }

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));

        _BindEvent(_GetButton((int)EButtons.TestButton).gameObject, _RestartIngame);
    }


    #region Event
    private void _RestartIngame()
    {
        var ingame = Manager.Instance.Ingame;
        _ClosePopupUI();
        ingame.ControlIngame(true);
        --ingame.HeroLevelUpCount;
        ingame.EnhanceHeroAbility();
    }
    #endregion
}
